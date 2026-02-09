namespace EasySave.Core.Strategies
{
	public interface IBackupStrategy
	{
		void ExecuteBackup(string sourcePath, string targetPath);
	}
}
