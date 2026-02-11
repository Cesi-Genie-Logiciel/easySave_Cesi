using EasySave.Interfaces;
using EasySave.Models;
using ProSoft.EasyLog;

namespace EasySave.Observers
{
    /// <summary>
    /// Observer that writes detailed file transfer logs using the EasyLog library.
    /// </summary>
    public class LoggerObserver : IBackupObserver
    {
        private readonly JsonFileLogger _logger;

        public LoggerObserver(string logDirectory)
        {
            _logger = new JsonFileLogger(logDirectory);
        }

        public void OnBackupStarted(string backupName)
        {
            // No-op: logging is done per file transfer.
        }

        public void OnBackupProgress(BackupEventArgs eventArgs)
        {
            // Only log transfers where we actually copied something (size > 0)
            if (eventArgs.FileSize <= 0)
                return;

            _logger.WriteLog(
                eventArgs.BackupName,
                eventArgs.SourceFile,
                eventArgs.DestFile,
                eventArgs.FileSize,
                (long)eventArgs.TransferTimeMs
            );
        }

        public void OnBackupCompleted(string backupName, bool success)
        {
            // No specific end-of-backup log entry required here.
        }
    }
}