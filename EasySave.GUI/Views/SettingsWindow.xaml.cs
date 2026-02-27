using System.Windows;
using EasySave.GUI.ViewModels;
using EasySave.Interfaces;

namespace EasySave.GUI.Views
{
    // Settings window code-behind. Creates its own ViewModel and
    // wires up the close/apply events coming from the ViewModel.
    public partial class SettingsWindow : Window
    {
        public SettingsWindow(ISettingsService settingsService)
        {
            InitializeComponent();
            SettingsViewModel viewModel = new SettingsViewModel(settingsService);
            LanguageManager lang = LanguageManager.Instance;

            viewModel.CloseRequested += (sender, saved) =>
            {
                DialogResult = saved;
                Close();
            };

            viewModel.ApplyRequested += (sender, args) =>
            {
                MessageBox.Show(
                    lang.Translate("SettingsSaved"),
                    lang.Translate("Settings"),
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            };

            DataContext = viewModel;
        }
    }
}
