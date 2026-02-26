using Microsoft.Win32;
using System;
using System.Windows;

namespace EasySave.GUI.Views
{
    // Dialog for creating or editing a backup job.
    // In edit mode, fields are pre-filled and the button text changes to "Save".
    public partial class CreateJobDialog : Window
    {
        public string JobName => JobNameTextBox.Text;
        public string SourcePath => SourcePathTextBox.Text;
        public string TargetPath => TargetPathTextBox.Text;
        public string BackupType => BackupTypeComboBox.SelectedIndex == 0 ? "complete" : "differential";

        public bool IsEditMode { get; }

        // Shortcut to the language manager
        private LanguageManager Lang => LanguageManager.Instance;

        public CreateJobDialog()
        {
            InitializeComponent();
            IsEditMode = false;
        }

        // Edit mode: pre-fills the form with the existing job data
        public CreateJobDialog(string jobName, string sourcePath, string targetPath, string backupType)
        {
            InitializeComponent();
            IsEditMode = true;
            Title = Lang.Translate("EditJobTitle");
            JobNameTextBox.Text = jobName ?? "";
            SourcePathTextBox.Text = sourcePath ?? "";
            TargetPathTextBox.Text = targetPath ?? "";
            BackupTypeComboBox.SelectedIndex = string.Equals(backupType, "differential", StringComparison.OrdinalIgnoreCase) ? 1 : 0;
            ConfirmButton.Content = Lang.Translate("Save");
        }

        private void BrowseSource_Click(object sender, RoutedEventArgs e)
        {
            OpenFolderDialog dialog = new OpenFolderDialog
            {
                Title = Lang.Translate("SelectSourceFolder")
            };

            if (dialog.ShowDialog() == true)
            {
                SourcePathTextBox.Text = dialog.FolderName;
            }
        }

        private void BrowseTarget_Click(object sender, RoutedEventArgs e)
        {
            OpenFolderDialog dialog = new OpenFolderDialog
            {
                Title = Lang.Translate("SelectTargetFolder")
            };

            if (dialog.ShowDialog() == true)
            {
                TargetPathTextBox.Text = dialog.FolderName;
            }
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(JobName))
            {
                MessageBox.Show(Lang.Translate("ValidationName"), Lang.Translate("Validation"),
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(SourcePath))
            {
                MessageBox.Show(Lang.Translate("ValidationSource"), Lang.Translate("Validation"),
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(TargetPath))
            {
                MessageBox.Show(Lang.Translate("ValidationTarget"), Lang.Translate("Validation"),
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
