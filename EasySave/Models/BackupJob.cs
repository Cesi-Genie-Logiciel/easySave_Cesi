using EasySave.Interfaces;

namespace EasySave.Models
{
    /// <summary>
    /// Represents a backup job with observers
    /// </summary>
    public class BackupJob : IBackupJob
    {
        public BackupConfig Config { get; private set; }
        public BackupStats Stats { get; private set; }

        private IBackupStrategy? _strategy;
        private readonly List<IBackupObserver> _observers;

        public BackupJob(BackupConfig config)
        {
            Config = config;
            Stats = new BackupStats { Name = config.Name };
            _observers = new List<IBackupObserver>();
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

        private void NotifyProgress(BackupEventArgs eventArgs)
        {
            foreach (var observer in _observers)
            {
                observer.OnBackupProgress(eventArgs);
            }
        }

        private void NotifyStarted()
        {
            foreach (var observer in _observers)
            {
                observer.OnBackupStarted(Config.Name);
            }
        }

        private void NotifyCompleted(bool success)
        {
            foreach (var observer in _observers)
            {
                observer.OnBackupCompleted(Config.Name, success);
            }
        }

        public void Execute()
        {
            if (_strategy == null)
                throw new InvalidOperationException("Strategy not set");

            Stats.State = "Active";
            Stats.Timestamp = DateTime.Now;

            // Notifier SEULEMENT le démarrage (pas de OnBackupProgress avec données vides)
            NotifyStarted();

            try
            {
                // La strategy va appeler NotifyProgress pour CHAQUE fichier
                _strategy.Execute(Config, Stats, NotifyProgress);

                Stats.State = "Completed";
                Stats.FilesRemaining = 0;
                Stats.SizeRemaining = 0;

                // Notifier SEULEMENT la complétion (pas de OnBackupProgress avec données vides)
                NotifyCompleted(success: true);
            }
            catch (Exception)
            {
                Stats.State = "Error";
                NotifyCompleted(success: false);
                throw;
            }
        }
    }
}
