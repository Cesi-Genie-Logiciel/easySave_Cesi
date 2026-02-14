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
    /// <summary>
    /// MainViewModel conforme au diagramme v2.0 (lignes 42-53)
    /// ViewModel principal de l'application avec gestion des jobs
    /// </summary>
    public class MainViewModel : BaseViewModel
    {
        private readonly IBackupService _backupService;
        private BackupJobViewModel? _selectedBackupJob;
        private string _statusText = "Prêt";
        private double _globalProgress;

        // Properties conformes au diagramme
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

        // Commands conformes au diagramme
        public ICommand CreateBackupCommand { get; }
        public ICommand ExecuteBackupCommand { get; }
        public ICommand DeleteBackupCommand { get; }
        
        // Bonus: Commands additionnels pour meilleure UX
        public ICommand ExecuteAllCommand { get; }
        public ICommand RefreshCommand { get; }

        public MainViewModel(IBackupService backupService)
        {
            _backupService = backupService ?? throw new ArgumentNullException(nameof(backupService));
            BackupJobs = new ObservableCollection<BackupJobViewModel>();

            // Initialiser les commandes
            CreateBackupCommand = new RelayCommand(CreateBackupJob);
            ExecuteBackupCommand = new RelayCommand(ExecuteBackup, CanExecuteBackup);
            DeleteBackupCommand = new RelayCommand(DeleteBackup, CanDeleteBackup);
            ExecuteAllCommand = new RelayCommand(ExecuteAll);
            RefreshCommand = new RelayCommand(Refresh);

            // S'abonner aux events du service (P2 events)
            _backupService.JobCreated += OnJobCreated;
            _backupService.JobDeleted += OnJobDeleted;
            _backupService.JobUpdated += OnJobUpdated;

            // Charger les jobs existants (depuis persistance P2)
            LoadJobs();
        }

        /// <summary>
        /// Charge tous les jobs depuis le BackupService (P2)
        /// </summary>
        private void LoadJobs()
        {
            try
            {
                var jobs = _backupService.GetAllBackupJobs();
                
                // Supprimer les ViewModels qui n'existent plus dans le service
                for (int i = BackupJobs.Count - 1; i >= 0; i--)
                {
                    if (!jobs.Any(j => j.Name == BackupJobs[i].Name))
                    {
                        BackupJobs.RemoveAt(i);
                    }
                }
                
                // Ajouter les nouveaux jobs qui n'ont pas encore de ViewModel
                foreach (var job in jobs)
                {
                    if (!BackupJobs.Any(vm => vm.Name == job.Name))
                    {
                        BackupJobs.Add(new BackupJobViewModel(job));
                    }
                }

                StatusText = $"{jobs.Count} job(s) chargé(s)";
            }
            catch (Exception ex)
            {
                StatusText = $"Erreur chargement: {ex.Message}";
            }
        }

        /// <summary>
        /// Crée un nouveau job de backup (conforme diagramme ligne 50)
        /// </summary>
        private void CreateBackupJob(object? parameter)
        {
            try
            {
                // TODO: Créer une fenêtre de dialogue pour saisir les infos
                // Pour l'instant, valeurs de test
                var dialog = new Views.CreateJobDialog();
                if (dialog.ShowDialog() == true)
                {
                    _backupService.CreateBackupJob(
                        dialog.JobName,
                        dialog.SourcePath,
                        dialog.TargetPath,
                        dialog.BackupType.ToLower()
                    );
                    
                    StatusText = $"Job '{dialog.JobName}' créé";
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Erreur lors de la création: {ex.Message}", 
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                StatusText = "Erreur création job";
            }
        }

        /// <summary>
        /// Exécute un backup sélectionné (conforme diagramme ligne 51)
        /// </summary>
        private void ExecuteBackup(object? parameter)
        {
            if (SelectedBackupJob == null)
                return;

            try
            {
                var index = BackupJobs.IndexOf(SelectedBackupJob);
                if (index >= 0)
                {
                    StatusText = $"Exécution: {SelectedBackupJob.Name}";
                    
                    // Exécuter de façon asynchrone pour ne pas bloquer l'UI
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
                                System.Windows.MessageBox.Show($"Erreur lors de l'exécution: {ex.Message}", 
                                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                                StatusText = "Erreur exécution";
                            });
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Erreur lors de l'exécution: {ex.Message}", 
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                StatusText = "Erreur exécution";
            }
        }

        private bool CanExecuteBackup(object? parameter) => SelectedBackupJob != null;

        /// <summary>
        /// Supprime un backup (conforme diagramme ligne 52)
        /// </summary>
        private void DeleteBackup(object? parameter)
        {
            if (SelectedBackupJob == null)
                return;

            // Sauvegarder le nom avant suppression
            var jobName = SelectedBackupJob.Name;
            
            var result = System.Windows.MessageBox.Show(
                $"Êtes-vous sûr de vouloir supprimer '{jobName}' ?",
                "Confirmation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var index = BackupJobs.IndexOf(SelectedBackupJob);
                    if (index >= 0)
                    {
                        _backupService.DeleteBackupJob(index);
                        StatusText = $"Job supprimé: {jobName}";
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"Erreur lors de la suppression: {ex.Message}", 
                        "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    StatusText = "Erreur suppression";
                }
            }
        }

        private bool CanDeleteBackup(object? parameter) => SelectedBackupJob != null;

        /// <summary>
        /// Exécute tous les jobs
        /// </summary>
        private void ExecuteAll(object? parameter)
        {
            try
            {
                var indices = Enumerable.Range(0, BackupJobs.Count).ToList();
                StatusText = $"Exécution de {indices.Count} job(s)...";
                
                // Exécuter de façon asynchrone pour ne pas bloquer l'UI
                System.Threading.Tasks.Task.Run(() =>
                {
                    try
                    {
                        _backupService.ExecuteMultipleBackupJobs(indices);
                        System.Windows.Application.Current.Dispatcher.Invoke(() =>
                        {
                            StatusText = $"Exécution de {indices.Count} job(s) terminée";
                        });
                    }
                    catch (Exception ex)
                    {
                        System.Windows.Application.Current.Dispatcher.Invoke(() =>
                        {
                            System.Windows.MessageBox.Show($"Erreur lors de l'exécution: {ex.Message}", 
                                "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                            StatusText = "Erreur exécution multiple";
                        });
                    }
                });
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Erreur lors de l'exécution: {ex.Message}", 
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                StatusText = "Erreur exécution multiple";
            }
        }

        /// <summary>
        /// Rafraîchit la liste des jobs
        /// </summary>
        private void Refresh(object? parameter)
        {
            LoadJobs();
        }

        // Event handlers pour les events P2
        private void OnJobCreated(object? sender, BackupJob job)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                BackupJobs.Add(new BackupJobViewModel(job));
                StatusText = $"Job créé: {job.Name}";
            });
        }

        private void OnJobDeleted(object? sender, BackupJob job)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                var vm = BackupJobs.FirstOrDefault(j => j.Name == job.Name);
                if (vm != null)
                {
                    BackupJobs.Remove(vm);
                }
            });
        }

        private void OnJobUpdated(object? sender, BackupJob job)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                var vm = BackupJobs.FirstOrDefault(j => j.Name == job.Name);
                if (vm != null)
                {
                    vm.UpdateFromModel(job);
                }
            });
        }
    }
}
