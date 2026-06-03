namespace ArquibMailing.Maui.Pages;

using ArquibMailing.Maui.ViewModels;

/// <summary>
/// Code-behind de NuevoEnvioPage.
/// Conecta la vista con su ViewModel vía inyección de dependencias.
/// </summary>
public partial class NuevoEnvioPage : ContentPage
{
    private readonly NuevoEnvioViewModel _viewModel;

    public NuevoEnvioPage(NuevoEnvioViewModel viewModel)
    {
        InitializeComponent();
        _viewModel  = viewModel;
        BindingContext = _viewModel;

#if WINDOWS
        // Oculta los checkboxes nativos de Windows que se ven oscuros y descuadran el diseño
        Microsoft.Maui.Controls.Handlers.Items.CollectionViewHandler.Mapper.AppendToMapping("HideWinUICheckbox", (handler, view) =>
        {
            if (handler.PlatformView is Microsoft.UI.Xaml.Controls.ListViewBase listViewBase)
            {
                listViewBase.IsMultiSelectCheckBoxEnabled = false;
            }
        });
#endif
    }

    /// <summary>
    /// Se ejecuta cada vez que la página aparece en pantalla.
    /// Carga los destinatarios si aún no se han cargado.
    /// </summary>
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.CargarDestinatariosAsync();
    }

    /// <summary>
    /// Sincroniza la lista de destinatarios seleccionados de la interfaz con el ViewModel.
    /// </summary>
    private void OnCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is CollectionView collectionView)
        {
            _viewModel.DestinatariosSeleccionados.Clear();
            foreach (var item in collectionView.SelectedItems)
            {
                if (item is ArquibMailing.Domain.Entities.Destinatario destinatario)
                {
                    _viewModel.DestinatariosSeleccionados.Add(destinatario);
                }
            }
            _viewModel.NotificarSeleccionCambiada();
        }
    }
}
