namespace EasySave.Models
{
    /// <summary>
    /// Configuration for a backup job
    /// </summary>
    public class BackupConfig
    {
        public string Name { get; set; } = string.Empty;
        public string SourcePath { get; set; } = string.Empty;
        public string TargetPath { get; set; } = string.Empty;
        public string BackupType { get; set; } = "Complete"; // "Complete" or "Differential"

        public BackupConfig() { }

        public BackupConfig(string name, string sourcePath, string targetPath, string backupType)
        {
            Name = name;
            SourcePath = sourcePath;
            TargetPath = targetPath;
            BackupType = backupType;
        }
    }
}
