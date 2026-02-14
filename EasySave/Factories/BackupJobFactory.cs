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
            string? cryptoPath = null;
            try
            {
                var overridePath = Environment.GetEnvironmentVariable("EASY_SAVE_CRYPTOSOFT_PATH");
                cryptoPath = !string.IsNullOrWhiteSpace(overridePath)
                    ? overridePath
                    : CryptoSoftService.TryGetDefaultCryptoSoftPath();

                // Fallback candidates (cross-platform, includes RID folders like linux-x64)
                if (string.IsNullOrWhiteSpace(cryptoPath) || !File.Exists(cryptoPath))
                {
                    cryptoPath = CryptoSoftService.GetDefaultCryptoSoftCandidatesForDebug()
                        .FirstOrDefault(File.Exists);
                }

                if (!string.IsNullOrWhiteSpace(cryptoPath))
                {
                    var service = new CryptoSoftService(cryptoPath);
                    if (service.IsAvailable())
                    {
                        cryptoService = service;
                        // CryptoSoft found and wired.
                    }
                }
            }
            catch
            {
                // Keep crypto optional - continue without it if unavailable
            }

            if (cryptoService == null)
            {
                // Minimal diagnostic to avoid silent 'EncryptionTime=0' when CryptoSoft isn't even found.
                Console.WriteLine("[CryptoSoft] Not available. Set EASY_SAVE_CRYPTOSOFT_PATH to the CryptoSoft executable/DLL.");
                if (!string.IsNullOrWhiteSpace(cryptoPath))
                    Console.WriteLine($"[CryptoSoft] Last candidate checked: {cryptoPath}");
            }

            // EasyLog: création via la factory
            var logger = LoggerFactory.CreateLogger(ProSoft.EasyLog.LogFormat.JSON, Path.Combine(target, "logs"));

            // Load extensions to encrypt from appsettings.json
            List<string> extensionsToEncrypt = new List<string>();
            try
            {
                var settingsService = new SettingsService();
                var settings = settingsService.GetCurrent();
                extensionsToEncrypt = settings.ExtensionsToEncrypt ?? new List<string>();
                
                if (extensionsToEncrypt.Count > 0)
                {
                    Console.WriteLine($"[CryptoSoft] Extensions to encrypt: {string.Join(", ", extensionsToEncrypt)}");
                }
            }
            catch
            {
                // If settings cannot be loaded, default to empty list (no encryption)
            }

            // Select strategy + inject crypto + logger (needed for JobEventType.Interrupted)
            IBackupStrategy strategy;
            switch (type.ToLower())
            {
                case "complete":
                    strategy = new CompleteBackupStrategy(cryptoService, logger: logger, extensionsToEncrypt: extensionsToEncrypt);
                    break;
                case "differential":
                    strategy = new DifferentialBackupStrategy(cryptoService, logger: logger, extensionsToEncrypt: extensionsToEncrypt);
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