namespace EasySave.Models
{
    // backup job configuration class for JSON serialization
    public class BackupConfig
    {
        public string Name { get; set; } = "";
        public string SourcePath { get; set; } = "";
        public string TargetPath { get; set; } = "";
        public string BackupType { get; set; } = "complete";

        public BackupConfig() { }

        public BackupConfig(string name, string source, string target, string type)
        {
            Name = name;
            SourcePath = source;
            TargetPath = target;
            BackupType = type;
        }
    }
}