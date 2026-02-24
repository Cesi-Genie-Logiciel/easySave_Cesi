namespace LogCentralizer.Models
{
    /// <summary>
    /// Requête de log reçue du client (P4).
    /// </summary>
    public class LogRequest
    {
        public string ClientId { get; set; } = string.Empty;
        public string MachineName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public LogEntry Entry { get; set; } = new LogEntry();
    }
}
