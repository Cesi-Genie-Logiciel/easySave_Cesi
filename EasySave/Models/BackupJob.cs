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
        private IBackupStrategy _strategy;
        private List<IBackupObserver> _observers = new List<IBackupObserver>();
        
        // ✅ FEATURE P2/P3: Events pour MVVM/GUI (v2.0)
        /// <summary>
        /// Déclenché lorsqu'un fichier est transféré pendant le backup
        /// </summary>
        public event EventHandler<BackupEventArgs>? FileTransferred;
        
        /// <summary>
        /// Déclenché lorsqu'un backup démarre
        /// </summary>
        public event EventHandler? BackupStarted;
        
        /// <summary>
        /// Déclenché lorsqu'un backup se termine
        /// </summary>
        public event EventHandler? BackupCompleted;
        
        public string Name => _name;
        public string SourcePath => _sourcePath;
        public string TargetPath => _targetPath;
        
        public BackupJob(string name, string source, string target, IBackupStrategy strategy)
        {
            _name = name;
            _sourcePath = source;
            _targetPath = target;
            _strategy = strategy;
        }
        
        public void Execute()
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Starting backup: {_name}");
            NotifyBackupStarted();
            
            try
            {
                // Configurer le callback de notification pour la stratégie
                if (_strategy is Strategies.CompleteBackupStrategy completeStrategy)
                {
                    completeStrategy.SetNotificationCallback(NotifyFileTransferred, _name);
                }
                else if (_strategy is Strategies.DifferentialBackupStrategy differentialStrategy)
                {
                    differentialStrategy.SetNotificationCallback(NotifyFileTransferred, _name);
                }
                
                _strategy.ExecuteBackup(_sourcePath, _targetPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during backup: {ex.Message}");
            }
            
            NotifyBackupCompleted();
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Backup completed: {_name}\n");
        }
        
        /// <summary>
        /// Met en pause le backup en cours
        /// TODO v2.0: Implémenter la logique de pause avec CancellationToken dans les stratégies
        /// </summary>
        public void Pause()
        {
            // Stub pour v2.0 - sera implémenté par P1 avec la GUI
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Pause requested for: {_name} (not yet implemented)");
        }
        
        /// <summary>
        /// Arrête le backup en cours
        /// TODO v2.0: Implémenter la logique d'arrêt avec CancellationToken dans les stratégies
        /// </summary>
        public void Stop()
        {
            // Stub pour v2.0 - sera implémenté par P1 avec la GUI
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Stop requested for: {_name} (not yet implemented)");
        }
        
        public void AddObserver(IBackupObserver observer)
        {
            _observers.Add(observer);
        }
        
        public void RemoveObserver(IBackupObserver observer)
        {
            _observers.Remove(observer);
        }
        
        private void NotifyBackupStarted()
        {
            // Pattern Observer (v1.0)
            foreach (var observer in _observers)
            {
                observer.OnBackupStarted(_name);
            }
            
            // Events (v2.0 - pour MVVM/GUI)
            BackupStarted?.Invoke(this, EventArgs.Empty);
        }
        
        private void NotifyFileTransferred(BackupEventArgs e)
        {
            // Pattern Observer (v1.0)
            foreach (var observer in _observers)
            {
                observer.OnFileTransferred(e);
            }
            
            // Events (v2.0 - pour MVVM/GUI)
            FileTransferred?.Invoke(this, e);
        }
        
        private void NotifyBackupCompleted()
        {
            // Pattern Observer (v1.0)
            foreach (var observer in _observers)
            {
                observer.OnBackupCompleted(_name);
            }
            
            // Events (v2.0 - pour MVVM/GUI)
            BackupCompleted?.Invoke(this, EventArgs.Empty);
        }
    }
}
