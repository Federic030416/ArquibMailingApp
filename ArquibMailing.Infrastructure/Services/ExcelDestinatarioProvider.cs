namespace ArquibMailing.Infrastructure.Services;

using ArquibMailing.Application.Interfaces;
using ArquibMailing.Domain.Entities;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;

/// <summary>
/// Implementación concreta de IDestinatarioProvider.
/// Lee los destinatarios desde un archivo Excel usando EPPlus,
/// exactamente como lo hacía ExcelManager en el backend original.
/// </summary>
public class ExcelDestinatarioProvider : IDestinatarioProvider
{
    private readonly IConfiguration _config;

    // Cache en memoria para no releer el Excel en cada consulta
    private List<Destinatario>? _cache;

    public ExcelDestinatarioProvider(IConfiguration config)
    {
        ExcelPackage.License.SetNonCommercialOrganization("Arquib");
        _config = config;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Destinatario>> ObtenerTodosAsync()
    {
        // Retorna del cache si ya fue cargado
        if (_cache != null)
            return _cache;

        return await Task.Run(CargarDesdeExcel);
    }

    /// <inheritdoc/>
    public async Task<Destinatario?> BuscarAsync(string criterio)
    {
        var todos = await ObtenerTodosAsync();
        return todos.FirstOrDefault(d =>
            d.Consecutivo.Equals(criterio, StringComparison.OrdinalIgnoreCase) ||
            d.Nombre.Contains(criterio, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Lee el archivo Excel y mapea cada fila a una entidad Destinatario.
    /// </summary>
    private List<Destinatario> CargarDesdeExcel()
    {
        var resultado   = new List<Destinatario>();
        var excelConfig = _config.GetSection("Excel");

        var rutaExcel = excelConfig["Path"]
            ?? throw new Exception("Excel: Path no configurado en appsettings.json");
        var nombreTabla = excelConfig["Table"]
            ?? throw new Exception("Excel: Table no configurado en appsettings.json");

        // Nombres de columnas configurables (igual que en el backend original)
        var colConsec   = excelConfig["Columns:ConsecColumn"] ?? "CONSECUTIVO";
        var colEmail    = excelConfig["Columns:SendColumn"]   ?? "CORREOELECTRONICO";
        var colArchivo  = excelConfig["Columns:FileColumn"]   ?? "ARCHIVO";
        var colPassword = excelConfig["Columns:ProtColumn"]   ?? "PASSWORD";

        using var package = new ExcelPackage(new FileInfo(rutaExcel));
        var hoja  = package.Workbook.Worksheets.First();
        var tabla = hoja.Tables[nombreTabla]
            ?? throw new Exception($"Excel: No se encontró la tabla '{nombreTabla}'");

        // Mapear los índices de columnas por nombre
        var headers = tabla.Columns
            .Select((c, i) => new { Nombre = c.Name.ToUpper(), Indice = i })
            .ToDictionary(x => x.Nombre, x => x.Indice);

        int colBase = tabla.Address.Start.Column;

        for (int fila = tabla.Address.Start.Row + 1; fila <= tabla.Address.End.Row; fila++)
        {
            var consecutivo = hoja.Cells[fila, colBase + headers[colConsec.ToUpper()]].Text.Trim();
            var email       = hoja.Cells[fila, colBase + headers[colEmail.ToUpper()]].Text.Trim();
            var archivo     = hoja.Cells[fila, colBase + headers[colArchivo.ToUpper()]].Text.Trim();
            var password    = hoja.Cells[fila, colBase + headers[colPassword.ToUpper()]].Text.Trim();

            if (string.IsNullOrEmpty(consecutivo) && string.IsNullOrEmpty(email))
                continue;

            resultado.Add(new Destinatario
            {
                Id          = consecutivo,
                Consecutivo = consecutivo,
                Email       = email,
                FileName    = archivo,
                Password    = password,
                // El nombre se toma del archivo si no hay columna dedicada
                Nombre      = !string.IsNullOrEmpty(archivo) ? Path.GetFileNameWithoutExtension(archivo) : consecutivo
            });
        }

        _cache = resultado;
        return resultado;
    }
}
