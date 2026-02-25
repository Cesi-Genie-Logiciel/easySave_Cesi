using ProSoft.EasyLog.Interfaces;
using EasySave.Interfaces;
using EasySave.Models;

namespace EasySave.Observers
{
    public class LoggerObserver : IBackupObserver
    {
        private readonly ILogger _logger;

        public LoggerObserver(ILogger logger)
        {
            _logger = logger;
        }

        public void OnBackupStarted(string backupName) { }

        public void OnFileTransferred(BackupEventArgs e)
        {
            _logger.LogFileTransfer(
                e.BackupName,
                e.SourceFile,
                e.DestFile,
                e.FileSize,
                Convert.ToInt64(e.TransferTimeMs),
                e.EncryptionTimeMs);
        }

        public void OnBackupCompleted(string backupName) { }

        public void OnBackupStateChanged(string backupName, BackupJobState state) { }
    }
}