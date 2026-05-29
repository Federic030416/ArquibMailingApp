namespace ArquibMailing.Domain.Entities;

/// <summary>
/// Representa a un destinatario de correo electrónico.
/// Entidad pura del dominio — no tiene dependencias externas.
/// </summary>
public class Destinatario
{
    public string Id          { get; set; } = string.Empty;
    public string Nombre      { get; set; } = string.Empty;
    public string Email       { get; set; } = string.Empty;
    public string Consecutivo { get; set; } = string.Empty;
    public string Password    { get; set; } = string.Empty;
    public string FileName    { get; set; } = string.Empty;
}
