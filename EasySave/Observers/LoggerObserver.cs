using EasySave.Interfaces;
using EasySave.Models;
using ProSoft.EasyLog;

namespace EasySave.Observers
{
    /// <summary>
    /// Observer that writes backup events to log files using EasyLog.dll
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
            // Optional: log start event (not required by spec)
        }

        public void OnBackupProgress(BackupEventArgs eventArgs)
        {
            // Write each file transfer to EasyLog
            _logger.WriteLog(
                backupName: eventArgs.BackupName,
                sourceFilePath: eventArgs.SourceFile,
                targetFilePath: eventArgs.DestFile,
                fileSize: eventArgs.FileSize,
                transferTime: eventArgs.TransferTime
            );
        }

        public void OnBackupCompleted(string backupName, bool success)
        {
            // Optional: log completion event (not required by spec)
        }
    }
}
