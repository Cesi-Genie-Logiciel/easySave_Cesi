using System.Windows.Input;
using EasySave.Commands;
using EasySave.Models;

namespace EasySave.ViewModels
{
    /// ViewModel wrapping a BackupJob model for UI display.
    /// Exposes properties and commands for data binding.
    public class BackupJobViewModel : BaseViewModel
    {
        private readonly BackupJob _model;
        private int _progress;
        private string _state;

        public string Name
        {
            get => _model.Name;
            set
            {
                if (_model.Name != value)
                {
                    _model.Name = value;
                    OnPropertyChanged();
                }
            }
        }

        public string SourcePath
        {
            get => _model.SourcePath;
            set
            {
                if (_model.SourcePath != value)
                {
                    _model.SourcePath = value;
                    OnPropertyChanged();
                }
            }
        }

        public string TargetPath
        {
            get => _model.TargetPath;
            set
            {
                if (_model.TargetPath != value)
                {
                    _model.TargetPath = value;
                    OnPropertyChanged();
                }
            }
        }

        public string BackupType
        {
            get => _model.BackupType;
            set
            {
                if (_model.BackupType != value)
                {
                    _model.BackupType = value;
                    OnPropertyChanged();
                }
            }
        }

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

        public ICommand PlayCommand { get; }

        public BackupJob Model => _model;

        public BackupJobViewModel(BackupJob model)
        {
            _model = model;
            _progress = model.Progress;
            _state = model.State ?? "Stopped";
            PlayCommand = new RelayCommand(_ => { }, _ => false);
        }

        public void UpdateProgress(int newProgress)
        {
            Progress = newProgress;
            _model.Progress = newProgress;
        }

        public void UpdateState(string newState)
        {
            State = newState;
            _model.State = newState;
        }
    }
}