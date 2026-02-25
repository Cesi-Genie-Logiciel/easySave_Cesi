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
using ProSoft.EasyLog.Models;

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

            // Logger (P4: Local / Centralized / Both via settings)
            ProSoft.EasyLog.Interfaces.ILogger logger;
            var extensions = new List<string>();
            try
            {
                var settings = new SettingsService().GetCurrent();
                extensions = settings.ExtensionsToEncrypt ?? new List<string>();

                var destStr = (settings.LogDestination ?? "Local").Trim();
                var serverUrl = settings.LogServerUrl?.Trim() ?? "http://localhost:5000";

                if (string.Equals(destStr, "Local", StringComparison.OrdinalIgnoreCase))
                {
                    logger = LoggerFactory.CreateLogger(
                        ProSoft.EasyLog.LogFormat.JSON,
                        Path.Combine(target, "logs"));
                }
                else
                {
                    var destination = string.Equals(destStr, "Both", StringComparison.OrdinalIgnoreCase)
                        ? LogDestination.Both
                        : LogDestination.Centralized;
                    logger = LoggerFactory.Create(ProSoft.EasyLog.LogFormat.JSON, destination, serverUrl);
                }
            }
            catch
            {
                logger = LoggerFactory.CreateLogger(
                    ProSoft.EasyLog.LogFormat.JSON,
                    Path.Combine(target, "logs"));
            }

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