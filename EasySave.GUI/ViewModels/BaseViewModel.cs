using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EasySave.GUI.ViewModels
{
    /// <summary>
    /// BaseViewModel conforme au diagramme v2.0 (lignes 36-40)
    /// Classe de base pour tous les ViewModels avec support INotifyPropertyChanged
    /// </summary>
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Notifie que la propriété a changé
        /// </summary>
        /// <param name="propertyName">Nom de la propriété (automatique via CallerMemberName)</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Met à jour un champ et notifie si la valeur change
        /// </summary>
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
