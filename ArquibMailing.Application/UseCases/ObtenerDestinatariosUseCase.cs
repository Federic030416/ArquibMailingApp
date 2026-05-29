namespace ArquibMailing.Application.UseCases;

using ArquibMailing.Application.Interfaces;
using ArquibMailing.Domain.Entities;

/// <summary>
/// Caso de Uso: Obtener y buscar destinatarios.
/// Centraliza la lógica de búsqueda independientemente de la fuente de datos.
/// </summary>
public class ObtenerDestinatariosUseCase
{
    private readonly IDestinatarioProvider _provider;

    public ObtenerDestinatariosUseCase(IDestinatarioProvider provider)
    {
        _provider = provider;
    }

    /// <summary>
    /// Retorna todos los destinatarios disponibles.
    /// </summary>
    public async Task<IEnumerable<Destinatario>> ObtenerTodosAsync()
    {
        return await _provider.ObtenerTodosAsync();
    }

    /// <summary>
    /// Busca destinatarios filtrando por nombre o consecutivo.
    /// </summary>
    public async Task<IEnumerable<Destinatario>> BuscarAsync(string criterio)
    {
        if (string.IsNullOrWhiteSpace(criterio))
            return await _provider.ObtenerTodosAsync();

        var todos = await _provider.ObtenerTodosAsync();

        // Filtra por nombre O por consecutivo (búsqueda insensible a mayúsculas)
        return todos.Where(d =>
            d.Nombre.Contains(criterio, StringComparison.OrdinalIgnoreCase) ||
            d.Consecutivo.Contains(criterio, StringComparison.OrdinalIgnoreCase));
    }
}
