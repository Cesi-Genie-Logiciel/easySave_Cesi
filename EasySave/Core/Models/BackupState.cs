using System;

namespace EasySave.Core.Models
{
	public class BackupState
	{
		public string Name { get; set; }
		public DateTime Timestamp { get; set; }
		public string State { get; set; } // "Active", "Inactive"
		public int TotalFiles { get; set; }
		public int FilesRemaining { get; set; }
		public long TotalSize { get; set; }
		public long SizeRemaining { get; set; }
		public string CurrentSourceFile { get; set; }
		public string CurrentDestFile { get; set; }
		public int Progress { get; set; } // Pourcentage
	}
}
