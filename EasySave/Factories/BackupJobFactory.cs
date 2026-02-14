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
    /// <summary>
    /// Factory for creating BackupJob instances with proper configuration.
    /// Handles strategy selection, crypto service integration, and observer setup.
    /// </summary>
    public class BackupJobFactory
    {
        /// <summary>
        /// Creates a configured BackupJob instance with all dependencies.
        /// </summary>
        /// <param name="name">Unique backup job name</param>
        /// <param name="source">Source directory path</param>
        /// <param name="target">Target directory path</param>
        /// <param name="type">Backup type: "complete" or "differential"</param>
        /// <returns>Fully configured BackupJob ready for execution</returns>
        public static BackupJob CreateBackupJob(string name, string source, string target, string type)
        {
            // Validate input parameters
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Backup name cannot be empty");
            if (!Directory.Exists(source))
                throw new DirectoryNotFoundException($"Source not found: {source}");

            // CryptoSoft integration: try to wire the external tool if present
            // If not present, strategies still work normally without encryption
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
                // Keep crypto optional - continue without it if unavailable
            }

            // EasyLog: création via la factory
            var logger = LoggerFactory.CreateLogger(LogFormat.JSON, Path.Combine(target, "logs"));

            // Select strategy + inject crypto + logger (needed for JobEventType.Interrupted)
            IBackupStrategy strategy;
            switch (type.ToLower())
            {
                case "complete":
                    strategy = new CompleteBackupStrategy(cryptoService, logger: logger);
                    break;
                case "differential":
                    strategy = new DifferentialBackupStrategy(cryptoService, logger: logger);
                    break;
                default:
                    throw new ArgumentException($"Unknown backup type: {type}");
            }

            // Create the backup job with selected strategy
            var job = new BackupJob(name, source, target, strategy);

            // Attach observers for monitoring and logging
            job.AddObserver(new ConsoleObserver());
            job.AddObserver(new LoggerObserver(logger));
            job.AddObserver(new StateObserver(logger));

            return job;
        }
    }
}