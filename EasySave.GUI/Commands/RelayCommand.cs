using System;
using System.Windows.Input;

namespace EasySave.GUI.Commands
{
    /// <summary>
    /// RelayCommand conforme au diagramme v2.0 (lignes 81-88)
    /// Pattern Command pour WPF/MVVM
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        private readonly Func<object?, bool>? _canExecute;

        public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object? parameter)
        {
            return _canExecute?.Invoke(parameter) ?? true;
        }

        public void Execute(object? parameter)
        {
            _execute(parameter);
        }

        /// <summary>
        /// Force la réévaluation de CanExecute
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
