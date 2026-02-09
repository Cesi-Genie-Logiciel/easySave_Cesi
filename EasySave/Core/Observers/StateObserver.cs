
using System;
using EasyLog.Interfaces;
using EasyLog.Implementation;
using EasySave.Core.Models;

namespace EasySave.Core.Observers
{
	public class StateObserver : IBackupObserver
	{
		private ILogger _logger;

		public StateObserver(ILogger logger)
		{
			_logger = logger;
		}

		public void OnBackupStarted(string backupName)
		{
			var state = new BackupState
			{
				Name = backupName,
				Timestamp = DateTime.Now,
				State = "Active",
				Progress = 0
			};
			_logger.UpdateState(state);
		}

		public void OnFileTransferred(BackupEventArgs e)
		{
			var state = new BackupState
			{
				Name = e.BackupName,
				Timestamp = DateTime.Now,
				State = "Active",
				TotalFiles = e.TotalFiles,
				FilesRemaining = e.TotalFiles - e.ProcessedFiles,
				CurrentSourceFile = e.SourceFile,
				CurrentDestFile = e.DestFile,
				Progress = e.Progress
			};
			_logger.UpdateState(state);
		}

		public void OnBackupCompleted(string backupName)
		{
			var state = new BackupState
			{
				Name = backupName,
				Timestamp = DateTime.Now,
				State = "Inactive",
				Progress = 100
			};
			_logger.UpdateState(state);
		}
	}
}