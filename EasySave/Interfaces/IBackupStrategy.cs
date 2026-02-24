using System.Threading.Tasks;
using EasySave.Models;

namespace EasySave.Interfaces
{
    // Defines how a backup is performed (complete vs differential).
    // Receives a BackupExecutionContext so it can check for pause/stop between each file.
    public interface IBackupStrategy
    {
        Task ExecuteBackup(string sourcePath, string targetPath, BackupExecutionContext context);
    }
}