using EasySave.Core.Models;

namespace EasySave.Core.Observers
{
	public interface IBackupObserver
	{
		void OnBackupStarted(string backupName);
		void OnFileTransferred(BackupEventArgs e);
		void OnBackupCompleted(string backupName);
	}
}
