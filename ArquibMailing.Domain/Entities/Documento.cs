namespace ArquibMailing.Domain.Entities;

/// <summary>
/// Estado posible de un envío de documento.
/// </summary>
public enum EstadoEnvio
{
    Pendiente,
    Enviado,
    Error
}

/// <summary>
/// Representa un documento PDF que será enviado por correo.
/// Entidad pura del dominio — no tiene dependencias externas.
/// </summary>
public class Documento
{
    public string      Consecutivo  { get; set; } = string.Empty;
    public string      NombreArchivo { get; set; } = string.Empty;
    public string      RutaArchivo  { get; set; } = string.Empty;
    public EstadoEnvio Estado       { get; set; } = EstadoEnvio.Pendiente;
    public Destinatario? Destinatario { get; set; }
    public DateTime?   FechaEnvio   { get; set; }
}
