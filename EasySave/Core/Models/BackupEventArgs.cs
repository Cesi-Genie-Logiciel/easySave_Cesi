namespace EasySave.Core.Models
{
	public class BackupEventArgs
	{
		public string BackupName { get; set; }
		public string SourceFile { get; set; }
		public string DestFile { get; set; }
		public long FileSize { get; set; }
		public double TransferTimeMs { get; set; }
		public int TotalFiles { get; set; }
		public int ProcessedFiles { get; set; }
		public int Progress { get; set; } // Pourcentage
	}
}
