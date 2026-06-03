namespace ArquibMailing.Maui.ViewModels;

using ArquibMailing.Application.UseCases;
using System.Windows.Input;

/// <summary>
/// ViewModel del Dashboard.
/// Muestra un resumen del estado del sistema y accesos rápidos.
/// </summary>
public class DashboardViewModel : BaseViewModel
{
    private readonly ObtenerDestinatariosUseCase _obtenerUseCase;

    private int    _totalDestinatarios;
    private int    _totalPdfsEnInputFolder;
    private string _rutaInput = string.Empty;
    private string _ultimaActividad = "Sin actividad reciente";

    public DashboardViewModel(ObtenerDestinatariosUseCase obtenerUseCase)
    {
        Title = "Dashboard";
        _obtenerUseCase = obtenerUseCase;

        CargarResumenCommand = new Command(async () => await CargarResumenAsync());
        ContinuarCommand = new Command(async () => await ContinuarAsync());
        CancelarCommand = new Command(()=> MostrarInvitacion = false);
    }

    //  Propiedades observables

    /// <summary>Total de destinatarios cargados desde el Excel.</summary>
    public int TotalDestinatarios
    {
        get => _totalDestinatarios;
        set => SetProperty(ref _totalDestinatarios, value);
    }

    private bool _mostrarInvitacion = true;

    public bool MostrarInvitacion
    {
        get => _mostrarInvitacion;
        set => SetProperty(ref _mostrarInvitacion, value);
    }

    /// <summary>Cantidad de PDFs en la carpeta input/ listos para enviar.</summary>
    public int TotalPdfsListos
    {
        get => _totalPdfsEnInputFolder;
        set => SetProperty(ref _totalPdfsEnInputFolder, value);
    }

    /// <summary>Ruta de la carpeta de entrada configurada.</summary>
    public string RutaInput
    {
        get => _rutaInput;
        set => SetProperty(ref _rutaInput, value);
    }

    /// <summary>Texto de la última actividad registrada.</summary>
    public string UltimaActividad
    {
        get => _ultimaActividad;
        set => SetProperty(ref _ultimaActividad, value);
    }

    //  Comandos 

    public ICommand CargarResumenCommand {get;}
    public ICommand ContinuarCommand {get;}
    public ICommand CancelarCommand {get;}

    //  Lógica

    /// <summary>
    /// Carga las métricas del resumen al entrar al Dashboard.
    /// </summary>
    public async Task CargarResumenAsync()
    {
        IsBusy = true;
        try
        {
            // Total de destinatarios desde el Excel
            var destinatarios    = await _obtenerUseCase.ObtenerTodosAsync();
            TotalDestinatarios   = destinatarios.Count();

            // PDFs en la carpeta input/
            var inputPath        = Path.Combine(AppContext.BaseDirectory, "input");
            RutaInput            = inputPath;

            if (Directory.Exists(inputPath))
            {
                var pdfs         = Directory.GetFiles(inputPath, "*.pdf");
                TotalPdfsListos  = pdfs.Length;
            }
            else
            {
                TotalPdfsListos  = 0;
            }

            UltimaActividad = $"Resumen actualizado: {DateTime.Now:dd/MM/yyyy HH:mm}";
        }
        catch (Exception ex)
        {
            UltimaActividad = $"Error al cargar resumen: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task ContinuarAsync()
    {
        await Shell.Current.GoToAsync("//Campaña");
    }
}
