using EasySave.Models;

namespace EasySave.Interfaces
{
    public interface IBackupObserver
    {
        void OnBackupStarted(string backupName);
        void OnFileTransferred(BackupEventArgs e);
        void OnBackupCompleted(string backupName);
    }
}