using System;
using System.Collections.Generic;
using System.Linq;
using EasySave.Models;
using EasySave.Factories;
using EasySave.Interfaces;
using ProSoft.EasyLog.Interfaces;

namespace EasySave.Services
{
    public class BackupService : IBackupService
    {
        private readonly List<BackupJob> _jobs = new List<BackupJob>();
        private readonly IJobStorageService _storage;
        private readonly BusinessSoftwareMonitor _monitor = new BusinessSoftwareMonitor();
        private readonly ILogger _logger;

        public BackupService(IJobStorageService storage)
        {
            _storage = storage;
            var logDir = Path.Combine(AppContext.BaseDirectory, "logs");
            _logger = ProSoft.EasyLog.Implementation.LoggerFactory.CreateLogger(
                ProSoft.EasyLog.LogFormat.JSON, logDir);
            ChargerJobs();
        }

        public BackupService() : this(new JobStorageService()) { }

        private void ChargerJobs()
        {
            try
            {
                foreach (var config in _storage.LoadJobs())
                {
                    try
                    {
                        _jobs.Add(BackupJobFactory.CreateBackupJob(
                            config.Name, config.SourcePath, config.TargetPath, config.BackupType));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Impossible de charger le job '{config.Name}' : {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur au chargement des jobs : {ex.Message}");
            }
        }

        private void SauvegarderJobs()
        {
            try
            {
                var configs = _jobs.Select(j => new BackupConfig(
                    j.Name, j.SourcePath, j.TargetPath, j.BackupType)).ToList();
                _storage.SaveJobs(configs);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur a la sauvegarde : {ex.Message}");
            }
        }

        public void CreateBackupJob(string name, string source, string target, string type)
        {
            var job = BackupJobFactory.CreateBackupJob(name, source, target, type);
            _jobs.Add(job);
            SauvegarderJobs();
        }

        public List<BackupJob> GetAllBackupJobs() => new List<BackupJob>(_jobs);

        public void ExecuteBackupJob(int index)
        {
            if (index < 0 || index >= _jobs.Count)
                throw new ArgumentOutOfRangeException($"Index invalide : {index}");

            if (_monitor.IsRunning())
            {
                var name = _jobs[index].Name;
                Console.WriteLine($"\nJob '{name}' refuse : logiciel metier en cours d'execution.");
                _logger.LogJobEvent(name, ProSoft.EasyLog.Models.JobEventType.Refused,
                    "Logiciel metier detecte", _monitor.ProcessName);
                return;
            }

            _jobs[index].Execute();
        }

        public void ExecuteMultipleBackupJobs(List<int> indices)
        {
            foreach (var index in indices)
                ExecuteBackupJob(index);
        }

        public void DeleteBackupJob(int index)
        {
            if (index < 0 || index >= _jobs.Count)
                throw new ArgumentOutOfRangeException($"Index invalide : {index}");

            var name = _jobs[index].Name;
            _jobs.RemoveAt(index);
            Console.WriteLine($"Job '{name}' supprime.");
            SauvegarderJobs();
        }

        public void UpdateBackupJob(int index, string name, string source, string target, string type)
        {
            if (index < 0 || index >= _jobs.Count)
                throw new ArgumentOutOfRangeException($"Index invalide : {index}");

            _jobs.RemoveAt(index);
            var newJob = BackupJobFactory.CreateBackupJob(name, source, target, type);
            _jobs.Insert(index, newJob);
            Console.WriteLine($"Job mis a jour : '{name}'");
            SauvegarderJobs();
        }
    }
}