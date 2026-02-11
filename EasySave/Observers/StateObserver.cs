using System;
using System.IO;
using System.Text.Json;
using EasySave.Interfaces;
using EasySave.Models;

namespace EasySave.Observers
{
    public class StateObserver : IBackupObserver
    {
        private readonly string _stateDirectory;
        private readonly Dictionary<string, BackupState> _lastStates = new Dictionary<string, BackupState>();
        
        public StateObserver(string stateDirectory)
        {
            _stateDirectory = stateDirectory;
            Directory.CreateDirectory(_stateDirectory);
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
            WriteState(state);
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
                TotalSize = e.FileSize * e.TotalFiles, // Approximation
                SizeRemaining = e.FileSize * (e.TotalFiles - e.ProcessedFiles),
                CurrentSourceFile = e.SourceFile,
                CurrentDestFile = e.DestFile,
                Progress = e.Progress
            };
            _lastStates[e.BackupName] = state;
            WriteState(state);
        }
        
        public void OnBackupCompleted(string backupName)
        {
            // Récupérer le dernier état pour garder les statistiques
            BackupState state;
            if (_lastStates.ContainsKey(backupName))
            {
                state = _lastStates[backupName];
                state.State = "Inactive";
                state.FilesRemaining = 0;
                state.SizeRemaining = 0;
                state.Progress = 100;
                state.Timestamp = DateTime.Now;
            }
            else
            {
                state = new BackupState
                {
                    Name = backupName,
                    Timestamp = DateTime.Now,
                    State = "Inactive",
                    Progress = 100
                };
            }
            WriteState(state);
        }
        
        private void WriteState(BackupState state)
        {
            // Assurer que le dossier existe
            if (!Directory.Exists(_stateDirectory))
            {
                Directory.CreateDirectory(_stateDirectory);
            }
            
            string fileName = $"{state.Name}_state.json";
            foreach (var c in Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(c, '_');
            }
            
            string path = Path.Combine(_stateDirectory, fileName);
            
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            
            File.WriteAllText(path, JsonSerializer.Serialize(state, options));
        }
    }
}
