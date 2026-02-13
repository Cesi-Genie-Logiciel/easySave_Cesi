using System;
using System.IO;
using EasySave.Models;
using EasySave.Strategies;
using EasySave.Observers;
using EasySave.Interfaces;
using ProSoft.EasyLog;
using ProSoft.EasyLog.Implementation;

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

            // Ajouter les observateurs
            // EasyLog: création via la factory (pas de singleton Logger.Instance dans cette implémentation)
            var logger = LoggerFactory.CreateLogger(LogFormat.JSON, Path.Combine(target, "logs"));

            job.AddObserver(new ConsoleObserver());
            job.AddObserver(new LoggerObserver(logger));
            job.AddObserver(new StateObserver(logger));

            return job;
        }
    }
}
