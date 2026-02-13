using EasySave.Interfaces;
using EasySave.Models;
using ProSoft.EasyLog.Implementation;
using ProSoft.EasyLog.Interfaces;
using System;
using System.Collections.Generic;

namespace EasySave.Observers
{
    public class StateObserver : IBackupObserver
    {
        private readonly ILogger _logger;
        private readonly Dictionary<string, BackupState> _lastStates = new Dictionary<string, BackupState>();

        public StateObserver(ILogger logger)
        {
            _logger = logger;
        }

        public void OnBackupStarted(string backupName)
        {
            var state = new BackupState
            {
                Name = backupName,
                Timestamp = DateTime.Now,
                State = "Active",
                Progress = 0
            };
            _lastStates[backupName] = state;

            // Utiliser la méthode UpdateState disponible dans ILogger
            _logger.UpdateStateToDisk();
        }

        public void OnFileTransferred(BackupEventArgs e)
        {
            var state = new BackupState
            {
                Name = e.BackupName,
                Timestamp = DateTime.Now,
                State = "Active",
                TotalFiles = e.TotalFiles,
                FilesRemaining = e.TotalFiles - e.ProcessedFiles,
                TotalSize = e.FileSize * e.TotalFiles,
                SizeRemaining = e.FileSize * (e.TotalFiles - e.ProcessedFiles),
                CurrentSourceFile = e.SourceFile,
                CurrentDestFile = e.DestFile,
                Progress = e.Progress
            };
            _lastStates[e.BackupName] = state;

            // Utiliser la méthode UpdateState disponible dans ILogger
            _logger.UpdateStateToDisk();
        }

        public void OnBackupCompleted(string backupName)
        {
            BackupState state;

            // Récupérer le dernier état connu pour conserver les données
            if (_lastStates.TryGetValue(backupName, out var lastState))
            {
                state = new BackupState
                {
                    Name = backupName,
                    Timestamp = DateTime.Now,
                    State = "Inactive",
                    TotalFiles = lastState.TotalFiles,
                    FilesRemaining = 0,
                    TotalSize = lastState.TotalSize,
                    SizeRemaining = 0,
                    CurrentSourceFile = lastState.CurrentSourceFile,
                    CurrentDestFile = lastState.CurrentDestFile,
                    Progress = 100
                };
            }
            else
            {
                // Fallback si pas d'état précédent
                state = new BackupState
                {
                    Name = backupName,
                    Timestamp = DateTime.Now,
                    State = "Inactive",
                    Progress = 100
                };
            }

            _lastStates[backupName] = state;

            // Utiliser la méthode UpdateState disponible dans ILogger
            _logger.UpdateStateToDisk();
        }
    }
}
