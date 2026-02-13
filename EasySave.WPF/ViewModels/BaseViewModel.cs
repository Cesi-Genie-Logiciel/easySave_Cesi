using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EasySave.ViewModels
{
    /// Base class for all ViewModels in the application.
    /// Implements INotifyPropertyChanged for MVVM data binding.
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        /// Raises the PropertyChanged event to notify the UI of property changes.
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        /// Updates a property and raises PropertyChanged if the value has changed.
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}