namespace ArquibMailing.Application.Interfaces;

using ArquibMailing.Domain.Entities;

/// <summary>
/// Contrato para obtener la lista de destinatarios desde la fuente de datos
/// (Excel, base de datos, etc). La capa de presentación nunca sabe
/// de dónde vienen los datos.
/// </summary>
public interface IDestinatarioProvider
{
    /// <summary>
    /// Retorna todos los destinatarios disponibles.
    /// </summary>
    Task<IEnumerable<Destinatario>> ObtenerTodosAsync();

    /// <summary>
    /// Busca un destinatario por su consecutivo o nombre.
    /// </summary>
    Task<Destinatario?> BuscarAsync(string criterio);
}
