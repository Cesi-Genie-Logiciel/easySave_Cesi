using EasySave.Interfaces;
using EasySave.Models;
using ProSoft.EasyLog;

namespace EasySave.Observers
{
    public class LoggerObserver : IBackupObserver
    {
        private readonly JsonFileLogger _logger;
        
        public LoggerObserver(JsonFileLogger logger)
        {
            _logger = logger;
        }
        
        public void OnBackupStarted(string backupName)
        {
            // Optionnel : logger le début
        }
        
        public void OnFileTransferred(BackupEventArgs e)
        {
            _logger.WriteLog(e.BackupName, e.SourceFile, e.DestFile, 
                           e.FileSize, (long)e.TransferTimeMs);
        }
        
        public void OnBackupCompleted(string backupName)
        {
            // Optionnel : logger la fin
        }
    }
}
