
using EasySave.Interfaces;

namespace EasySave.Models
{
    /// <summary>
    /// Concrete implementation of a backup job.
    /// Holds configuration, statistics and orchestrates strategy + observers.
    /// </summary>
    public class BackupJob : IBackupJob
    {
        private IBackupStrategy _strategy;
        private readonly List<IBackupObserver> _observers = new();

        public BackupConfig Config { get; }
        public BackupStats Stats { get; }

        public BackupJob(BackupConfig config)
        {
            Config = config;
            Stats = new BackupStats
            {
                Name = config.Name
            };
        }

        public void SetStrategy(IBackupStrategy strategy)
        {
            _strategy = strategy;
        }

        public void AddObserver(IBackupObserver observer)
        {
            _observers.Add(observer);
        }

        public void RemoveObserver(IBackupObserver observer)
        {
            _observers.Remove(observer);
        }

        public void Execute()
        {
            if (_strategy == null)
                throw new InvalidOperationException("No backup strategy configured for this job.");

            bool success = true;

            NotifyStarted();

            try
            {
                _strategy.Execute(
                    Config,
                    Stats,
                    NotifyProgress
                );
            }
            catch
            {
                success = false;
                throw;
            }
            finally
            {
                NotifyCompleted(success);
            }
        }

        private void NotifyStarted()
        {
            foreach (var observer in _observers)
            {
                observer.OnBackupStarted(Config.Name);
            }
        }

        private void NotifyProgress(BackupEventArgs e)
        {
            // Update stats from event
            Stats.TotalFiles = e.TotalFiles;
            Stats.FilesRemaining = e.TotalFiles - e.ProcessedFiles;
            Stats.TotalSize += e.FileSize;
            Stats.SizeRemaining = Stats.TotalSize - (long)(e.TransferTimeMs >= 0 ? e.FileSize : 0); // simple approximation
            Stats.CurrentSourceFile = e.SourceFile;
            Stats.CurrentDestFile = e.DestFile;

            foreach (var observer in _observers)
            {
                observer.OnBackupProgress(e);
            }
        }

        private void NotifyCompleted(bool success)
        {
            Stats.State = "Completed";
            Stats.FilesRemaining = 0;
            Stats.SizeRemaining = 0;

            foreach (var observer in _observers)
            {
                observer.OnBackupCompleted(Config.Name, success);
            }
        }
    }
}