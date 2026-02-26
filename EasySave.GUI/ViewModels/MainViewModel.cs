using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using EasySave.GUI.Commands;
using EasySave.Interfaces;
using EasySave.Models;

namespace EasySave.GUI.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly IBackupService _backupService;
        private BackupJobViewModel? _selectedBackupJob;
        private string _statusText = "Pret";
        private double _globalProgress;

        public ObservableCollection<BackupJobViewModel> BackupJobs { get; }

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
        public ICommand EditBackupCommand { get; } = null!;
        public ICommand ExecuteBackupCommand { get; }
        public ICommand DeleteBackupCommand { get; }
        public ICommand ExecuteAllCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand OpenSettingsCommand { get; }

        private readonly ISettingsService? _settingsService;

        public MainViewModel(IBackupService backupService, ISettingsService? settingsService = null)
        {
            _backupService = backupService ?? throw new ArgumentNullException(nameof(backupService));
            _settingsService = settingsService;
            BackupJobs = new ObservableCollection<BackupJobViewModel>();

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
                var jobs = _backupService.GetAllBackupJobs();
                // Rebuild list so ViewModels always wrap the current job objects (important after Edit).
                BackupJobs.Clear();
                foreach (var job in jobs)
                    BackupJobs.Add(new BackupJobViewModel(job));
                StatusText = $"{jobs.Count} job(s) charge(s)";
            }
            catch (Exception ex)
            {
                StatusText = $"Erreur chargement: {ex.Message}";
            }
        }

        private void CreateBackupJob(object? parameter)
        {
            try
            {
                var dialog = new Views.CreateJobDialog();
                if (dialog.ShowDialog() == true)
                {
                    _backupService.CreateBackupJob(
                        dialog.JobName,
                        dialog.SourcePath,
                        dialog.TargetPath,
                        dialog.BackupType.ToLower());

                    LoadJobs();
                    StatusText = $"Job '{dialog.JobName}' cree";
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Erreur : {ex.Message}", "Erreur",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private bool CanEditBackup(object? parameter) => SelectedBackupJob != null;

        private void EditBackupJob(object? parameter)
        {
            if (SelectedBackupJob == null) return;
            var index = BackupJobs.IndexOf(SelectedBackupJob);
            if (index < 0) return;
            try
            {
                var dialog = new Views.CreateJobDialog(
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
                    StatusText = $"Job '{dialog.JobName}' modifie";
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Erreur : {ex.Message}", "Erreur",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private void ExecuteBackup(object? parameter)
        {
            if (SelectedBackupJob == null) return;

            var index = BackupJobs.IndexOf(SelectedBackupJob);
            if (index < 0) return;

            StatusText = $"Execution : {SelectedBackupJob.Name}";
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
                        System.Windows.MessageBox.Show($"Erreur : {ex.Message}", "Erreur",
                            System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                        StatusText = "Erreur execution";
                    });
                }
            });
        }

        private bool CanExecuteBackup(object? parameter) => SelectedBackupJob != null;

        private void DeleteBackup(object? parameter)
        {
            if (SelectedBackupJob == null) return;

            var jobName = SelectedBackupJob.Name;
            var result = System.Windows.MessageBox.Show(
                $"Supprimer '{jobName}' ?", "Confirmation",
                System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question);

            if (result == System.Windows.MessageBoxResult.Yes)
            {
                try
                {
                    var index = BackupJobs.IndexOf(SelectedBackupJob);
                    if (index >= 0)
                    {
                        _backupService.DeleteBackupJob(index);
                        LoadJobs();
                        StatusText = $"Job supprime : {jobName}";
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"Erreur : {ex.Message}", "Erreur",
                        System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                }
            }
        }

        private bool CanDeleteBackup(object? parameter) => SelectedBackupJob != null;

        private void ExecuteAll(object? parameter)
        {
            var indices = Enumerable.Range(0, BackupJobs.Count).ToList();
            StatusText = $"Execution de {indices.Count} job(s)...";

            System.Threading.Tasks.Task.Run(() =>
            {
                try
                {
                    _backupService.ExecuteMultipleBackupJobs(indices);
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        StatusText = $"Execution de {indices.Count} job(s) terminee";
                    });
                }
                catch (Exception ex)
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        System.Windows.MessageBox.Show($"Erreur : {ex.Message}", "Erreur",
                            System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
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
            var window = new Views.SettingsWindow(_settingsService)
            {
                Owner = System.Windows.Application.Current.MainWindow
            };
            window.ShowDialog();
        }
    }
}