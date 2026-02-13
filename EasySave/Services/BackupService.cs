using System;
using System.Collections.Generic;
using EasySave.Models;
using EasySave.Factories;
using EasySave.Interfaces;
using ProSoft.EasyLog;
using ProSoft.EasyLog.Interfaces;

namespace EasySave.Services
{
    public class BackupService : IBackupService
    {
        private List<BackupJob> _jobs = new List<BackupJob>();
        private readonly BusinessSoftwareMonitor _businessSoftwareMonitor = new BusinessSoftwareMonitor();
        private readonly ILogger _serviceLogger;

        public BackupService()
        {
            // Service-level logger used for job-level events (Refused/Interrupted) when needed.
            // Kept independent from job target paths to avoid refactoring the job model.
            var logDir = Path.Combine(AppContext.BaseDirectory, "logs");
            _serviceLogger = ProSoft.EasyLog.Implementation.LoggerFactory.CreateLogger(LogFormat.JSON, logDir);
        }

        public void CreateBackupJob(string name, string source, string target, string type)
        {
            if (_jobs.Count >= 5)
            {
                throw new InvalidOperationException("Maximum 5 backup jobs allowed");
            }

            var job = BackupJobFactory.CreateBackupJob(name, source, target, type);
            _jobs.Add(job);
            Console.WriteLine($"Backup job '{name}' created successfully");
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

            // Business software detection: block starting a job if the configured process is running.
            if (_businessSoftwareMonitor.IsRunning())
            {
                var jobName = _jobs[index].Name;
                var process = _businessSoftwareMonitor.ProcessName ?? string.Empty;

                Console.WriteLine($"\n[BusinessSoftware] Job '{jobName}' not started: business software is running.");

                // Log refusal (v2.0 requirement)
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

        public void DeleteBackupJob(int index)
        {
            if (index < 0 || index >= _jobs.Count)
            {
                throw new ArgumentOutOfRangeException($"Invalid job index: {index}");
            }

            var jobName = _jobs[index].Name;
            _jobs.RemoveAt(index);
            Console.WriteLine($"Backup job '{jobName}' deleted");
        }
    }
}
