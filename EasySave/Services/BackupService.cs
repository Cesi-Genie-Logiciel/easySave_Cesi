using System;
using System.Collections.Generic;
using System.Linq;
using EasySave.Models;
using EasySave.Factories;
using EasySave.Interfaces;

namespace EasySave.Services
{
    public class BackupService : IBackupService
    {
        private List<BackupJob> _jobs = new List<BackupJob>();
        private readonly IJobStorageService _storageService;
        
        /// <summary>
        /// Constructeur avec injection du service de stockage
        /// Charge automatiquement les jobs sauvegardés
        /// </summary>
        public BackupService(IJobStorageService storageService)
        {
            _storageService = storageService;
            LoadJobsFromStorage();
        }
        
        /// <summary>
        /// Constructeur par défaut (rétrocompatibilité)
        /// Crée un service de stockage par défaut
        /// </summary>
        public BackupService() : this(new JobStorageService())
        {
        }
        
        /// <summary>
        /// Charge les jobs depuis le stockage persistant
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
        /// Sauvegarde tous les jobs dans le stockage persistant
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
        /// Note: Pour v2.0, cette info devrait être exposée dans BackupJob
        /// </summary>
        private string GetBackupType(BackupJob job)
        {
            // Pour l'instant, on ne peut pas déterminer le type depuis le job
            // On retourne "complete" par défaut
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
            
            // Sauvegarder les jobs après création
            SaveJobsToStorage();
            
            // Déclencher l'event pour la GUI
            JobCreated?.Invoke(this, job);
        }
        
        public List<BackupJob> GetAllBackupJobs()
        {
            return new List<BackupJob>(_jobs);
        }
        
        public void ExecuteBackupJob(int index)
        {
            if (index < 0 || index >= _jobs.Count)
            {
                throw new ArgumentOutOfRangeException($"Invalid job index: {index}");
            }
            
            _jobs[index].Execute();
        }
        
        public void ExecuteMultipleBackupJobs(List<int> indices)
        {
            foreach (var index in indices)
            {
                ExecuteBackupJob(index);
            }
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
            
            // Sauvegarder les jobs après suppression
            SaveJobsToStorage();
            
            // Déclencher l'event pour la GUI
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
            
            // Supprimer l'ancien job et créer un nouveau
            var oldJob = _jobs[index];
            _jobs.RemoveAt(index);
            
            var newJob = BackupJobFactory.CreateBackupJob(name, source, target, type);
            _jobs.Insert(index, newJob);
            
            Console.WriteLine($"Backup job updated: '{oldJob.Name}' -> '{name}'");
            
            // Sauvegarder après update
            SaveJobsToStorage();
            
            // Déclencher l'event pour la GUI
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
