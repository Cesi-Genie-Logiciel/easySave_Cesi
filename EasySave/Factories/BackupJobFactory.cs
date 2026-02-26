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
    // Creates fully configured BackupJob instances with all their dependencies:
    // crypto service, logger, strategy, and observers.
    public class BackupJobFactory
    {
        public static BackupJob CreateBackupJob(string name, string source, string target, string type)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Job name cannot be empty");

            if (!Directory.Exists(source))
                throw new DirectoryNotFoundException("Source folder not found: " + source);

            // Try to set up the encryption service if CryptoSoft is available
            ICryptoService? cryptoService = null;
            try
            {
                string? path = Environment.GetEnvironmentVariable("EASY_SAVE_CRYPTOSOFT_PATH")
                               ?? CryptoSoftService.TryGetDefaultCryptoSoftPath();
                if (!string.IsNullOrWhiteSpace(path))
                {
                    CryptoSoftService svc = new CryptoSoftService(path);
                    if (svc.IsAvailable())
                        cryptoService = svc;
                }
            }
            catch { }

            // Set up the logger and read encryption extensions from settings
            ProSoft.EasyLog.Interfaces.ILogger logger;
            List<string> extensions = new List<string>();
            try
            {
                AppSettings settings = new SettingsService().GetCurrent();
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
                    var centralizedDir = Path.Combine(target, "logs", "centralized");
                    logger = LoggerFactory.CreateLogger(ProSoft.EasyLog.LogFormat.JSON, centralizedDir);
                    _ = serverUrl;
                }
            }
            catch
            {
                // Fallback if settings cannot be loaded
                logger = LoggerFactory.CreateLogger(
                    ProSoft.EasyLog.LogFormat.JSON,
                    Path.Combine(target, "logs"));
            }

            // Pick the right backup strategy based on the type
            IBackupStrategy strategy = type.ToLower() switch
            {
                "complete" => new CompleteBackupStrategy(cryptoService, logger: logger, extensionsToEncrypt: extensions),
                "differential" => new DifferentialBackupStrategy(cryptoService, logger: logger, extensionsToEncrypt: extensions),
                _ => throw new ArgumentException("Unknown backup type: " + type)
            };

            // Wire up the job with its observers
            BackupJob job = new BackupJob(name, source, target, type, strategy);
            job.AddObserver(new ConsoleObserver());
            job.AddObserver(new LoggerObserver(logger));
            job.AddObserver(new StateObserver(logger));

            return job;
        }
    }
}