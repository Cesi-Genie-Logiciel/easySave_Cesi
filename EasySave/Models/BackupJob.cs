using System;
using System.Collections.Generic;
using EasySave.Interfaces;
using EasySave.Strategies;

namespace EasySave.Models
{
    public class BackupJob
    {
        private string _name;
        private string _sourcePath;
        private string _targetPath;
        private string _backupType;
        private IBackupStrategy _strategy;
        private List<IBackupObserver> _observers = new List<IBackupObserver>();

        public event EventHandler<BackupEventArgs>? FileTransferred;
        public event EventHandler? BackupStarted;
        public event EventHandler? BackupCompleted;

        public string Name => _name;
        public string SourcePath => _sourcePath;
        public string TargetPath => _targetPath;
        public string BackupType => _backupType;

        public BackupJob(string name, string source, string target, string backupType, IBackupStrategy strategy)
        {
            _name = name;
            _sourcePath = source;
            _targetPath = target;
            _backupType = backupType;
            _strategy = strategy;
        }

        public void Execute()
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Demarrage : {_name}");
            NotifyBackupStarted();

            try
            {
                if (_strategy is CompleteBackupStrategy complete)
                    complete.SetNotificationCallback(NotifyFileTransferred, _name);
                else if (_strategy is DifferentialBackupStrategy diff)
                    diff.SetNotificationCallback(NotifyFileTransferred, _name);

                _strategy.ExecuteBackup(_sourcePath, _targetPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur pendant la sauvegarde : {ex.Message}");
            }

            NotifyBackupCompleted();
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Termine : {_name}\n");
        }

        /// <summary>
        /// V3/P3: Pause/Resume/Stop seront implémentés dans le livrable 3 (P3).
        /// Stub minimal pour conserver la compatibilité de l'API et permettre la compilation.
        /// </summary>
        public void Pause()
        {
            // TODO(v3/P3): implémenter une vraie pause via contexte d'exécution + synchro UI.
        }

        /// <summary>
        /// V3/P3: Stub minimal. Voir TODO dans <see cref="Pause"/>.
        /// </summary>
        public void Stop()
        {
            // TODO(v3/P3): implémenter un vrai stop via CancellationToken.
        }

        public void AddObserver(IBackupObserver observer) => _observers.Add(observer);
        public void RemoveObserver(IBackupObserver observer) => _observers.Remove(observer);

        private void NotifyBackupStarted()
        {
            foreach (var obs in _observers)
                obs.OnBackupStarted(_name);
            BackupStarted?.Invoke(this, EventArgs.Empty);
        }

        private void NotifyFileTransferred(BackupEventArgs e)
        {
            foreach (var obs in _observers)
                obs.OnFileTransferred(e);
            FileTransferred?.Invoke(this, e);
        }

        private void NotifyBackupCompleted()
        {
            foreach (var obs in _observers)
                obs.OnBackupCompleted(_name);
            BackupCompleted?.Invoke(this, EventArgs.Empty);
        }
    }
}