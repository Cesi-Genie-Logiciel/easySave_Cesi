using System.Text.Json;
using LogCentralizer.Models;

namespace LogCentralizer.Repositories
{
    public class FileLogRepository : ILogRepository
    {
        private readonly string _baseDirectory;
        private readonly object _lock = new object();

        public FileLogRepository(string baseDirectory)
        {
            _baseDirectory = baseDirectory;
            if (!Directory.Exists(_baseDirectory))
            {
                Directory.CreateDirectory(_baseDirectory);
            }
        }

        public void AppendLog(string clientId, LogEntry entry)
        {
            lock (_lock)
            {
                var logFileName = $"centralized_log_{DateTime.Now:yyyy-MM-dd}.json";
                var logFilePath = Path.Combine(_baseDirectory, logFileName);

                var logRecord = new
                {
                    clientId,
                    timestamp = entry.Timestamp,
                    backupName = entry.BackupName,
                    sourcePath = entry.SourcePath,
                    destPath = entry.DestPath,
                    fileSize = entry.FileSize,
                    transferTimeMs = entry.TransferTimeMs,
                    encryptionTimeMs = entry.EncryptionTimeMs,
                    eventType = entry.EventType,
                    reason = entry.Reason,
                    businessSoftware = entry.BusinessSoftware
                };

                var jsonLine = JsonSerializer.Serialize(logRecord);

                if (!File.Exists(logFilePath))
                {
                    File.WriteAllText(logFilePath, $"[{Environment.NewLine}{jsonLine}{Environment.NewLine}]");
                }
                else
                {
                    var content = File.ReadAllText(logFilePath).TrimEnd();
                    if (content.EndsWith("]"))
                    {
                        content = content.Substring(0, content.Length - 1);
                        content += $",{Environment.NewLine}{jsonLine}{Environment.NewLine}]";
                        File.WriteAllText(logFilePath, content);
                    }
                }
            }
        }

        public void SaveState(string clientId, BackupState state)
        {
            lock (_lock)
            {
                var safeClientId = string.IsNullOrEmpty(clientId) ? "default" : string.Join("_", clientId.Split(Path.GetInvalidFileNameChars()));
                var stateFileName = $"state_{safeClientId}_{DateTime.Now:yyyy-MM-dd}.json";
                var stateFilePath = Path.Combine(_baseDirectory, stateFileName);
                var json = JsonSerializer.Serialize(state, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(stateFilePath, json);
            }
        }

        public List<string> GetLogs(string date)
        {
            var logFileName = $"centralized_log_{date}.json";
            var logFilePath = Path.Combine(_baseDirectory, logFileName);

            if (!File.Exists(logFilePath))
                return new List<string>();

            return new List<string> { File.ReadAllText(logFilePath) };
        }
    }
}
