using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
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
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Le nom ne peut pas etre vide");
            if (!Directory.Exists(source))
                throw new DirectoryNotFoundException($"Dossier source introuvable : {source}");

            // Cruptographic service (optional)
            ICryptoService? cryptoService = null;
            try
            {
                var path = Environment.GetEnvironmentVariable("EASY_SAVE_CRYPTOSOFT_PATH")
                           ?? CryptoSoftService.TryGetDefaultCryptoSoftPath();

                if (!string.IsNullOrWhiteSpace(path))
                {
                    var svc = new CryptoSoftService(path);
                    if (svc.IsAvailable())
                        cryptoService = svc;
                }
            }
            catch { }

            // Logger
            var logger = LoggerFactory.CreateLogger(
                ProSoft.EasyLog.LogFormat.JSON,
                Path.Combine(target, "logs"));

            // extensions to encrypt from settings
            var extensions = new List<string>();
            try
            {
                var settings = new SettingsService().GetCurrent();
                extensions = settings.ExtensionsToEncrypt ?? new List<string>();
            }
            catch { }

            // choice of strategy
            IBackupStrategy strategy = type.ToLower() switch
            {
                "complete" => new CompleteBackupStrategy(cryptoService, logger: logger, extensionsToEncrypt: extensions),
                "differential" => new DifferentialBackupStrategy(cryptoService, logger: logger, extensionsToEncrypt: extensions),
                _ => throw new ArgumentException($"Type de sauvegarde inconnu : {type}")
            };

            var job = new BackupJob(name, source, target, type, strategy);

            job.AddObserver(new ConsoleObserver());
            job.AddObserver(new LoggerObserver(logger));
            job.AddObserver(new StateObserver(logger));

            return job;
        }
    }
}