namespace ArquibMailing.Maui.Pages;

using ArquibMailing.Maui.ViewModels;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        // Solo inicializa el diseño del XAML
        InitializeComponent(); 

        //Le decimos tus datos y botones vienen de LoginViewModel
        BindingContext = new LoginViewModel();
    }
}
