namespace ArquibMailing.Application.Interfaces;

/// <summary>
/// Contrato para el servicio de envío de correos.
/// La capa de Presentación y Aplicación solo conocen esta interfaz,
/// nunca la implementación concreta (Microsoft Graph).
/// </summary>
public interface IMailingService
{
    /// <summary>
    /// Envía un correo con el archivo PDF adjunto al destinatario indicado.
    /// </summary>
    /// <param name="rutaArchivo">Ruta local del PDF a adjuntar</param>
    /// <param name="emailDestino">Correo del destinatario</param>
    /// <returns>True si el envío fue exitoso, False si falló</returns>
    Task<bool> EnviarCorreoAsync(string rutaArchivo, string emailDestino);
}
