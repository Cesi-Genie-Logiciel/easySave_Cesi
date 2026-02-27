using System;
using System.Windows.Input;
using EasySave.GUI.Commands;
using EasySave.Models;

namespace EasySave.GUI.ViewModels
{
    // Wraps a BackupJob model for display in the DataGrid.
    // Handles play/pause/stop commands and real-time progress tracking.
    // State labels come from LanguageManager so they match the active language.
    public class BackupJobViewModel : BaseViewModel
    {
        private readonly BackupJob _model;
        private int _progress;
        private string _state;
        private int _totalFiles;
        private int _filesRemaining;

        // Shortcut to the language manager
        private LanguageManager Lang => LanguageManager.Instance;

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
            _state = Lang.Translate("Stopped");

            PlayCommand = new RelayCommand(ExecuteJob, CanExecuteJob);
            PauseCommand = new RelayCommand(PauseJob, CanPause);
            StopCommand = new RelayCommand(StopJob, CanStop);

            // Listen to the model events so we can update the UI in real time
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
                State = Lang.Translate("InProgress");
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
                State = Lang.Translate("Completed");
                Progress = 100;
                FilesRemaining = 0;
            });
        }

        // Play is available whenever the job is not actively running.
        // This covers Stopped, Completed, Paused and Error states.
        private bool CanExecuteJob(object? parameter) =>
            State != Lang.Translate("InProgress");

        // Pause is only available when the job is actively running
        private bool CanPause(object? parameter) => State == Lang.Translate("InProgress");

        // Stop is available when the job is running or paused
        private bool CanStop(object? parameter) =>
            State == Lang.Translate("InProgress") || State == Lang.Translate("Paused");

        private void PauseJob(object? parameter)
        {
            _model.Pause();
            State = Lang.Translate("Paused");
        }

        private void StopJob(object? parameter)
        {
            _model.Stop();
            State = Lang.Translate("Stopped");
        }

        private void ExecuteJob(object? parameter)
        {
            if (State == Lang.Translate("InProgress")) return;

            // If the job was paused, resume it instead of starting over.
            // This calls Resume() on the model which reopens the ManualResetEventSlim
            // gate, letting the blocked backup thread continue where it left off.
            if (State == Lang.Translate("Paused"))
            {
                _model.Resume();
                State = Lang.Translate("InProgress");
                return;
            }

            // Otherwise start a fresh execution in a background thread
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
                        State = Lang.Translate("Error") + ": " + ex.Message;
                        Progress = 0;
                    });
                }
            });
        }
    }
}