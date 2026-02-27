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
        private readonly IJobStorageService _storageService;
        private readonly IBusinessSoftwareDetector? _businessSoftwareDetector;
        private readonly ILogger _serviceLogger;

        public event EventHandler<BackupJob>? JobCreated;
        public event EventHandler<BackupJob>? JobDeleted;
        public event EventHandler<BackupJob>? JobUpdated;

        public BackupService(IJobStorageService storageService, IBusinessSoftwareDetector? businessSoftwareDetector = null)
        {
            _storageService = storageService;
            _businessSoftwareDetector = businessSoftwareDetector;

            Console.WriteLine("=== Detector recu : " + (_businessSoftwareDetector != null ? "OUI" : "NON") + " ===");

            // If a business software detector was provided, wire up the auto-pause logic
            if (_businessSoftwareDetector != null)
            {
                _businessSoftwareDetector.BusinessSoftwareStateChanged += (bool isRunning) =>
                {
                    if (isRunning)
                    {
                        Console.WriteLine("[BusinessSoftware] Detected - pausing all backup jobs");
                        PauseAllBackupJobs();
                    }
                    else
                    {
                        Console.WriteLine("[BusinessSoftware] Closed - resuming all backup jobs");
                        ResumeAllBackupJobs();
                    }
                };

                // Start checking for the business software process in background
                _businessSoftwareDetector.StartMonitoring();
            }

            string logDir = Path.Combine(AppContext.BaseDirectory, "logs");
            _serviceLogger = ProSoft.EasyLog.Implementation.LoggerFactory.CreateLogger(ProSoft.EasyLog.LogFormat.JSON, logDir);

            LoadJobsFromStorage();
        }

        public BackupService() : this(new JobStorageService(), null)
        {
        }

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
                            config.Name,
                            config.SourcePath,
                            config.TargetPath,
                            config.BackupType);
                        _jobs.Add(job);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to load job '{config.Name}': {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading jobs from storage: {ex.Message}");
            }
        }

        private void SaveJobsToStorage()
        {
            try
            {
                List<BackupConfig> configs = _jobs.Select(job => new BackupConfig(
                    job.Name,
                    job.SourcePath,
                    job.TargetPath,
                    GetBackupType(job)
                )).ToList();

                _storageService.SaveJobs(configs);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving jobs to storage: {ex.Message}");
            }
        }

        private string GetBackupType(BackupJob job) => job.BackupType;

        public void CreateBackupJob(string name, string source, string target, string type)
        {
            BackupJob job = BackupJobFactory.CreateBackupJob(name, source, target, type);
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

            // Block starting a job if business software is currently running
            if (_businessSoftwareDetector?.IsBusinessSoftwareRunning() == true)
            {
                string jobName = _jobs[index].Name;
                string process = _businessSoftwareDetector.BusinessSoftwareName ?? string.Empty;

                Console.WriteLine($"[BusinessSoftware] Job '{jobName}' not started: business software is running.");
                _serviceLogger.LogJobEvent(jobName, ProSoft.EasyLog.Models.JobEventType.Refused, "Business software running", process);

                return false;
            }

            _jobs[index].Execute();
            return true;
        }

        public void ExecuteMultipleBackupJobs(List<int> indices)
        {
            foreach (int index in indices)
            {
                ExecuteBackupJob(index);
            }
        }

        public async Task ExecuteAllBackupJobsParallel()
        {
            List<BackupJob> jobsSnapshot = _jobs.ToList();
            if (jobsSnapshot.Count == 0)
            {
                return;
            }

            ParallelBackupCoordinator coordinator = new ParallelBackupCoordinator();
            await coordinator.ExecuteJobsInParallel(jobsSnapshot);
        }

        public void DeleteBackupJob(int index)
        {
            if (index < 0 || index >= _jobs.Count)
            {
                throw new ArgumentOutOfRangeException($"Invalid job index: {index}");
            }

            BackupJob job = _jobs[index];
            string jobName = job.Name;
            _jobs.RemoveAt(index);
            Console.WriteLine($"Backup job '{jobName}' deleted");

            SaveJobsToStorage();
            JobDeleted?.Invoke(this, job);
        }

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

            BackupJob oldJob = _jobs[index];
            _jobs.RemoveAt(index);

            BackupJob newJob = BackupJobFactory.CreateBackupJob(name, source, target, type);
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
                throw new ArgumentNullException(nameof(job));
            job.Stop();
        }

        public void ResumeBackupJob(BackupJob job)
        {
            if (job == null)
                throw new ArgumentNullException(nameof(job));
            job.Resume();
        }

        public void PauseAllBackupJobs()
        {
            foreach (BackupJob job in _jobs)
                job.Pause();
        }

        public void ResumeAllBackupJobs()
        {
            foreach (BackupJob job in _jobs)
                job.Resume();
        }

        public void StopAllBackupJobs()
        {
            foreach (BackupJob job in _jobs)
                job.Stop();
        }
    }
}