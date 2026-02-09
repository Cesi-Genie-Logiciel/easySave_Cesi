using System;
using System.Collections.Generic;
using System.IO;
using EasySave.Core.Models;
using EasySave.Core.Strategies;
using EasySave.Core.Observers;
using EasyLog.Implementation;

namespace EasySave.Core.Factory
{
	public class BackupJobFactory
	{
		public static BackupJob CreateBackupJob(string name, string source, string target, string type)
		{
			// Validation
			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentException("Backup name cannot be empty");

			if (!Directory.Exists(source))
				throw new DirectoryNotFoundException($"Source not found: {source}");

			// Choisir la stratégie
			IBackupStrategy strategy;
			switch (type.ToLower())
			{
				case "complete":
					strategy = new CompleteBackupStrategy();
					break;
				case "differential":
					strategy = new DifferentialBackupStrategy();
					break;
				default:
					throw new ArgumentException($"Unknown backup type: {type}");
			}

			// Créer le job
			var job = new BackupJob(name, source, target, strategy);

			// Ajouter les observateurs
			var logger = Logger.Instance;
			job.AddObserver(new ConsoleObserver());
			job.AddObserver(new LoggerObserver(logger));
			job.AddObserver(new StateObserver(logger));

			return job;
		}
	}
}