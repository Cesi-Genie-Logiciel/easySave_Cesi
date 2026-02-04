using EasySave.Interfaces;
using EasySave.Models;
using System.Text.Json;

namespace EasySave.Observers
{
    /// <summary>
    /// Observer that writes real-time backup state to state.json
    /// </summary>
    public class StateObserver : IBackupObserver
    {
        private readonly string _stateFilePath;
        private readonly object _lockObject = new();

        public StateObserver(string stateDirectory)
        {
            Directory.CreateDirectory(stateDirectory);
            _stateFilePath = Path.Combine(stateDirectory, "state.json");
        }

        public void OnBackupStarted(string backupName)
        {
            // State will be updated on progress
        }

        public void OnBackupProgress(BackupEventArgs eventArgs)
        {
            lock (_lockObject)
            {
                // Read existing states
                List<BackupStats> states = ReadStates();

                // Update or add current backup state
                var existingState = states.FirstOrDefault(s => s.Name == eventArgs.BackupName);
                if (existingState != null)
                {
                    // Update existing
                    existingState.Timestamp = DateTime.Now;
                    existingState.State = eventArgs.Stats.State;
                    existingState.TotalFiles = eventArgs.Stats.TotalFiles;
                    existingState.FilesRemaining = eventArgs.Stats.FilesRemaining;
                    existingState.TotalSize = eventArgs.Stats.TotalSize;
                    existingState.SizeRemaining = eventArgs.Stats.SizeRemaining;
                    existingState.CurrentSourceFile = eventArgs.SourceFile;
                    existingState.CurrentDestFile = eventArgs.DestFile;
                }
                else
                {
                    // Add new
                    states.Add(new BackupStats
                    {
                        Name = eventArgs.BackupName,
                        Timestamp = DateTime.Now,
                        State = eventArgs.Stats.State,
                        TotalFiles = eventArgs.Stats.TotalFiles,
                        FilesRemaining = eventArgs.Stats.FilesRemaining,
                        TotalSize = eventArgs.Stats.TotalSize,
                        SizeRemaining = eventArgs.Stats.SizeRemaining,
                        CurrentSourceFile = eventArgs.SourceFile,
                        CurrentDestFile = eventArgs.DestFile
                    });
                }

                // Write back
                WriteStates(states);
            }
        }

        public void OnBackupCompleted(string backupName, bool success)
        {
            lock (_lockObject)
            {
                var states = ReadStates();
                var state = states.FirstOrDefault(s => s.Name == backupName);

                if (state != null)
                {
                    state.State = success ? "Completed" : "Error";
                    state.FilesRemaining = 0;
                    state.SizeRemaining = 0;
                    state.Timestamp = DateTime.Now;
                    WriteStates(states);
                }
            }
        }

        private List<BackupStats> ReadStates()
        {
            if (!File.Exists(_stateFilePath))
                return new List<BackupStats>();

            try
            {
                string json = File.ReadAllText(_stateFilePath);
                return JsonSerializer.Deserialize<List<BackupStats>>(json)
                    ?? new List<BackupStats>();
            }
            catch
            {
                return new List<BackupStats>();
            }
        }

        private void WriteStates(List<BackupStats> states)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            string json = JsonSerializer.Serialize(states, options);
            File.WriteAllText(_stateFilePath, json);
        }
    }
}
