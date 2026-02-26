using System;
using System.Windows;
using Microsoft.Win32;

namespace EasySave.GUI.Views
{
    /// <summary>
    /// Dialogue de création ou modification d'un job de backup (conforme schéma UpdateBackupJob).
    /// </summary>
    public partial class CreateJobDialog : Window
    {
        public string JobName => JobNameTextBox.Text;
        public string SourcePath => SourcePathTextBox.Text;
        public string TargetPath => TargetPathTextBox.Text;
        public string BackupType => BackupTypeComboBox.SelectedIndex == 0 ? "complete" : "differential";

        public bool IsEditMode { get; }

        public CreateJobDialog()
        {
            InitializeComponent();
            IsEditMode = false;
        }

        /// <summary>
        /// Mode édition : pré-remplit les champs avec les données du job existant.
        /// </summary>
        public CreateJobDialog(string jobName, string sourcePath, string targetPath, string backupType)
        {
            InitializeComponent();
            IsEditMode = true;
            Title = "Modifier le Job de Sauvegarde";
            JobNameTextBox.Text = jobName ?? "";
            SourcePathTextBox.Text = sourcePath ?? "";
            TargetPathTextBox.Text = targetPath ?? "";
            BackupTypeComboBox.SelectedIndex = string.Equals(backupType, "differential", StringComparison.OrdinalIgnoreCase) ? 1 : 0;
            ConfirmButton.Content = "✅ Enregistrer";
        }

        private void BrowseSource_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFolderDialog
            {
                Title = "Sélectionnez le dossier source"
            };

            if (dialog.ShowDialog() == true)
            {
                SourcePathTextBox.Text = dialog.FolderName;
            }
        }

        private void BrowseTarget_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFolderDialog
            {
                Title = "Sélectionnez le dossier cible"
            };

            if (dialog.ShowDialog() == true)
            {
                TargetPathTextBox.Text = dialog.FolderName;
            }
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(JobName))
            {
                System.Windows.MessageBox.Show("Le nom du job est requis.", "Validation", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(SourcePath))
            {
                System.Windows.MessageBox.Show("Le chemin source est requis.", "Validation", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(TargetPath))
            {
                System.Windows.MessageBox.Show("Le chemin cible est requis.", "Validation", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
