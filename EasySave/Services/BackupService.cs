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
        
        // ✅ FEATURE P2: Events pour notifier la GUI (v2.0)
        public event EventHandler<BackupJob>? JobCreated;
        public event EventHandler<BackupJob>? JobDeleted;
        public event EventHandler<BackupJob>? JobUpdated;
        
        public void CreateBackupJob(string name, string source, string target, string type)
        {
            // ✅ FEATURE P2: Limite de 5 jobs supprimée
            
            var job = BackupJobFactory.CreateBackupJob(name, source, target, type);
            _jobs.Add(job);
            Console.WriteLine($"Backup job '{name}' created successfully");
            
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
