using System;
using System.Collections.Generic;
using System.IO;
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
        private readonly IJobStorageService _storageService;
        private readonly ParallelBackupCoordinator _coordinator;
        private readonly ILogger _serviceLogger;

        public BackupService(IJobStorageService storageService)
        {
            _storageService = storageService;
            _coordinator = new ParallelBackupCoordinator();

            string logDir = Path.Combine(AppContext.BaseDirectory, "logs");
            _serviceLogger = ProSoft.EasyLog.Implementation.LoggerFactory.CreateLogger(
                ProSoft.EasyLog.LogFormat.JSON, logDir);

            LoadJobsFromStorage();
        }

        public BackupService() : this(new JobStorageService()) { }

        // Loads saved job configurations and recreates the job objects
        private void LoadJobsFromStorage()
        {
            try
            {
                List<BackupConfig> configs = _storageService.LoadJobs();
                foreach (BackupConfig config in configs)
                {
                    try
                    {
                        BackupJob job = BackupJobFactory.CreateBackupJob(
                            config.Name, config.SourcePath,
                            config.TargetPath, config.BackupType);
                        _jobs.Add(job);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"  Failed to load job '{config.Name}': {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  Error loading jobs from storage: {ex.Message}");
            }
        }

        private void SaveJobsToStorage()
        {
            try
            {
                List<BackupConfig> configs = _jobs.Select(job => new BackupConfig(
                    job.Name, job.SourcePath, job.TargetPath, job.BackupType
                )).ToList();
                _storageService.SaveJobs(configs);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  Error saving jobs to storage: {ex.Message}");
            }
        }

        public event EventHandler<BackupJob>? JobCreated;
        public event EventHandler<BackupJob>? JobDeleted;
        public event EventHandler<BackupJob>? JobUpdated;

        public void CreateBackupJob(string name, string source, string target, string type)
        {
            BackupJob job = BackupJobFactory.CreateBackupJob(name, source, target, type);
            _jobs.Add(job);
            Console.WriteLine($"Backup job '{name}' created successfully");
            SaveJobsToStorage();
            JobCreated?.Invoke(this, job);
        }

        public List<BackupJob> GetAllBackupJobs() => new List<BackupJob>(_jobs);

        public void ExecuteBackupJob(int index)
        {
            if (index < 0 || index >= _jobs.Count)
                throw new ArgumentOutOfRangeException($"Invalid job index: {index}");
            _jobs[index].Execute();
        }

        public void ExecuteMultipleBackupJobs(List<int> indices)
        {
            foreach (int index in indices)
                ExecuteBackupJob(index);
        }

        public async Task ExecuteAllBackupJobsParallel()
        {
            List<BackupJob> jobsSnapshot = _jobs.ToList();
            if (jobsSnapshot.Count == 0) return;
            await _coordinator.ExecuteJobsInParallel(jobsSnapshot);
        }

        public void DeleteBackupJob(int index)
        {
            if (index < 0 || index >= _jobs.Count)
                throw new ArgumentOutOfRangeException($"Invalid job index: {index}");

            BackupJob job = _jobs[index];
            _jobs.RemoveAt(index);
            Console.WriteLine($"Backup job '{job.Name}' deleted");
            SaveJobsToStorage();
            JobDeleted?.Invoke(this, job);
        }

        public BackupJob GetJobByIndex(int index)
        {
            if (index < 0 || index >= _jobs.Count) return null;
            return _jobs[index];
        }

        public BackupJob GetJobByName(string name)
        {
            return _jobs.FirstOrDefault(j => j.Name == name);
        }

        public void UpdateBackupJob(int index, string name, string source, string target, string type)
        {
            if (index < 0 || index >= _jobs.Count)
                throw new ArgumentOutOfRangeException($"Invalid job index: {index}");

            BackupJob oldJob = _jobs[index];
            _jobs.RemoveAt(index);

            BackupJob newJob = BackupJobFactory.CreateBackupJob(name, source, target, type);
            _jobs.Insert(index, newJob);

            Console.WriteLine($"Backup job updated: '{oldJob.Name}' -> '{name}'");
            SaveJobsToStorage();
            JobUpdated?.Invoke(this, newJob);
        }

        // Individual job controls - delegate to the job directly
        public void PauseBackupJob(BackupJob job) => job.Pause();
        public void ResumeBackupJob(BackupJob job) => job.Resume();
        public void StopBackupJob(BackupJob job) => job.Stop();

        // Batch controls - delegate to the coordinator which tracks running tasks
        public void PauseAllBackupJobs() => _coordinator.PauseAll();
        public void ResumeAllBackupJobs() => _coordinator.ResumeAll();
        public void StopAllBackupJobs() => _coordinator.StopAll();
    }
}