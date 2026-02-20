using System;

namespace EasySave.Models
{
    // state class to track backup progress for JSON serialization
    public class BackupState
    {
        public string Name { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string State { get; set; } = "Inactive";
        public int TotalFiles { get; set; }
        public int FilesRemaining { get; set; }
        public long TotalSize { get; set; }
        public long SizeRemaining { get; set; }
        public string CurrentSourceFile { get; set; } = string.Empty;
        public string CurrentDestFile { get; set; } = string.Empty;
        public int Progress { get; set; }
    }
}