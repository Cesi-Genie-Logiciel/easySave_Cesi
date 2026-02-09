using EasyLog.Interfaces;
using EasyLog.Implementation;
using EasySave.Core.Models;

namespace EasySave.Core.Observers
{
	public class LoggerObserver : IBackupObserver
	{
		private ILogger _logger;

		public LoggerObserver(ILogger logger)
		{
			_logger = logger;
		}

		public void OnBackupStarted(string backupName)
		{
			// Optionnel : logger le début
		}

		public void OnFileTransferred(BackupEventArgs e)
		{
			_logger.LogFileTransfer(e.BackupName, e.SourceFile, e.DestFile,
								   e.FileSize, e.TransferTimeMs);
		}

		public void OnBackupCompleted(string backupName)
		{
			// Optionnel : logger la fin
		}
	}
}