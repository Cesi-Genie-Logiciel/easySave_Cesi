using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using EasySave.GUI.Commands;
using EasySave.Interfaces;
using EasySave.Models;

namespace EasySave.GUI.ViewModels
{
    // Orchestrates the main window: job list, toolbar commands, and status bar.
    // All user-facing strings go through LanguageManager so they adapt to the current language.
    public class MainViewModel : BaseViewModel
    {
        private readonly IBackupService _backupService;
        private readonly ISettingsService? _settingsService;
        private BackupJobViewModel? _selectedBackupJob;
        private string _statusText;
        private double _globalProgress;

        // Shortcut to avoid repeating LanguageManager.Instance everywhere
        private LanguageManager Lang => LanguageManager.Instance;

        public ObservableCollection<BackupJobViewModel> BackupJobs { get; }

        // List of available languages shown in the ComboBox
        public ObservableCollection<string> AvailableLanguages { get; }

        // Currently selected language in the ComboBox
        private string _selectedLanguage;
        public string SelectedLanguage
        {
            get => _selectedLanguage;
            set
            {
                if (SetProperty(ref _selectedLanguage, value))
                {
                    // Map the display name back to its culture code
                    string cultureCode = value == "English" ? "en" : "fr";
                    Lang.CurrentLanguage = cultureCode;
                }
            }
        }

        public BackupJobViewModel? SelectedBackupJob
        {
            get => _selectedBackupJob;
            set
            {
                if (SetProperty(ref _selectedBackupJob, value))
                    System.Windows.Input.CommandManager.InvalidateRequerySuggested();
            }
        }

        public string StatusText
        {
            get => _statusText;
            set => SetProperty(ref _statusText, value);
        }

        public double GlobalProgress
        {
            get => _globalProgress;
            set => SetProperty(ref _globalProgress, value);
        }

        public ICommand CreateBackupCommand { get; }
        public ICommand EditBackupCommand { get; }
        public ICommand ExecuteBackupCommand { get; }
        public ICommand DeleteBackupCommand { get; }
        public ICommand ExecuteAllCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand OpenSettingsCommand { get; }

        public MainViewModel(IBackupService backupService, ISettingsService? settingsService = null)
        {
            _backupService = backupService ?? throw new ArgumentNullException(nameof(backupService));
            _settingsService = settingsService;
            BackupJobs = new ObservableCollection<BackupJobViewModel>();

            // Set up the language options
            AvailableLanguages = new ObservableCollection<string> { "Francais", "English" };
            _selectedLanguage = Lang.CurrentLanguage == "en" ? "English" : "Francais";
            _statusText = Lang.Translate("Ready");

            CreateBackupCommand = new RelayCommand(CreateBackupJob);
            EditBackupCommand = new RelayCommand(EditBackupJob, CanEditBackup);
            ExecuteBackupCommand = new RelayCommand(ExecuteBackup, CanExecuteBackup);
            DeleteBackupCommand = new RelayCommand(DeleteBackup, CanDeleteBackup);
            ExecuteAllCommand = new RelayCommand(ExecuteAll);
            RefreshCommand = new RelayCommand(Refresh);
            OpenSettingsCommand = new RelayCommand(OpenSettings);

            LoadJobs();
        }

        private void LoadJobs()
        {
            try
            {
                List<BackupJob> jobs = _backupService.GetAllBackupJobs();
                BackupJobs.Clear();
                foreach (BackupJob job in jobs)
                    BackupJobs.Add(new BackupJobViewModel(job));
                StatusText = jobs.Count + " " + Lang.Translate("JobsLoaded");
            }
            catch (Exception ex)
            {
                StatusText = Lang.Translate("ErrorLoading") + ": " + ex.Message;
            }
        }

        private void CreateBackupJob(object? parameter)
        {
            try
            {
                Views.CreateJobDialog dialog = new Views.CreateJobDialog();
                if (dialog.ShowDialog() == true)
                {
                    _backupService.CreateBackupJob(
                        dialog.JobName,
                        dialog.SourcePath,
                        dialog.TargetPath,
                        dialog.BackupType.ToLower());

                    LoadJobs();
                    StatusText = Lang.Translate("JobCreated") + ": " + dialog.JobName;
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(
                    Lang.Translate("Error") + ": " + ex.Message,
                    Lang.Translate("Error"),
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error);
            }
        }

        private bool CanEditBackup(object? parameter) => SelectedBackupJob != null;

        private void EditBackupJob(object? parameter)
        {
            if (SelectedBackupJob == null) return;
            int index = BackupJobs.IndexOf(SelectedBackupJob);
            if (index < 0) return;
            try
            {
                Views.CreateJobDialog dialog = new Views.CreateJobDialog(
                    SelectedBackupJob.Name,
                    SelectedBackupJob.SourcePath,
                    SelectedBackupJob.TargetPath,
                    SelectedBackupJob.BackupType)
                {
                    Owner = System.Windows.Application.Current.MainWindow
                };
                if (dialog.ShowDialog() == true)
                {
                    _backupService.UpdateBackupJob(
                        index,
                        dialog.JobName,
                        dialog.SourcePath,
                        dialog.TargetPath,
                        dialog.BackupType.ToLower());
                    LoadJobs();
                    StatusText = Lang.Translate("JobModified") + ": " + dialog.JobName;
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(
                    Lang.Translate("Error") + ": " + ex.Message,
                    Lang.Translate("Error"),
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error);
            }
        }

        private void ExecuteBackup(object? parameter)
        {
            if (SelectedBackupJob == null) return;
            int index = BackupJobs.IndexOf(SelectedBackupJob);
            if (index < 0) return;

            StatusText = Lang.Translate("Running") + ": " + SelectedBackupJob.Name;
            System.Threading.Tasks.Task.Run(() =>
            {
                try
                {
                    _backupService.ExecuteBackupJob(index);
                }
                catch (Exception ex)
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        System.Windows.MessageBox.Show(
                            Lang.Translate("Error") + ": " + ex.Message,
                            Lang.Translate("Error"),
                            System.Windows.MessageBoxButton.OK,
                            System.Windows.MessageBoxImage.Error);
                        StatusText = Lang.Translate("ErrorExecution");
                    });
                }
            });
        }

        private bool CanExecuteBackup(object? parameter) => SelectedBackupJob != null;

        private void DeleteBackup(object? parameter)
        {
            if (SelectedBackupJob == null) return;

            string jobName = SelectedBackupJob.Name;
            System.Windows.MessageBoxResult result = System.Windows.MessageBox.Show(
                Lang.Translate("ConfirmDelete") + " (" + jobName + ")",
                Lang.Translate("Confirmation"),
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Question);

            if (result == System.Windows.MessageBoxResult.Yes)
            {
                try
                {
                    int index = BackupJobs.IndexOf(SelectedBackupJob);
                    if (index >= 0)
                    {
                        _backupService.DeleteBackupJob(index);
                        LoadJobs();
                        StatusText = Lang.Translate("JobDeleted") + ": " + jobName;
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(
                        Lang.Translate("Error") + ": " + ex.Message,
                        Lang.Translate("Error"),
                        System.Windows.MessageBoxButton.OK,
                        System.Windows.MessageBoxImage.Error);
                }
            }
        }

        private bool CanDeleteBackup(object? parameter) => SelectedBackupJob != null;

        private void ExecuteAll(object? parameter)
        {
            List<int> indices = Enumerable.Range(0, BackupJobs.Count).ToList();
            StatusText = Lang.Translate("Running") + " " + indices.Count + " job(s)...";

            System.Threading.Tasks.Task.Run(() =>
            {
                try
                {
                    _backupService.ExecuteMultipleBackupJobs(indices);
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        StatusText = Lang.Translate("AllDone");
                    });
                }
                catch (Exception ex)
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        System.Windows.MessageBox.Show(
                            Lang.Translate("Error") + ": " + ex.Message,
                            Lang.Translate("Error"),
                            System.Windows.MessageBoxButton.OK,
                            System.Windows.MessageBoxImage.Error);
                    });
                }
            });
        }

        private void Refresh(object? parameter)
        {
            LoadJobs();
        }

        private void OpenSettings(object? parameter)
        {
            if (_settingsService == null) return;
            Views.SettingsWindow window = new Views.SettingsWindow(_settingsService)
            {
                Owner = System.Windows.Application.Current.MainWindow
            };
            window.ShowDialog();
        }
    }
}