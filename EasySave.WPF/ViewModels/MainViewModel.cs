using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using EasySave.Commands;
using EasySave.Models;
using EasySave.Services.Interfaces;

namespace EasySave.ViewModels
{
    /// Main ViewModel orchestrating the entire application.
    /// Manages job list, execution, and global state.
    public class MainViewModel : BaseViewModel
    {
        private readonly IBackupService _backupService;
        private BackupJobViewModel? _selectedBackupJob;
        private double _globalProgress;
        private string _statusText;
        private bool _isExecuting;

        public ObservableCollection<BackupJobViewModel> BackupJobs { get; }

        public BackupJobViewModel? SelectedBackupJob
        {
            get => _selectedBackupJob;
            set => SetProperty(ref _selectedBackupJob, value);
        }

        public double GlobalProgress
        {
            get => _globalProgress;
            set => SetProperty(ref _globalProgress, value);
        }

        public string StatusText
        {
            get => _statusText;
            set => SetProperty(ref _statusText, value);
        }

        public bool IsExecuting
        {
            get => _isExecuting;
            set => SetProperty(ref _isExecuting, value);
        }

        public ICommand CreateBackupCommand { get; }
        public ICommand ExecuteBackupCommand { get; }
        public ICommand ExecuteAllCommand { get; }

        public MainViewModel(IBackupService backupService)
        {
            _backupService = backupService ?? throw new ArgumentNullException(nameof(backupService));

            BackupJobs = new ObservableCollection<BackupJobViewModel>();
            _statusText = "Ready";
            GlobalProgress = 0;

            CreateBackupCommand = new RelayCommand(CreateBackup);
            ExecuteBackupCommand = new AsyncRelayCommand(ExecuteBackupAsync, CanExecuteBackup);
            ExecuteAllCommand = new AsyncRelayCommand(ExecuteAllAsync, _ => !IsExecuting && BackupJobs.Any());

            LoadMockJobs();
        }

        private void CreateBackup(object? parameter)
        {
            int jobNumber = BackupJobs.Count + 1;
            var newJob = new BackupJob
            {
                Name = $"Job {jobNumber}",
                SourcePath = $@"C:\Source{jobNumber}",
                TargetPath = $@"D:\Backup{jobNumber}",
                BackupType = "Complete",
                State = "Stopped",
                Progress = 0
            };

            BackupJobs.Add(new BackupJobViewModel(newJob));
            StatusText = $"New job '{newJob.Name}' created";
        }

        private async Task ExecuteBackupAsync(object? parameter)
        {
            if (SelectedBackupJob == null)
                return;

            IsExecuting = true;
            SelectedBackupJob.UpdateState("Running");
            SelectedBackupJob.UpdateProgress(0);
            StatusText = $"Executing: {SelectedBackupJob.Name}...";

            try
            {
                await _backupService.ExecuteBackupAsync(
                    SelectedBackupJob.Model,
                    progress =>
                    {
                        SelectedBackupJob.UpdateProgress(progress);
                        GlobalProgress = progress;
                    }
                );

                SelectedBackupJob.UpdateState("Completed");
                StatusText = $"Job '{SelectedBackupJob.Name}' completed successfully";
            }
            catch (Exception ex)
            {
                SelectedBackupJob.UpdateState("Error");
                StatusText = $"Error during execution: {ex.Message}";
            }
            finally
            {
                IsExecuting = false;
                GlobalProgress = 0;
            }
        }

        private bool CanExecuteBackup(object? parameter)
        {
            return !IsExecuting && SelectedBackupJob != null;
        }

        private async Task ExecuteAllAsync(object? parameter)
        {
            IsExecuting = true;
            StatusText = "Executing all jobs...";

            int completedJobs = 0;
            int totalJobs = BackupJobs.Count;

            foreach (var jobViewModel in BackupJobs)
            {
                jobViewModel.UpdateState("Running");
                jobViewModel.UpdateProgress(0);

                try
                {
                    await _backupService.ExecuteBackupAsync(
                        jobViewModel.Model,
                        progress =>
                        {
                            jobViewModel.UpdateProgress(progress);
                            double jobWeight = 100.0 / totalJobs;
                            double currentJobProgress = (progress / 100.0) * jobWeight;
                            GlobalProgress = (completedJobs * jobWeight) + currentJobProgress;
                        }
                    );

                    jobViewModel.UpdateState("Completed");
                    completedJobs++;
                }
                catch (Exception ex)
                {
                    jobViewModel.UpdateState("Error");
                    StatusText = $"Error on job '{jobViewModel.Name}': {ex.Message}";
                }
            }

            IsExecuting = false;
            GlobalProgress = 100;
            StatusText = $"All jobs completed ({completedJobs}/{totalJobs} successful)";

            await Task.Delay(2000);
            GlobalProgress = 0;
        }

        private void LoadMockJobs()
        {
            BackupJobs.Add(new BackupJobViewModel(new BackupJob
            {
                Name = "Personal Documents",
                SourcePath = @"C:\Users\Documents",
                TargetPath = @"D:\Backup\Documents",
                BackupType = "Complete",
                State = "Stopped",
                Progress = 0
            }));

            BackupJobs.Add(new BackupJobViewModel(new BackupJob
            {
                Name = "Holiday Photos",
                SourcePath = @"C:\Users\Pictures",
                TargetPath = @"D:\Backup\Pictures",
                BackupType = "Differential",
                State = "Stopped",
                Progress = 0
            }));

            BackupJobs.Add(new BackupJobViewModel(new BackupJob
            {
                Name = "CESI Project",
                SourcePath = @"C:\Dev\CESI",
                TargetPath = @"D:\Backup\CESI",
                BackupType = "Complete",
                State = "Stopped",
                Progress = 0
            }));
        }
    }
}