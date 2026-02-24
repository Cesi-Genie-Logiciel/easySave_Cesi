using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasySave.Models;
using EasySave.Factories;
using EasySave.Interfaces;
using ProSoft.EasyLog;
using ProSoft.EasyLog.Interfaces;

namespace EasySave.Services
{
    public class BackupService : IBackupService
    {
        private readonly List<BackupJob> _jobs = new List<BackupJob>();

        // P2 (GUI): stockage persistant
        private readonly IJobStorageService _storageService;

        // P4: détection logiciel métier + log job event
        private readonly IBusinessSoftwareDetector? _businessSoftwareDetector;
        private readonly ILogger _serviceLogger;

        /// <summary>
        /// Constructeur avec injection du service de stockage.
        /// Charge automatiquement les jobs sauvegardés.
        /// IMPORTANT: le détecteur est injecté via l'interface IBusinessSoftwareDetector.
        /// </summary>
        public BackupService(IJobStorageService storageService, IBusinessSoftwareDetector? businessSoftwareDetector = null)
        {
            _storageService = storageService;
            _businessSoftwareDetector = businessSoftwareDetector;

            // Hook prêt pour v3 (non utilisé ici) : abonnement seulement si un détecteur est injecté.
            if (_businessSoftwareDetector != null)
            {
                _businessSoftwareDetector.BusinessSoftwareStateChanged += _ => { /* TODO v3: orchestration auto-pause */ };
            }

            var logDir = Path.Combine(AppContext.BaseDirectory, "logs");
            _serviceLogger = ProSoft.EasyLog.Implementation.LoggerFactory.CreateLogger(ProSoft.EasyLog.LogFormat.JSON, logDir);

            LoadJobsFromStorage();
        }

        /// <summary>
        /// Constructeur par défaut (rétrocompatibilité)
        /// Crée un service de stockage par défaut.
        /// </summary>
        public BackupService() : this(new JobStorageService(), null)
        {
        }

        /// <summary>
        /// Charge les jobs depuis le stockage persistant.
        /// </summary>
        private void LoadJobsFromStorage()
        {
            try
            {
                var configs = _storageService.LoadJobs();
                foreach (var config in configs)
                {
                    try
                    {
                        var job = BackupJobFactory.CreateBackupJob(
                            config.Name,
                            config.SourcePath,
                            config.TargetPath,
                            config.BackupType);
                        _jobs.Add(job);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"⚠️  Failed to load job '{config.Name}': {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error loading jobs from storage: {ex.Message}");
            }
        }

        /// <summary>
        /// Sauvegarde tous les jobs dans le stockage persistant.
        /// </summary>
        private void SaveJobsToStorage()
        {
            try
            {
                var configs = _jobs.Select(job => new BackupConfig(
                    job.Name,
                    job.SourcePath,
                    job.TargetPath,
                    GetBackupType(job)
                )).ToList();

                _storageService.SaveJobs(configs);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error saving jobs to storage: {ex.Message}");
            }
        }

        /// <summary>
        /// Récupère le type de backup d'un job (helper method)
        /// Note: Pour v2.0, cette info devrait être exposée dans BackupJob.
        /// </summary>
        private string GetBackupType(BackupJob job)
        {
            // TODO v2.0: Ajouter une propriété BackupType dans BackupJob
            return "complete";
        }

        // ✅ FEATURE P2: Events pour notifier la GUI (v2.0)
        public event EventHandler<BackupJob>? JobCreated;
        public event EventHandler<BackupJob>? JobDeleted;
        public event EventHandler<BackupJob>? JobUpdated;

        public void CreateBackupJob(string name, string source, string target, string type)
        {
            // ✅ FEATURE P2: Limite de 5 jobs supprimée - stockage illimité
            var job = BackupJobFactory.CreateBackupJob(name, source, target, type);
            _jobs.Add(job);
            Console.WriteLine($"Backup job '{name}' created successfully");

            SaveJobsToStorage();
            JobCreated?.Invoke(this, job);
        }

        public List<BackupJob> GetAllBackupJobs()
        {
            return new List<BackupJob>(_jobs);
        }

        public void ExecuteBackupJob(int index)
        {
            TryExecuteBackupJob(index);
        }

        public bool TryExecuteBackupJob(int index)
        {
            if (index < 0 || index >= _jobs.Count)
            {
                throw new ArgumentOutOfRangeException($"Invalid job index: {index}");
            }

            // P4: Interdire de démarrer si logiciel métier détecté
            if (_businessSoftwareDetector?.IsBusinessSoftwareRunning() == true)
            {
                var jobName = _jobs[index].Name;
                var process = _businessSoftwareDetector.BusinessSoftwareName ?? string.Empty;

                Console.WriteLine($"\n[BusinessSoftware] Job '{jobName}' not started: business software is running.");
                _serviceLogger.LogJobEvent(jobName, ProSoft.EasyLog.Models.JobEventType.Refused, "Business software running", process);

                return false;
            }

            _jobs[index].Execute();
            return true;
        }

        public void ExecuteMultipleBackupJobs(List<int> indices)
        {
            foreach (var index in indices)
            {
                ExecuteBackupJob(index);
            }
        }

        public async Task ExecuteAllBackupJobsParallel()
        {
            // V3/P1 (step 2): délègue l'orchestration au coordinateur.
            // IMPORTANT: on n'appelle PAS TryExecuteBackupJob ici (logique P4).

            var jobsSnapshot = _jobs.ToList();
            if (jobsSnapshot.Count == 0)
            {
                return;
            }

            var coordinator = new ParallelBackupCoordinator();
            await coordinator.ExecuteJobsInParallel(jobsSnapshot);
        }

        public void DeleteBackupJob(int index)
        {
            if (index < 0 || index >= _jobs.Count)
            {
                throw new ArgumentOutOfRangeException($"Invalid job index: {index}");
            }

            var job = _jobs[index];
            var jobName = job.Name;
            _jobs.RemoveAt(index);
            Console.WriteLine($"Backup job '{jobName}' deleted");

            SaveJobsToStorage();
            JobDeleted?.Invoke(this, job);
        }

        // ✅ FEATURE P2: Nouvelles méthodes pour GUI/MVVM (v2.0)
        public BackupJob? GetJobByIndex(int index)
        {
            if (index < 0 || index >= _jobs.Count)
            {
                return null;
            }
            return _jobs[index];
        }

        public BackupJob? GetJobByName(string name)
        {
            return _jobs.FirstOrDefault(j => j.Name == name);
        }

        public void UpdateBackupJob(int index, string name, string source, string target, string type)
        {
            if (index < 0 || index >= _jobs.Count)
            {
                throw new ArgumentOutOfRangeException($"Invalid job index: {index}");
            }

            var oldJob = _jobs[index];
            _jobs.RemoveAt(index);

            var newJob = BackupJobFactory.CreateBackupJob(name, source, target, type);
            _jobs.Insert(index, newJob);

            Console.WriteLine($"Backup job updated: '{oldJob.Name}' -> '{name}'");

            SaveJobsToStorage();
            JobUpdated?.Invoke(this, newJob);
        }

        public void PauseBackupJob(BackupJob job)
        {
            if (job == null)
            {
                throw new ArgumentNullException(nameof(job));
            }

            job.Pause();
        }

        public void StopBackupJob(BackupJob job)
        {
            if (job == null)
            {
                throw new ArgumentNullException(nameof(job));
            }

            job.Stop();
        }
    }
}
