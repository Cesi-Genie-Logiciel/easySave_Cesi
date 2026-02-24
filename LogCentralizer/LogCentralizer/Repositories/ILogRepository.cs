using LogCentralizer.Models;

namespace LogCentralizer.Repositories
{
    public interface ILogRepository
    {
        void AppendLog(string clientId, LogEntry entry);
        List<string> GetLogs(string date);
    }
}
