using System;
using System.Windows.Input;
using EasySave.GUI.Commands;
using EasySave.Models;

namespace EasySave.GUI.ViewModels
{
    public class BackupJobViewModel : BaseViewModel
    {
        private readonly BackupJob _model;
        private int _progress;
        private string _state = "Arrete";
        private int _totalFiles;
        private int _filesRemaining;

        public string Name => _model.Name;
        public string SourcePath => _model.SourcePath;
        public string TargetPath => _model.TargetPath;
        public string BackupType => _model.BackupType;

        public int Progress
        {
            get => _progress;
            set => SetProperty(ref _progress, value);
        }

        public string State
        {
            get => _state;
            set => SetProperty(ref _state, value);
        }

        public int TotalFiles
        {
            get => _totalFiles;
            set => SetProperty(ref _totalFiles, value);
        }

        public int FilesRemaining
        {
            get => _filesRemaining;
            set => SetProperty(ref _filesRemaining, value);
        }

        public ICommand PlayCommand { get; }
        public ICommand PauseCommand { get; }
        public ICommand StopCommand { get; }

        public BackupJobViewModel(BackupJob model)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));
            PlayCommand = new RelayCommand(ExecuteJob, CanExecuteJob);
            PauseCommand = new RelayCommand(PauseJob, CanPauseOrStop);
            StopCommand = new RelayCommand(StopJob, CanPauseOrStop);

            _model.BackupStarted += OnBackupStarted;
            _model.FileTransferred += OnFileTransferred;
            _model.BackupCompleted += OnBackupCompleted;
        }

        public void UpdateFromModel(BackupJob backupJob)
        {
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(SourcePath));
            OnPropertyChanged(nameof(TargetPath));
            OnPropertyChanged(nameof(BackupType));
        }

        private void OnBackupStarted(object? sender, EventArgs e)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                State = "En cours";
                Progress = 0;
            });
        }

        private void OnFileTransferred(object? sender, BackupEventArgs e)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                Progress = e.Progress;
                TotalFiles = e.TotalFiles;
                FilesRemaining = e.TotalFiles - e.ProcessedFiles;
            });
        }

        private void OnBackupCompleted(object? sender, EventArgs e)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                State = "Termine";
                Progress = 100;
                FilesRemaining = 0;
            });
        }

        private bool CanExecuteJob(object? parameter) => State != "En cours";

        private bool CanPauseOrStop(object? parameter) => State == "En cours";

        private void PauseJob(object? parameter)
        {
            _model.Pause();
        }

        private void StopJob(object? parameter)
        {
            _model.Stop();
        }

        private void ExecuteJob(object? parameter)
        {
            if (State == "En cours") return;

            System.Threading.Tasks.Task.Run(() =>
            {
                try
                {
                    _model.Execute();
                }
                catch (Exception ex)
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        State = $"Erreur: {ex.Message}";
                        Progress = 0;
                    });
                }
            });
        }
    }
}