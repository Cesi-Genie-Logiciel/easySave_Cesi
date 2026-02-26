namespace LogCentralizer.Models
{
    public class StateRequest
    {
        public string ClientId { get; set; } = string.Empty;
        public BackupState State { get; set; } = new BackupState();
    }
}
