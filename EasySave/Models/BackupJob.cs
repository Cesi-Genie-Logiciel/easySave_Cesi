using System;
using System.Collections.Generic;
using System.Threading;
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

        // Token source that lets us cancel the file copy loop from outside
        private CancellationTokenSource? _cancellationTokenSource;

        // Gate that blocks the copy loop when the job is paused.
        // Starts in the signaled state (open) so the job runs freely until Pause() is called.
        private ManualResetEventSlim _pauseEvent = new ManualResetEventSlim(true);

        public event EventHandler<BackupEventArgs>? FileTransferred;
        public event EventHandler? BackupStarted;
        public event EventHandler? BackupCompleted;

        public string Name => _name;
        public string SourcePath => _sourcePath;
        public string TargetPath => _targetPath;
        public string BackupType => _backupType;

        // Expose these so the strategies can check them between each file
        public CancellationToken CancellationToken => _cancellationTokenSource?.Token ?? CancellationToken.None;
        public ManualResetEventSlim PauseEvent => _pauseEvent;

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
            // Create a fresh token source each time the job starts,
            // so the same job can be stopped and restarted later
            _cancellationTokenSource = new CancellationTokenSource();
            _pauseEvent.Set();

            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Demarrage : {_name}");
            NotifyBackupStarted();

            try
            {
                if (_strategy is CompleteBackupStrategy complete)
                {
                    complete.SetParentJob(this);
                    complete.SetNotificationCallback(NotifyFileTransferred, _name);
                }
                else if (_strategy is DifferentialBackupStrategy diff)
                {
                    diff.SetParentJob(this);
                    diff.SetNotificationCallback(NotifyFileTransferred, _name);
                }

                _strategy.ExecuteBackup(_sourcePath, _targetPath);
            }
            catch (OperationCanceledException)
            {
                // The user clicked Stop, this is expected behavior
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Arrete par l'utilisateur : {_name}");
                NotifyStateChanged(BackupJobState.Stopped);
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur pendant la sauvegarde : {ex.Message}");
            }

            NotifyBackupCompleted();
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Termine : {_name}\n");
        }

        // Closes the gate so the copy loop blocks at the next file boundary
        public void Pause()
        {
            _pauseEvent.Reset();
            NotifyStateChanged(BackupJobState.Paused);
        }

        // Opens the gate so the blocked thread can continue
        public void Resume()
        {
            _pauseEvent.Set();
            NotifyStateChanged(BackupJobState.Running);
        }

        // Signals the cancellation token and opens the pause gate
        // so the thread is not stuck waiting while we try to cancel it
        public void Stop()
        {
            _cancellationTokenSource?.Cancel();
            _pauseEvent.Set();
            NotifyStateChanged(BackupJobState.Stopped);
        }

        public void AddObserver(IBackupObserver observer) => _observers.Add(observer);
        public void RemoveObserver(IBackupObserver observer) => _observers.Remove(observer);

        private void NotifyBackupStarted()
        {
            NotifyStateChanged(BackupJobState.Running);
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
            NotifyStateChanged(BackupJobState.Completed);
            BackupCompleted?.Invoke(this, EventArgs.Empty);
        }

        private void NotifyStateChanged(BackupJobState state)
        {
            foreach (var obs in _observers)
                obs.OnBackupStateChanged(_name, state);
        }
    }
}