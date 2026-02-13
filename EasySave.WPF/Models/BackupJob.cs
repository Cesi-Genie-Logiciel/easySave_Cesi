namespace EasySave.Models
{
    /// Represents a backup job with its configuration and state.
    /// Simple data model without business logic (MVVM Model layer).
    public class BackupJob
    {
        public string Name { get; set; } = string.Empty;
        public string SourcePath { get; set; } = string.Empty;
        public string TargetPath { get; set; } = string.Empty;
        public string BackupType { get; set; } = "Complete";
        public int Progress { get; set; }
        public string State { get; set; } = "Stopped";
    }
}
