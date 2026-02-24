using EasySave.Interfaces;
using EasySave.Models;
using ProSoft.EasyLog.Interfaces;

namespace EasySave.Observers
{
    // Writes the current backup state to disk after each event,
    // so the GUI or other tools can read the real-time state file.
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

        public void OnFileTransferred(BackupEventArgs args)
        {
            _logger.UpdateStateToDisk();
        }

        public void OnBackupCompleted(string backupName)
        {
            _logger.UpdateStateToDisk();
        }

        public void OnBackupStateChanged(string backupName, BackupJobState state)
        {
            _logger.UpdateStateToDisk();
        }
    }
}