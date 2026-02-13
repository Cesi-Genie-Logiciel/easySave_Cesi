using System;

namespace EasySave.Models
{
    /// <summary>
    /// Snapshot of a backup job state, meant to be serialized to a JSON "state" file.
    /// </summary>
    public class BackupState
    {
        public string Name { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string State { get; set; } = "Inactive"; // "Active", "Inactive", "Completed"

        public int TotalFiles { get; set; }
        public int FilesRemaining { get; set; }
        public long TotalSize { get; set; }
        public long SizeRemaining { get; set; }

        public string CurrentSourceFile { get; set; } = string.Empty;
        public string CurrentDestFile { get; set; } = string.Empty;

        /// <summary>
        /// Percentage of completion (0-100).
        /// </summary>
        public int Progress { get; set; }
    }
}
