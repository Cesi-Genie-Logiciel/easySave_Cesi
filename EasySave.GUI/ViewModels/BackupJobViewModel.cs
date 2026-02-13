using System;
using System.Windows;
using System.Windows.Input;
using EasySave.GUI.Commands;
using EasySave.Models;

namespace EasySave.GUI.ViewModels
{
    /// <summary>
    /// BackupJobViewModel conforme au diagramme v2.0 (lignes 55-69)
    /// ViewModel pour un job de backup individuel avec binding vers la GUI
    /// </summary>
    public class BackupJobViewModel : BaseViewModel
    {
        private readonly BackupJob _model;
        private int _progress;
        private string _state = "Arrêté";
        private int _totalFiles;
        private int _filesRemaining;

        // Properties conformes au diagramme
        public string Name => _model.Name;
        public string SourcePath => _model.SourcePath;
        public string TargetPath => _model.TargetPath;
        
        public string BackupType
        {
            get
            {
                // Récupérer le type depuis la stratégie
                var strategyType = _model.GetType().GetProperty("_strategy", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?
                    .GetValue(_model)?.GetType().Name ?? "Complete";
                
                return strategyType.Contains("Differential") ? "Différentielle" : "Complète";
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

        // Commands conformes au diagramme
        public ICommand PlayCommand { get; }
        public ICommand PauseCommand { get; }
        public ICommand StopCommand { get; }

        public BackupJobViewModel(BackupJob model)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));

            // Initialiser les commandes
            PlayCommand = new RelayCommand(Execute, CanExecute);
            PauseCommand = new RelayCommand(Pause, CanPause);
            StopCommand = new RelayCommand(Stop, CanStop);

            // S'abonner aux events du model (P2 events)
            _model.BackupStarted += OnBackupStarted;
            _model.FileTransferred += OnFileTransferred;
            _model.BackupCompleted += OnBackupCompleted;
        }

        /// <summary>
        /// Met à jour le ViewModel depuis le modèle (conforme diagramme ligne 68)
        /// </summary>
        public void UpdateFromModel(BackupJob backupJob)
        {
            // Mettre à jour toutes les propriétés
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(SourcePath));
            OnPropertyChanged(nameof(TargetPath));
            OnPropertyChanged(nameof(BackupType));
        }

        // Event handlers pour P2 events
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
                State = "Terminé";
                Progress = 100;
                FilesRemaining = 0;
            });
        }

        // Command methods
        private bool CanExecute(object? parameter) => State != "En cours" && State != "En pause";

        private void Execute(object? parameter)
        {
            if (State == "En cours")
                return;
                
            // Lancer l'exécution en async pour ne pas bloquer l'UI
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

        private bool CanPause(object? parameter) => State == "En cours";

        private void Pause(object? parameter)
        {
            _model.Pause();
            State = "En pause";
        }

        private bool CanStop(object? parameter) => State == "En cours" || State == "En pause";

        private void Stop(object? parameter)
        {
            _model.Stop();
            State = "Arrêté";
        }
    }
}
