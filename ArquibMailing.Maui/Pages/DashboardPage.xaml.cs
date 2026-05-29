namespace ArquibMailing.Maui.Pages;

using ArquibMailing.Maui.ViewModels;

/// <summary>
/// Code-behind del Dashboard principal.
/// Carga el resumen al entrar y navega a las otras secciones.
/// </summary>
public partial class DashboardPage : ContentPage
{
    private readonly DashboardViewModel _viewModel;

    public DashboardPage(DashboardViewModel viewModel)
    {
        InitializeComponent();
        _viewModel     = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.CargarResumenAsync();
    }

    /// <summary>Navega a la pantalla de Nuevo Envío.</summary>
    private async void OnNuevoEnvioTapped(object sender, TappedEventArgs e)
        => await Shell.Current.GoToAsync("//NuevoEnvio");

    /// <summary>Navega a la pantalla de Destinatarios.</summary>
    private async void OnDestinatariosTapped(object sender, TappedEventArgs e)
        => await Shell.Current.GoToAsync("//Destinatarios");
}
