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
            foreach (var observer in _observers)
            {
                observer.OnBackupStarted(_name);
            }
        }
        
        private void NotifyFileTransferred(BackupEventArgs e)
        {
            foreach (var observer in _observers)
            {
                observer.OnFileTransferred(e);
            }
        }
        
        private void NotifyBackupCompleted()
        {
            foreach (var observer in _observers)
            {
                observer.OnBackupCompleted(_name);
            }
        }
    }
}
