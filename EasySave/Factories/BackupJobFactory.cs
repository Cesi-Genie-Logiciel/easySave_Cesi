using System;
using System.IO;
using EasySave.Models;
using EasySave.Strategies;
using EasySave.Observers;
using EasySave.Interfaces;
using EasySave.Services;
using ProSoft.EasyLog;
using ProSoft.EasyLog.Implementation;

namespace EasySave.Factories
{
    public class BackupJobFactory
    {
        public static BackupJob CreateBackupJob(string name, string source, string target, string type)
        {
            // Validation (dev)
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Backup name cannot be empty");

            if (!Directory.Exists(source))
                throw new DirectoryNotFoundException($"Source not found: {source}");

            // CryptoSoft integration (P4): try to wire the external tool if present.
            // If not present, strategies still work normally.
            ICryptoService? cryptoService = null;
            try
            {
                var overridePath = Environment.GetEnvironmentVariable("EASY_SAVE_CRYPTOSOFT_PATH");
                var cryptoPath = !string.IsNullOrWhiteSpace(overridePath)
                    ? overridePath
                    : CryptoSoftService.TryGetDefaultCryptoSoftPath();

                if (!string.IsNullOrWhiteSpace(cryptoPath))
                {
                    var service = new CryptoSoftService(cryptoPath);
                    if (service.IsAvailable())
                        cryptoService = service;
                }
            }
            catch
            {
                // Keep crypto optional.
            }

            // Choisir la stratégie (dev) + injection crypto
            IBackupStrategy strategy;
            switch (type.ToLower())
            {
                case "complete":
                    strategy = new CompleteBackupStrategy(cryptoService);
                    break;
                case "differential":
                    strategy = new DifferentialBackupStrategy(cryptoService);
                    break;
                default:
                    throw new ArgumentException($"Unknown backup type: {type}");
            }

            // Créer le job (dev)
            var job = new BackupJob(name, source, target, strategy);

            // Ajouter les observateurs (dev)
            // EasyLog: création via la factory (pas de singleton Logger.Instance dans cette implémentation)
            var logger = LoggerFactory.CreateLogger(LogFormat.JSON, Path.Combine(target, "logs"));

            job.AddObserver(new ConsoleObserver());
            job.AddObserver(new LoggerObserver(logger));
            job.AddObserver(new StateObserver(logger));

            return job;
        }
    }
}
