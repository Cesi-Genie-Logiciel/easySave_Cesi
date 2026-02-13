using EasySave.Interfaces;
using EasySave.Models;
using EasySave.Strategies;
using EasySave.Observers;
using EasySave.Services;

namespace EasySave.Factories
{
    /// <summary>
    /// Factory responsible for creating fully configured BackupJob instances
    /// (strategy + observers) from a BackupConfig.
    /// </summary>
    public static class BackupJobFactory
    {
        public static BackupJob Create(BackupConfig config, IEnumerable<IBackupObserver> observers)
        {
            // CryptoSoft integration (branch 1): try to wire the external tool if present.
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

            // Choose strategy based on BackupType
            IBackupStrategy strategy = config.BackupType.ToLower() switch
            {
                "complete" => new CompleteBackupStrategy(cryptoService),
                "differential" => new DifferentialBackupStrategy(cryptoService),
                _ => new CompleteBackupStrategy(cryptoService)
            };

            var job = new BackupJob(config);
            job.SetStrategy(strategy);

            // Attach all observers to the job
            foreach (var observer in observers)
            {
                job.AddObserver(observer);
            }

            return job;
        }
    }
}
