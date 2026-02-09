using System;
using System.Collections.Generic;
using EasySave.Core.Models;
using EasySave.Core.Factory;
using EasySave.Services.Interfaces;

namespace EasySave.Services.Implementations
{
	public class BackupService : IBackupService
	{
		private List<BackupJob> _jobs = new List<BackupJob>();

		public void CreateBackupJob(string name, string source, string target, string type)
		{
			if (_jobs.Count >= 5)
			{
				throw new InvalidOperationException("Maximum 5 backup jobs allowed");
			}

			var job = BackupJobFactory.CreateBackupJob(name, source, target, type);
			_jobs.Add(job);
			Console.WriteLine($"Backup job '{name}' created successfully");
		}

		public List<BackupJob> GetAllBackupJobs()
		{
			return new List<BackupJob>(_jobs);
		}

		public void ExecuteBackupJob(int index)
		{
			if (index < 0 || index >= _jobs.Count)
			{
				throw new ArgumentOutOfRangeException($"Invalid job index: {index}");
			}

			_jobs[index].Execute();
		}

		public void ExecuteMultipleBackupJobs(List<int> indices)
		{
			foreach (var index in indices)
			{
				ExecuteBackupJob(index);
			}
		}

		public void DeleteBackupJob(int index)
		{
			if (index < 0 || index >= _jobs.Count)
			{
				throw new ArgumentOutOfRangeException($"Invalid job index: {index}");
			}

			var jobName = _jobs[index].Name;
			_jobs.RemoveAt(index);
			Console.WriteLine($"Backup job '{jobName}' deleted");
		}
	}
}
