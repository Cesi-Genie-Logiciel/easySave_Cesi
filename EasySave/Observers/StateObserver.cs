using System.Text.Json;
using EasySave.Interfaces;
using EasySave.Models;

namespace EasySave.Observers
{
    /// <summary>
    /// Observer that writes backup state snapshots to JSON files.
    /// </summary>
    public class StateObserver : IBackupObserver
    {
        private readonly string _stateDirectory;

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

            WriteState(state);
        }

        public void OnBackupProgress(BackupEventArgs eventArgs)
        {
            var state = new BackupState
            {
                Name = eventArgs.BackupName,
                Timestamp = DateTime.Now,
                State = "Active",
                TotalFiles = eventArgs.TotalFiles,
                FilesRemaining = eventArgs.TotalFiles - eventArgs.ProcessedFiles,
                CurrentSourceFile = eventArgs.SourceFile,
                CurrentDestFile = eventArgs.DestFile,
                Progress = eventArgs.Progress
            };

            WriteState(state);
        }

        public void OnBackupCompleted(string backupName, bool success)
        {
            var state = new BackupState
            {
                Name = backupName,
                Timestamp = DateTime.Now,
                State = success ? "Completed" : "Inactive",
                Progress = 100
            };

            WriteState(state);
        }

        private void WriteState(BackupState state)
        {
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