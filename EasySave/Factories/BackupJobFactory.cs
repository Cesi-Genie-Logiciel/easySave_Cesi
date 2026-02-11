using System;
using System.IO;
using EasySave.Models;
using EasySave.Strategies;
using EasySave.Observers;
using EasySave.Interfaces;
using ProSoft.EasyLog;

namespace EasySave.Factories
{
    public class BackupJobFactory
    {
        public static BackupJob CreateBackupJob(string name, string source, string target, string type)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Backup name cannot be empty");
            
            if (!Directory.Exists(source))
                throw new DirectoryNotFoundException($"Source not found: {source}");
            
            // Choisir la stratégie
            IBackupStrategy strategy;
            switch (type.ToLower())
            {
                case "complete":
                    strategy = new CompleteBackupStrategy();
                    break;
                case "differential":
                    strategy = new DifferentialBackupStrategy();
                    break;
                default:
                    throw new ArgumentException($"Unknown backup type: {type}");
            }
            
            // Créer le job
            var job = new BackupJob(name, source, target, strategy);
            
            // Créer les observateurs
            string appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "EasySave"
            );
            string logPath = Path.Combine(appDataPath, "Logs");
            string statePath = Path.Combine(appDataPath, "State");
            
            Directory.CreateDirectory(logPath);
            Directory.CreateDirectory(statePath);
            
            var logger = new JsonFileLogger(logPath);
            
            job.AddObserver(new ConsoleObserver());
            job.AddObserver(new LoggerObserver(logger));
            job.AddObserver(new StateObserver(statePath));
            
            return job;
        }
    }
}
