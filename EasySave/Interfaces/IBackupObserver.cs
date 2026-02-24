using EasySave.Models;

namespace EasySave.Interfaces
{
    // Notified by BackupJob when something happens during execution.
    // Each observer decides what to do: log to disk, update the console, write state file, etc.
    public interface IBackupObserver
    {
        void OnBackupStarted(string backupName);
        void OnFileTransferred(BackupEventArgs args);
        void OnBackupCompleted(string backupName);
        void OnBackupStateChanged(string backupName, BackupJobState state);
    }
}