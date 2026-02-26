using System.Windows;
using EasySave.GUI.ViewModels;
using EasySave.Interfaces;

namespace EasySave.GUI.Views
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow(ISettingsService settingsService)
        {
            InitializeComponent();
            var vm = new SettingsViewModel(settingsService);
            vm.CloseRequested += (_, saved) =>
            {
                DialogResult = saved;
                Close();
            };
            vm.ApplyRequested += (_, _) =>
            {
                System.Windows.MessageBox.Show("Paramètres appliqués.", "Paramètres", MessageBoxButton.OK, MessageBoxImage.Information);
            };
            DataContext = vm;
        }
    }
}
