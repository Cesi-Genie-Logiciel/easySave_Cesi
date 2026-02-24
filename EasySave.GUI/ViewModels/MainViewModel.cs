using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
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
        private string _statusText = "Ready";
        private double _globalProgress;

        public ObservableCollection<BackupJobViewModel> BackupJobs { get; }

        public BackupJobViewModel? SelectedBackupJob
        {
            get => _selectedBackupJob;
            set => SetProperty(ref _selectedBackupJob, value);
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
        public ICommand ExecuteBackupCommand { get; }
        public ICommand DeleteBackupCommand { get; }
        public ICommand ExecuteAllCommand { get; }
        public ICommand PauseAllCommand { get; }
        public ICommand ResumeAllCommand { get; }
        public ICommand StopAllCommand { get; }
        public ICommand RefreshCommand { get; }

        public MainViewModel(IBackupService backupService)
        {
            _backupService = backupService ?? throw new ArgumentNullException(nameof(backupService));
            BackupJobs = new ObservableCollection<BackupJobViewModel>();

            CreateBackupCommand = new RelayCommand(CreateBackupJob);
            ExecuteBackupCommand = new RelayCommand(ExecuteBackup, CanExecuteBackup);
            DeleteBackupCommand = new RelayCommand(DeleteBackup, CanDeleteBackup);
            ExecuteAllCommand = new RelayCommand(ExecuteAll);
            PauseAllCommand = new RelayCommand(PauseAll);
            ResumeAllCommand = new RelayCommand(ResumeAll);
            StopAllCommand = new RelayCommand(StopAll);
            RefreshCommand = new RelayCommand(Refresh);

            LoadJobs();
        }

        private void LoadJobs()
        {
            try
            {
                System.Collections.Generic.List<BackupJob> jobs = _backupService.GetAllBackupJobs();

                for (int i = BackupJobs.Count - 1; i >= 0; i--)
                {
                    if (!jobs.Any(j => j.Name == BackupJobs[i].Name))
                        BackupJobs.RemoveAt(i);
                }

                foreach (BackupJob job in jobs)
                {
                    if (!BackupJobs.Any(vm => vm.Name == job.Name))
                        BackupJobs.Add(new BackupJobViewModel(job));
                }

                StatusText = $"{jobs.Count} job(s) loaded";
            }
            catch (Exception ex)
            {
                StatusText = $"Load error: {ex.Message}";
            }
        }

        private void CreateBackupJob(object? param)
        {
            try
            {
                Views.CreateJobDialog dialog = new Views.CreateJobDialog();
                if (dialog.ShowDialog() == true)
                {
                    _backupService.CreateBackupJob(
                        dialog.JobName, dialog.SourcePath,
                        dialog.TargetPath, dialog.BackupType.ToLower());
                    LoadJobs();
                    StatusText = $"Job '{dialog.JobName}' created";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteBackup(object? param)
        {
            if (SelectedBackupJob == null) return;
            StatusText = $"Executing: {SelectedBackupJob.Name}";
            SelectedBackupJob.PlayCommand.Execute(null);
        }

        private bool CanExecuteBackup(object? param) => SelectedBackupJob != null;

        private void DeleteBackup(object? param)
        {
            if (SelectedBackupJob == null) return;

            string jobName = SelectedBackupJob.Name;
            MessageBoxResult result = MessageBox.Show(
                $"Delete '{jobName}'?", "Confirmation",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    int index = BackupJobs.IndexOf(SelectedBackupJob);
                    if (index >= 0)
                    {
                        _backupService.DeleteBackupJob(index);
                        LoadJobs();
                        StatusText = $"Job deleted: {jobName}";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private bool CanDeleteBackup(object? param) => SelectedBackupJob != null;

        private void ExecuteAll(object? param)
        {
            StatusText = $"Executing {BackupJobs.Count} job(s) in parallel...";
            System.Threading.Tasks.Task.Run(async () =>
            {
                try
                {
                    await _backupService.ExecuteAllBackupJobsParallel();
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        StatusText = $"All {BackupJobs.Count} job(s) completed";
                    });
                }
                catch (Exception ex)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        StatusText = $"Error: {ex.Message}";
                    });
                }
            });
        }

        private void PauseAll(object? param)
        {
            _backupService.PauseAllBackupJobs();
            StatusText = "All jobs paused";
        }

        private void ResumeAll(object? param)
        {
            _backupService.ResumeAllBackupJobs();
            StatusText = "All jobs resumed";
        }

        private void StopAll(object? param)
        {
            _backupService.StopAllBackupJobs();
            StatusText = "All jobs stopped";
        }

        private void Refresh(object? param)
        {
            LoadJobs();
        }
    }
}