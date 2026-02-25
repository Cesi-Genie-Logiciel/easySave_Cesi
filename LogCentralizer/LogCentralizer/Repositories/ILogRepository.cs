using LogCentralizer.Models;

namespace LogCentralizer.Repositories
{
    public interface ILogRepository
    {
        void AppendLog(string clientId, LogEntry entry);
        void SaveState(string clientId, BackupState state);
        List<string> GetLogs(string date);
    }
}
