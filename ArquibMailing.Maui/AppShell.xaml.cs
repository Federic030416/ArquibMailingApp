namespace ArquibMailing.Maui;

using ArquibMailing.Maui.Pages;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Registrar rutas para navegación con GoToAsync
        Routing.RegisterRoute("Dashboard",    typeof(DashboardPage));
        Routing.RegisterRoute("NuevoEnvio",   typeof(NuevoEnvioPage));
        Routing.RegisterRoute("Destinatarios", typeof(DestinatariosPage));
    }
}

