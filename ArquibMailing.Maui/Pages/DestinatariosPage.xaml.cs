namespace ArquibMailing.Maui.Pages;

using ArquibMailing.Maui.ViewModels;

public partial class DestinatariosPage : ContentPage
{
    private readonly DestinatariosViewModel _viewModel;

    public DestinatariosPage(DestinatariosViewModel viewModel)
    {
        InitializeComponent();
        _viewModel     = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.CargarAsync();
    }
}
