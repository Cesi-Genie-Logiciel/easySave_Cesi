using EasySave.Models;

namespace EasySave.Interfaces
{
    public interface IBackupStrategy
    {
        void ExecuteBackup(string sourcePath, string targetPath, Action<BackupEventArgs> onFileTransferred, string backupName);
    }
}
