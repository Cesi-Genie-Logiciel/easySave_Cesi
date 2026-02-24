using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EasySave.Interfaces;

namespace EasySave.Models
{
    public class BackupJob
    {
        private string _name;
        private string _sourcePath;
        private string _targetPath;
        private string _backupType;
        private IBackupStrategy _strategy;
        private BackupJobState _state;
        private int _progress;

        // These two primitives are the core of pause/stop.
        // CancellationTokenSource: calling Cancel() signals all strategies to stop.
        // ManualResetEventSlim: calling Reset() blocks strategies at their next checkpoint = pause.
        //                       calling Set() unblocks them = resume.
        private CancellationTokenSource _cts;
        private ManualResetEventSlim _pauseEvent;

        private List<IBackupObserver> _observers = new List<IBackupObserver>();

        public event EventHandler<BackupEventArgs>? FileTransferred;
        public event EventHandler? BackupStarted;
        public event EventHandler? BackupCompleted;
        public event EventHandler<BackupJobState>? StateChanged;

        public string Name => _name;
        public string SourcePath => _sourcePath;
        public string TargetPath => _targetPath;
        public string BackupType => _backupType;

        public BackupJobState State
        {
            get => _state;
            private set
            {
                if (_state == value) return;
                _state = value;
                NotifyStateChanged(value);
            }
        }

        public int Progress
        {
            get => _progress;
            private set => _progress = value;
        }

        public BackupJob(string name, string source, string target, string backupType, IBackupStrategy strategy)
        {
            _name = name;
            _sourcePath = source;
            _targetPath = target;
            _backupType = backupType;
            _strategy = strategy;
            _state = BackupJobState.Pending;

            // Both start in their "running" state:
            // CTS is fresh (not cancelled), PauseEvent is signaled (not paused)
            _cts = new CancellationTokenSource();
            _pauseEvent = new ManualResetEventSlim(true);
        }

        // Async execution that respects pause/stop through the context.
        // The strategy checks context.PauseEvent.Wait() and context.Token between each file.
        public async Task Execute(BackupExecutionContext context)
        {
            if (_state == BackupJobState.Running) return;

            // If the job was already run before, reset the primitives for a fresh start
            if (_state == BackupJobState.Stopped || _state == BackupJobState.Completed || _state == BackupJobState.Error)
            {
                _cts = new CancellationTokenSource();
                _pauseEvent = new ManualResetEventSlim(true);
            }

            State = BackupJobState.Running;
            Progress = 0;
            NotifyBackupStarted();

            try
            {
                // Build a context that carries this job's own pause/stop controls.
                // The strategy only sees the context, it does not know about BackupJob directly.
                BackupExecutionContext jobContext = new BackupExecutionContext(
                    _cts.Token,
                    _pauseEvent,
                    context.ExtensionsToEncrypt);

                // Wire up the notification callback so the strategy can report progress
                if (_strategy is Strategies.CompleteBackupStrategy complete)
                    complete.SetNotificationCallback(NotifyFileTransferred, _name);
                else if (_strategy is Strategies.DifferentialBackupStrategy diff)
                    diff.SetNotificationCallback(NotifyFileTransferred, _name);

                await _strategy.ExecuteBackup(_sourcePath, _targetPath, jobContext);

                // If we reach here without cancellation, the backup finished normally
                if (_state == BackupJobState.Running)
                {
                    Progress = 100;
                    State = BackupJobState.Completed;
                }
            }
            catch (OperationCanceledException)
            {
                // Expected when Stop() was called
                State = BackupJobState.Stopped;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  Error during backup '{_name}': {ex.Message}");
                State = BackupJobState.Error;
            }

            NotifyBackupCompleted();
        }

        // Backward compatibility: synchronous execution for CLI mode
        public void Execute()
        {
            BackupExecutionContext simpleContext = new BackupExecutionContext();
            Execute(simpleContext).GetAwaiter().GetResult();
        }

        // Pauses the job after the current file finishes copying.
        // Reset() puts the ManualResetEventSlim in non-signaled state,
        // so the next time the strategy calls Wait(), it will block.
        public void Pause()
        {
            if (_state != BackupJobState.Running) return;
            _pauseEvent.Reset();
            State = BackupJobState.Paused;
        }

        // Resumes a paused job.
        // Set() puts the ManualResetEventSlim back in signaled state,
        // unblocking the strategy thread that was waiting.
        public void Resume()
        {
            if (_state != BackupJobState.Paused) return;
            State = BackupJobState.Running;
            _pauseEvent.Set();
        }

        // Stops the job immediately by cancelling the token.
        // If the job is paused, we also signal the pause event
        // so the strategy wakes up and sees the cancellation.
        public void Stop()
        {
            if (_state != BackupJobState.Running && _state != BackupJobState.Paused) return;
            _cts.Cancel();
            if (_state == BackupJobState.Paused)
                _pauseEvent.Set();
            State = BackupJobState.Stopped;
        }

        public void AddObserver(IBackupObserver observer) => _observers.Add(observer);
        public void RemoveObserver(IBackupObserver observer) => _observers.Remove(observer);

        private void NotifyBackupStarted()
        {
            foreach (IBackupObserver obs in _observers)
                obs.OnBackupStarted(_name);
            BackupStarted?.Invoke(this, EventArgs.Empty);
        }

        public void NotifyFileTransferred(BackupEventArgs args)
        {
            Progress = args.Progress;
            foreach (IBackupObserver obs in _observers)
                obs.OnFileTransferred(args);
            FileTransferred?.Invoke(this, args);
        }

        private void NotifyBackupCompleted()
        {
            foreach (IBackupObserver obs in _observers)
                obs.OnBackupCompleted(_name);
            BackupCompleted?.Invoke(this, EventArgs.Empty);
        }

        private void NotifyStateChanged(BackupJobState state)
        {
            foreach (IBackupObserver obs in _observers)
                obs.OnBackupStateChanged(_name, state);
            StateChanged?.Invoke(this, state);
        }
    }
}