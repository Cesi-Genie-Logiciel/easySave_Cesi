using LogCentralizer.Models;

namespace LogCentralizer.Repositories
{
    /// <summary>
    /// Interface pour le repository de logs (P4).
    /// </summary>
    public interface ILogRepository
    {
        void AppendLog(string clientId, LogEntry entry);
        List<string> GetLogs(string date);
    }
}
