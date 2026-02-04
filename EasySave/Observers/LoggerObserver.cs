using EasySave.Interfaces;
using EasySave.Models;
using ProSoft.EasyLog;

namespace EasySave.Observers
{
    /// <summary>
    /// Observer that writes backup events to log files using EasyLog.dll
    /// Only logs actual file transfers (OnBackupProgress), not start/end events
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
            // Ne RIEN logger au démarrage
        }

        public void OnBackupProgress(BackupEventArgs eventArgs)
        {
            // Vérification : Ne logger QUE si les données sont complètes
            if (string.IsNullOrWhiteSpace(eventArgs.SourceFile) ||
                string.IsNullOrWhiteSpace(eventArgs.BackupName))
            {
                return;
            }

            // Logger UNIQUEMENT les vrais transferts de fichiers
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
            // Ne RIEN logger à la fin
        }
    }
}
