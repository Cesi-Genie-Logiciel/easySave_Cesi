using EasySave.Models;

namespace EasySave.Interfaces
{
    /// <summary>
    /// Interface for observers that react to backup progress
    /// </summary>
    public interface IBackupObserver
    {
        /// <summary>
        /// Called when backup progress is made
        /// </summary>
        void OnBackupProgress(BackupEventArgs eventArgs);

        /// <summary>
        /// Called when backup starts
        /// </summary>
        void OnBackupStarted(string backupName);

        /// <summary>
        /// Called when backup completes
        /// </summary>
        void OnBackupCompleted(string backupName, bool success);
    }
}
