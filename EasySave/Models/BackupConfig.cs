namespace EasySave.Models
{
    /// <summary>
    /// Configuration for a backup job (paths + type).
    /// </summary>
    public class BackupConfig
    {
        public string Name { get; set; }
        public string SourcePath { get; set; }
        public string TargetPath { get; set; }

        /// <summary>
        /// Backup type: "Complete" or "Differential".
        /// </summary>
        public string BackupType { get; set; }

        public BackupConfig(string name, string source, string target, string type)
        {
            Name = name;
            SourcePath = source;
            TargetPath = target;
            BackupType = type;
        }
    }
}
