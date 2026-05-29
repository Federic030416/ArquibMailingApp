namespace ArquibMailing.Infrastructure.Services;

using ArquibMailing.Application.Interfaces;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.Graph.Models;

/// <summary>
/// Implementación concreta de IMailingService usando Microsoft Graph API.
/// Este servicio envuelve la lógica del backend existente (WrkSendMassiveMails)
/// sin modificarlo — aplica el principio Open/Closed de SOLID.
/// </summary>
public class GraphMailingService : IMailingService
{
    private readonly IConfiguration _config;

    public GraphMailingService(IConfiguration config)
    {
        _config = config;
    }

    /// <inheritdoc/>
    public async Task<bool> EnviarCorreoAsync(string rutaArchivo, string emailDestino)
    {
        int maxIntentos = 3;

        for (int intento = 1; intento <= maxIntentos; intento++)
        {
            try
            {
                var graphConfig = _config.GetSection("Graph");

                // Autenticación con Azure AD usando credenciales de cliente
                var credential = new ClientSecretCredential(
                    graphConfig["TenantId"],
                    graphConfig["ClientId"],
                    graphConfig["ClientSecret"]
                );

                var graphClient = new GraphServiceClient(credential);
                var fileBytes  = await File.ReadAllBytesAsync(rutaArchivo);

                // Construir el mensaje de correo
                var message = new Message
                {
                    Subject = _config["Mail:Subject"],
                    Body = new ItemBody
                    {
                        ContentType = BodyType.Text,
                        Content     = _config["Mail:Body"]
                    },
                    ToRecipients = new List<Recipient>
                    {
                        new Recipient
                        {
                            EmailAddress = new EmailAddress { Address = emailDestino }
                        }
                    },
                    Attachments = new List<Microsoft.Graph.Models.Attachment>
                    {
                        new FileAttachment
                        {
                            OdataType    = "#microsoft.graph.fileAttachment",
                            Name         = Path.GetFileName(rutaArchivo),
                            ContentBytes = fileBytes
                        }
                    }
                };

                // Enviar usando la cuenta configurada
                await graphClient
                    .Users[graphConfig["UserEmail"]]
                    .SendMail
                    .PostAsync(new Microsoft.Graph.Users.Item.SendMail.SendMailPostRequestBody
                    {
                        Message        = message,
                        SaveToSentItems = true
                    });

                return true;
            }
            catch (ServiceException ex) when ((int)ex.ResponseStatusCode == 429)
            {
                // Throttling — Microsoft nos pide esperar
                int delay      = 5000;
                var retryAfter = ex.ResponseHeaders?
                    .FirstOrDefault(h => h.Key == "Retry-After").Value;

                if (retryAfter != null && int.TryParse(retryAfter.FirstOrDefault(), out int segundos))
                    delay = segundos * 1000;

                Console.WriteLine($"Throttling detectado. Esperando {delay} ms ...");
                await Task.Delay(delay);
                // Continúa el bucle para reintentar
            }
            catch (Exception ex)
            {
                // Propagar el error real para que la UI pueda mostrarlo
                throw new Exception($"Error al enviar correo a '{emailDestino}': {ex.Message}", ex);
            }
        }

        return false;
    }
}
