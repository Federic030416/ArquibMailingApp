namespace ArquibMailing.Maui.ViewModels;

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

/// <summary>
/// ViewModel base que implementa INotifyPropertyChanged.
/// Todos los ViewModels heredan de esta clase para obtener
/// la notificación de cambios de propiedades (patrón MVVM).
/// </summary>
public abstract class BaseViewModel : INotifyPropertyChanged
{
    private bool   _isBusy;
    private string _title = string.Empty;
    private string _errorMessage = string.Empty;
    private bool _tieneError;

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Indica si hay una operación en curso (muestra un indicador de carga).
    /// </summary>
    public bool IsBusy
    {
        get => _isBusy;
        set => SetProperty(ref _isBusy, value);
    }

    /// <summary>
    /// Título de la página actual.
    /// </summary>
    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

        /// <summary>
    /// Almacena el mensaje de error para mostrar al usuario.
    /// </summary>
    public string ErrorMessage
    {
        get => _errorMessage;
        set
        {
            SetProperty(ref _errorMessage, value);
            // Si el mensaje no está vacío, TieneError se activa automáticamente
            TieneError = !string.IsNullOrEmpty(value);
        }
    }
    /// <summary>
    /// Indica si la pantalla actual tiene algún error activo.
    /// </summary>
    public bool TieneError
    {
        get => _tieneError;
        set => SetProperty(ref _tieneError, value);
    }

    /// <summary>
    /// Notifica a la vista que una propiedad cambió.
    /// </summary>
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    /// <summary>
    /// Asigna el valor y notifica el cambio solo si es diferente.
    /// </summary>
    protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? name = null)
    {
        if (EqualityComparer<T>.Default.Equals(storage, value))
            return false;

        storage = value;
        OnPropertyChanged(name);
        return true;
    }
}
