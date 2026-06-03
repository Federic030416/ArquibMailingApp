namespace ArquibMailing.Maui.ViewModels;

using ArquibMailing.Application.UseCases;
using ArquibMailing.Domain.Entities;
using System.Collections.ObjectModel;
using System.Windows.Input;

/// <summary>
/// ViewModel para la página de Nuevo Envío.
/// Gestiona la selección de destinatario, el archivo PDF y el envío.
/// </summary>
public class NuevoEnvioViewModel : BaseViewModel
{
    private readonly EnviarDocumentoUseCase      _enviarUseCase;
    private readonly ObtenerDestinatariosUseCase _obtenerUseCase;


    private string        _rutaArchivo    = string.Empty;
    private string        _nombreArchivo  = "Ningún archivo seleccionado";
    private string        _mensajeEstado  = string.Empty;
    private bool          _envioExitoso;

    private bool          _esEnvioManual;
    private string        _correoManual   = string.Empty;
    private string        _textoBusqueda  = string.Empty;

    public NuevoEnvioViewModel(
        EnviarDocumentoUseCase      enviarUseCase,
        ObtenerDestinatariosUseCase obtenerUseCase)
    {
        Title           = "Nuevo Envío";
        _enviarUseCase  = enviarUseCase;
        _obtenerUseCase = obtenerUseCase;

        Destinatarios          = new ObservableCollection<Destinatario>();
        DestinatariosFiltrados = new ObservableCollection<Destinatario>();
        SeleccionarArchivoCommand = new Command(async () => await SeleccionarArchivoAsync());
        EnviarCommand             = new Command(async () => await EnviarAsync(), PuedeEnviar);
        CargarDestinatariosCommand = new Command(async () => await CargarDestinatariosAsync());
        CambiarModoListaCommand   = new Command(() => EsEnvioManual = false);
        CambiarModoManualCommand  = new Command(() => EsEnvioManual = true);
    }

    // ── Propiedades observables ──────────────────────────────────────────────

    public ObservableCollection<Destinatario> Destinatarios { get; }
    public ObservableCollection<Destinatario> DestinatariosFiltrados { get; }
    public ObservableCollection<Destinatario> DestinatariosSeleccionados { get; } = new();

    public bool EsEnvioManual
    {
        get => _esEnvioManual;
        set
        {
            SetProperty(ref _esEnvioManual, value);
            ((Command)EnviarCommand).ChangeCanExecute();
        }
    }

    public string CorreoManual
    {
        get => _correoManual;
        set
        {
            SetProperty(ref _correoManual, value);
            ((Command)EnviarCommand).ChangeCanExecute();
        }
    }

    public string TextoBusqueda
    {
        get => _textoBusqueda;
        set
        {
            SetProperty(ref _textoBusqueda, value);
            FiltrarDestinatarios();
        }
    }

    public bool TieneSeleccionados => DestinatariosSeleccionados.Count > 0;

    public string TextoSeleccionados => DestinatariosSeleccionados.Count > 0
        ? $"{DestinatariosSeleccionados.Count} destinatarios seleccionados"
        : string.Empty;

    public void NotificarSeleccionCambiada()
    {
        OnPropertyChanged(nameof(TieneSeleccionados));
        OnPropertyChanged(nameof(TextoSeleccionados));
        ((Command)EnviarCommand).ChangeCanExecute();
    }

    public string RutaArchivo
    {
        get => _rutaArchivo;
        set
        {
            SetProperty(ref _rutaArchivo, value);
            ((Command)EnviarCommand).ChangeCanExecute();
        }
    }

    public string NombreArchivo
    {
        get => _nombreArchivo;
        set => SetProperty(ref _nombreArchivo, value);
    }

    public string MensajeEstado
    {
        get => _mensajeEstado;
        set => SetProperty(ref _mensajeEstado, value);
    }

    public bool EnvioExitoso
    {
        get => _envioExitoso;
        set => SetProperty(ref _envioExitoso, value);
    }

    // ── Comandos ─────────────────────────────────────────────────────────────

    public ICommand SeleccionarArchivoCommand    { get; }
    public ICommand EnviarCommand                { get; }
    public ICommand CargarDestinatariosCommand   { get; }
    public ICommand CambiarModoListaCommand      { get; }
    public ICommand CambiarModoManualCommand     { get; }

    // ── Lógica ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Carga la lista de destinatarios desde el Excel al iniciar la página.
    /// </summary>
    public async Task CargarDestinatariosAsync()
    {
        IsBusy = true;
        try
        {
            var lista = await _obtenerUseCase.ObtenerTodosAsync();
            Destinatarios.Clear();
            DestinatariosFiltrados.Clear();
            foreach (var d in lista)
            {
                Destinatarios.Add(d);
                DestinatariosFiltrados.Add(d);
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Abre el selector de archivos del sistema operativo (solo PDFs).
    /// </summary>
    private async Task SeleccionarArchivoAsync()
    {
        var fileType = new FilePickerFileType(
            new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.WinUI, new[] { ".pdf" } }
            });

        var resultado = await FilePicker.Default.PickAsync(new PickOptions
        {
            FileTypes = fileType
        });

        if (resultado != null)
        {
            RutaArchivo   = resultado.FullPath;
            NombreArchivo = resultado.FileName;
        }
    }

    /// <summary>
    /// Ejecuta el caso de uso de envío y actualiza el estado en la UI.
    /// </summary>
    private async Task EnviarAsync()
    {
        IsBusy        = true;
        MensajeEstado = "Enviando correos de campaña...";

        try
        {
            List<Destinatario> destinatariosAEnviar = new();
            if (EsEnvioManual)
            {
                destinatariosAEnviar.Add(new Destinatario
                {
                    Id = "Manual",
                    Consecutivo = "Manual",
                    Email = CorreoManual.Trim(),
                    Nombre = CorreoManual.Trim()
                });
            }
            else
            {
                destinatariosAEnviar.AddRange(DestinatariosSeleccionados);
            }

            int exitos = 0;
            int fallidos = 0;

            foreach (var dest in destinatariosAEnviar)
            {
                var documento = new Documento
                {
                    Consecutivo   = dest.Consecutivo,
                    RutaArchivo   = RutaArchivo,
                    NombreArchivo = NombreArchivo
                };

                var exito = await _enviarUseCase.EjecutarAsync(documento, dest);
                if (exito) exitos++;
                else fallidos++;
            }

            EnvioExitoso = (fallidos == 0);
            if (fallidos == 0)
            {
                MensajeEstado = $"✅ Todos los correos ({exitos}) fueron enviados exitosamente.";
            }
            else
            {
                MensajeEstado = $"⚠️ Se enviaron {exitos} correos con éxito, pero fallaron {fallidos}.";
            }
        }
        catch (Exception ex)
        {
            EnvioExitoso  = false;
            MensajeEstado = $"❌ Error: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// El botón Enviar solo se habilita si hay archivo Y destinatario/correo válidos.
    /// </summary>
    private bool PuedeEnviar()
    {
        if (string.IsNullOrEmpty(RutaArchivo)) return false;
        
        if (EsEnvioManual)
            return !string.IsNullOrWhiteSpace(CorreoManual) && CorreoManual.Contains("@");
        else
            return DestinatariosSeleccionados.Count > 0;
    }

    /// <summary>
    /// Filtra la lista de destinatarios por nombre o consecutivo
    /// </summary>
    private void FiltrarDestinatarios()
    {
        DestinatariosFiltrados.Clear();
        if (string.IsNullOrWhiteSpace(TextoBusqueda))
        {
            foreach (var d in Destinatarios) 
                DestinatariosFiltrados.Add(d);
            return;
        }

        var busqueda = TextoBusqueda.ToLowerInvariant();
        var filtrados = Destinatarios.Where(d => 
            (d.Nombre != null && d.Nombre.ToLowerInvariant().Contains(busqueda)) || 
            (d.Email != null && d.Email.ToLowerInvariant().Contains(busqueda)) ||
            (d.Consecutivo != null && d.Consecutivo.ToLowerInvariant().Contains(busqueda)));

        foreach (var d in filtrados)
            DestinatariosFiltrados.Add(d);
    }
}
