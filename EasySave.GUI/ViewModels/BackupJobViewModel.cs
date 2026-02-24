using System;
using System.Windows;
using System.Windows.Input;
using EasySave.GUI.Commands;
using EasySave.Models;

namespace EasySave.GUI.ViewModels
{
    // Wraps a BackupJob for display in the GUI.
    // Exposes Play/Pause/Stop commands and real-time progress through data binding.
    public class BackupJobViewModel : BaseViewModel
    {
        private readonly BackupJob _model;
        private int _progress;
        private BackupJobState _state;

        public string Name => _model.Name;
        public string SourcePath => _model.SourcePath;
        public string TargetPath => _model.TargetPath;
        public string BackupType => _model.BackupType;

        public BackupJobState State
        {
            get => _state;
            set => SetProperty(ref _state, value);
        }

        public int Progress
        {
            get => _progress;
            set => SetProperty(ref _progress, value);
        }

        public bool IsRunning => _state == BackupJobState.Running;
        public bool IsPaused => _state == BackupJobState.Paused;

        public ICommand PlayCommand { get; }
        public ICommand PauseCommand { get; }
        public ICommand StopCommand { get; }

        public BackupJob Model => _model;

        public BackupJobViewModel(BackupJob model)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));
            _state = BackupJobState.Pending;

            PlayCommand = new RelayCommand(ExecutePlay, CanPlay);
            PauseCommand = new RelayCommand(ExecutePause, CanPause);
            StopCommand = new RelayCommand(ExecuteStop, CanStop);

            _model.BackupStarted += OnBackupStarted;
            _model.FileTransferred += OnFileTransferred;
            _model.BackupCompleted += OnBackupCompleted;
            _model.StateChanged += OnStateChanged;
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
            Application.Current.Dispatcher.Invoke(() =>
            {
                Progress = 0;
            });
        }

        private void OnFileTransferred(object? sender, BackupEventArgs args)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Progress = args.Progress;
            });
        }

        private void OnBackupCompleted(object? sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Progress = 100;
            });
        }

        private void OnStateChanged(object? sender, BackupJobState newState)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                State = newState;
                OnPropertyChanged(nameof(IsRunning));
                OnPropertyChanged(nameof(IsPaused));
            });
        }

        private bool CanPlay(object? param)
        {
            return _state == BackupJobState.Pending
                || _state == BackupJobState.Paused
                || _state == BackupJobState.Stopped
                || _state == BackupJobState.Completed
                || _state == BackupJobState.Error;
        }

        private void ExecutePlay(object? param)
        {
            if (_state == BackupJobState.Paused)
            {
                _model.Resume();
            }
            else
            {
                System.Threading.Tasks.Task.Run(() =>
                {
                    try
                    {
                        _model.Execute();
                    }
                    catch (Exception)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            State = BackupJobState.Error;
                        });
                    }
                });
            }
        }

        private bool CanPause(object? param)
        {
            return _state == BackupJobState.Running;
        }

        private void ExecutePause(object? param)
        {
            _model.Pause();
        }

        private bool CanStop(object? param)
        {
            return _state == BackupJobState.Running || _state == BackupJobState.Paused;
        }

        private void ExecuteStop(object? param)
        {
            _model.Stop();
        }
    }
}