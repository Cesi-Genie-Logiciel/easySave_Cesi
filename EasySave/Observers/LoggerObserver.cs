using ProSoft.EasyLog.Interfaces;
using EasySave.Interfaces;
using EasySave.Models;
using System;

namespace EasySave.Observers
{
    // Logs file transfer events to the daily log file through the EasyLog DLL.
    // State changes are not logged here (that is the StateObserver's job).
    public class LoggerObserver : IBackupObserver
    {
        private readonly ILogger _logger;

        public LoggerObserver(ILogger logger)
        {
            _logger = logger;
        }

        public void OnBackupStarted(string backupName) { }

        public void OnFileTransferred(BackupEventArgs args)
        {
            _logger.LogFileTransfer(
                args.BackupName,
                args.SourceFile,
                args.DestFile,
                args.FileSize,
                Convert.ToInt64(args.TransferTimeMs),
                args.EncryptionTimeMs);
        }

        public void OnBackupCompleted(string backupName) { }

        public void OnBackupStateChanged(string backupName, BackupJobState state) { }
    }
}