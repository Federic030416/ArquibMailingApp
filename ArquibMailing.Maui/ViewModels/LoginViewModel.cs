namespace ArquibMailing.Maui.ViewModels;

using System.Windows.Input;
using System.Text.RegularExpressions;
using Microsoft.Maui.Controls;

public class LoginViewModel : BaseViewModel
{
    //Aca definimos al usuario y a la contraseña como variables privadas
    private string _usuario = string.Empty;
    private string _password = string.Empty;

    private string _errorMessageUsuario = string.Empty;
    private bool _tieneUsuarioError;

    private string _errorMessagePassword = string.Empty;
    private bool _tienePasswordError;

    private bool _ocultarPassword = true;
    private string _iconoOjo = "eye_off.png";


    //Input del usuario
    public string Usuario
    {
        get=> _usuario;
        set
        {
            //Si el usuario vuelve a escribir quitamos el error
            if(SetProperty(ref _usuario, value))
            {
                //Hacemos la limpieza en tiempo real 1
                TieneUsuarioError = false;
                ErrorMessageUsuario = string.Empty;
            }
        }
    }

    //Input de la contraseña 
    public string Password
    {
        get => _password;
        set
        {
            if(SetProperty(ref _password, value))
            {
                TienePasswordError = false;
                ErrorMessagePassword = string.Empty;
            }
        }
    }
    //Aca solo estamos declarando la propiedad
    //Mensaje de error para el usuario
    public string ErrorMessageUsuario
    {
        get => _errorMessageUsuario;
        set => SetProperty(ref _errorMessageUsuario, value);
    }

    //Indica si el campo de usuario tiene error (DataTrigger)
    public bool TieneUsuarioError
    {
        get => _tieneUsuarioError;
        set => SetProperty(ref _tieneUsuarioError, value);
    }

    //Mensaje de Error para la contraseña
    public string ErrorMessagePassword
    {
        get => _errorMessagePassword;
        set => SetProperty(ref _errorMessagePassword, value);
    }

    //Aca se define la propiedad del icono del ojo
    public bool OcultarPassword
    {
        get => _ocultarPassword;
        set => SetProperty(ref _ocultarPassword, value);
    }

    //Controla la imagen del ojo vectorial activa
    public string IconoOjo
    {
        get => _iconoOjo;
        set=> SetProperty(ref _iconoOjo, value);
    }

    //
    

    //Indica si el campo de contraseña tiene error para el DataTrigger
    public bool TienePasswordError
    {
        get => _tienePasswordError;
        set => SetProperty(ref _tienePasswordError, value);
    }

    //Comando del login
    public ICommand IniciarSesionCommand {get;}
    public ICommand OlvidePasswordCommand {get;}
    public ICommand TogglePasswordCommand {get;}

    //Creamos el constructor para inicializar los comandos

    public LoginViewModel()
    {
        IniciarSesionCommand = new Command(async()=>await IniciarSesionAsync());
        OlvidePasswordCommand = new Command(async()=>await OlvidePasswordAsync());
        TogglePasswordCommand = new Command(TogglePassword);
    }
    
    //Metodo para iniciar sesion 

    public async Task IniciarSesionAsync()
    {
        //Borramos los errores viejos 
        TieneUsuarioError = false;
        ErrorMessageUsuario = string.Empty;
        TienePasswordError = false;
        ErrorMessagePassword = string.Empty;
        
        //Validamos si el usuario esta vacio
        if(string.IsNullOrWhiteSpace(Usuario))
        {
            ErrorMessageUsuario = "Por favor, ingresa tu correo elctronico.";
            TieneUsuarioError = true;
            return;
        }

        //Validamos el formato del correo (abajo ponemos la funcion de EsEmailValido por buenas practicas)
        if(!EsEmailValido(Usuario))
        {
            ErrorMessageUsuario ="El formato del correo es incorrecto";
            TieneUsuarioError = true;
            return;
        }

        //Validamos si la contraseña essta vacia 
        if(string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessagePassword ="Por favor ingresa tu contraseña.";
            TienePasswordError = true;
            return;
        }

        //Iniciamos la carga y mostramos el loadaer
        IsBusy = true;

        //Simulamos un retraso de 1.5 para la carga
        await Task.Delay(1500);

        IsBusy = false;

        //Validaciones con credenciales unicas por ahora
        //Despues se conceta a el API
        if(Usuario.ToLower() == "noreply@grupoarquib.com" && Password == "123456")
        {
            
            if(Application.Current != null)
            {
                Application.Current.MainPage = new AppShell();
            }
        }

        else
        {
            //Ponemos el error
            ErrorMessagePassword ="Usuario o contraseña incorrectos.";
            TienePasswordError = true;
            //Limpiamos el campo de contraseña
            _password = string.Empty;
            //Avisamos que hubo un cambio en el campo
            OnPropertyChanged(nameof(Password));
            
        }


    }

    //Metodo para recuperar la contraseña
    public async Task OlvidePasswordAsync()
    {
        if(Application.Current != null)
        {
            await Application.Current.MainPage.DisplayAlert(
            "Recuperacion",
            "Se ha enviado un enlace de recuperacion a tu correo electronico.",
            "Aceptar"
            );
        }
    }
    //Metodo para la visibilidad de la contraseña

    public void TogglePassword()
    {
        OcultarPassword = !OcultarPassword;

        if(OcultarPassword == true)
        {
            IconoOjo = "eye_off.png";
        }
        else
        {
            IconoOjo = "eye.png";
        }
    }

    //Metodo para validar el correo
    private bool EsEmailValido(string email)
    {
        if(string.IsNullOrWhiteSpace(email))
        {
            return false;
        }
        //Aqui usaremos las expresiones regulares que llamamos 
        //en el using namespace de using.System.Text.RegularExpressions;
        try
        {
            return Regex.IsMatch(email,
             @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
             RegexOptions.IgnoreCase,
             TimeSpan.FromMilliseconds(250));
        }
        catch(RegexMatchTimeoutException)
        {
            return false;
        }
    }
}