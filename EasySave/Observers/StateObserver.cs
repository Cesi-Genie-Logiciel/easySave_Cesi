using EasySave.Interfaces;
using EasySave.Models;
using ProSoft.EasyLog.Interfaces;

namespace EasySave.Observers
{
    public class StateObserver : IBackupObserver
    {
        private readonly ILogger _logger;

        public StateObserver(ILogger logger)
        {
            _logger = logger;
        }

        public void OnBackupStarted(string backupName)
        {
            _logger.UpdateStateToDisk();
        }

        public void OnFileTransferred(BackupEventArgs e)
        {
            _logger.UpdateStateToDisk();
        }

        public void OnBackupCompleted(string backupName)
        {
            _logger.UpdateStateToDisk();
        }
    }
}