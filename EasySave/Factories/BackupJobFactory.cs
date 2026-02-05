using EasySave.Interfaces;
using EasySave.Models;
using EasySave.Strategies;
using EasySave.Observers;

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
            // Choose strategy based on BackupType
            IBackupStrategy strategy = config.BackupType.ToLower() switch
            {
                "complete" => new CompleteBackupStrategy(),
                "differential" => new DifferentialBackupStrategy(),
                _ => new CompleteBackupStrategy()
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
