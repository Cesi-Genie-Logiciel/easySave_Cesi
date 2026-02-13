namespace EasySave.Models
{
    /// <summary>
    /// Configuration for a backup job (paths + type).
    /// Sert de DTO pour la persistance des jobs
    /// </summary>
    public class BackupConfig
    {
        public string Name { get; set; } = "";
        public string SourcePath { get; set; } = "";
        public string TargetPath { get; set; } = "";

        /// <summary>
        /// Backup type: "Complete" or "Differential".
        /// </summary>
        public string BackupType { get; set; } = "complete";

        /// <summary>
        /// Constructeur par défaut (requis pour la désérialisation JSON)
        /// </summary>
        public BackupConfig()
        {
        }

        /// <summary>
        /// Constructeur avec paramètres
        /// </summary>
        public BackupConfig(string name, string source, string target, string type)
        {
            Name = name;
            SourcePath = source;
            TargetPath = target;
            BackupType = type;
        }
    }
}
