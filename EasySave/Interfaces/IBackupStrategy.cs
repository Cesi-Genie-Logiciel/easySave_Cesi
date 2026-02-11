namespace EasySave.Interfaces
{
    public interface IBackupStrategy
    {
        void ExecuteBackup(string sourcePath, string targetPath);
    }
}
