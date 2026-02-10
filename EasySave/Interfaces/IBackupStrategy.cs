using EasySave.Models;

namespace EasySave.Interfaces
{
    /// <summary>
    /// Interface for backup strategies (Complete, Differential)
    /// Allows adding new strategies without modifying existing code
    /// </summary>
    public interface IBackupStrategy
    {
        /// <summary>
        /// Execute the backup strategy
        /// </summary>
        void Execute(
            BackupConfig config,
            BackupStats stats,
            Action<BackupEventArgs> notifyProgress
        );
    }
}
