using EasySave.Models;

namespace EasySave.Interfaces
{
    /// <summary>
    /// Interface for a backup job
    /// </summary>
    public interface IBackupJob
    {
        BackupConfig Config { get; }
        BackupStats Stats { get; }

        void SetStrategy(IBackupStrategy strategy);
        void AddObserver(IBackupObserver observer);
        void RemoveObserver(IBackupObserver observer);
        void Execute();
    }
}
