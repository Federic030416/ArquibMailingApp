namespace ArquibMailing.Application.UseCases;

using ArquibMailing.Application.Interfaces;
using ArquibMailing.Domain.Entities;

/// <summary>
/// Caso de Uso: Enviar un documento a un destinatario.
/// Este es el orquestador del flujo — coordina los servicios
/// pero no sabe CÓMO se envía ni de dónde vienen los datos.
/// </summary>
public class EnviarDocumentoUseCase
{
    private readonly IMailingService _mailingService;

    public EnviarDocumentoUseCase(IMailingService mailingService)
    {
        _mailingService = mailingService;
    }

    /// <summary>
    /// Ejecuta el envío del documento al destinatario dado.
    /// </summary>
    /// <param name="documento">Documento a enviar (debe tener RutaArchivo)</param>
    /// <param name="destinatario">Destinatario que recibirá el correo</param>
    /// <returns>True si el envío fue exitoso</returns>
    public async Task<bool> EjecutarAsync(Documento documento, Destinatario destinatario)
    {
        // Validaciones básicas del dominio
        if (string.IsNullOrWhiteSpace(destinatario.Email))
            throw new ArgumentException("El destinatario no tiene correo asignado.");

        if (string.IsNullOrWhiteSpace(documento.RutaArchivo))
            throw new ArgumentException("El documento no tiene una ruta de archivo válida.");

        if (!File.Exists(documento.RutaArchivo))
            throw new FileNotFoundException($"No se encontró el archivo: {documento.RutaArchivo}");

        // Delegar el envío al servicio (Infrastructure lo implementa)
        var resultado = await _mailingService.EnviarCorreoAsync(documento.RutaArchivo, destinatario.Email);

        // Actualizar el estado del documento según el resultado
        documento.Estado     = resultado ? EstadoEnvio.Enviado : EstadoEnvio.Error;
        documento.FechaEnvio = resultado ? DateTime.Now : null;

        return resultado;
    }
}
