namespace ArquibMailing.Maui.ViewModels;

using ArquibMailing.Application.UseCases;
using ArquibMailing.Domain.Entities;
using System.Collections.ObjectModel;
using System.Windows.Input;

/// <summary>
/// ViewModel para la lista de destinatarios con búsqueda en tiempo real.
/// </summary>
public class DestinatariosViewModel : BaseViewModel
{
    private readonly ObtenerDestinatariosUseCase _useCase;
    private string _criterioBusqueda = string.Empty;

    public DestinatariosViewModel(ObtenerDestinatariosUseCase useCase)
    {
        Title    = "Destinatarios";
        _useCase = useCase;

        Destinatarios  = new ObservableCollection<Destinatario>();
        BuscarCommand  = new Command(async () => await BuscarAsync());
        RefrescarCommand = new Command(async () => await CargarAsync());
    }

    public ObservableCollection<Destinatario> Destinatarios { get; }

    public string CriterioBusqueda
    {
        get => _criterioBusqueda;
        set
        {
            SetProperty(ref _criterioBusqueda, value);
            // Búsqueda reactiva: se ejecuta al escribir
            ((Command)BuscarCommand).Execute(null);
        }
    }

    public ICommand BuscarCommand   { get; }
    public ICommand RefrescarCommand { get; }

    public async Task CargarAsync()
    {
        IsBusy = true;
        try
        {
            var lista = await _useCase.ObtenerTodosAsync();
            Destinatarios.Clear();
            foreach (var d in lista)
                Destinatarios.Add(d);
        }
        finally { IsBusy = false; }
    }

    private async Task BuscarAsync()
    {
        IsBusy = true;
        try
        {
            var lista = await _useCase.BuscarAsync(CriterioBusqueda);
            Destinatarios.Clear();
            foreach (var d in lista)
                Destinatarios.Add(d);
        }
        finally { IsBusy = false; }
    }
}
