using System;
using System.Collections.Generic;
using EasySave.Services.Interfaces;
using EasySave.Services.Implementations;
using EasySave.Core.Models;

namespace EasySave
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("=================================");
			Console.WriteLine("   EASYSAVE v1.0 - ProSoft");
			Console.WriteLine("=================================\n");

			IBackupService service = new BackupService();

			RunMenu(service);
		}

		static void RunMenu(IBackupService service)
		{
			bool exit = false;

			while (!exit)
			{
				Console.Clear();
				Console.WriteLine("===== EASYSAVE v1.0 - MENU =====");
				Console.WriteLine("1. Create a backup job");
				Console.WriteLine("2. List backup jobs");
				Console.WriteLine("3. Execute one backup job");
				Console.WriteLine("4. Execute several backup jobs");
				Console.WriteLine("5. Delete a backup job");
				Console.WriteLine("0. Exit");
				Console.Write("\nYour choice: ");

				string choice = Console.ReadLine();

				try
				{
					switch (choice)
					{
						case "1":
							CreateJobMenu(service);
							break;
						case "2":
							ListJobsMenu(service);
							break;
						case "3":
							ExecuteOneJobMenu(service);
							break;
						case "4":
							ExecuteMultipleJobsMenu(service);
							break;
						case "5":
							DeleteJobMenu(service);
							break;
						case "0":
							exit = true;
							break;
						default:
							Console.WriteLine("Invalid choice.");
							Pause();
							break;
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Error: {ex.Message}");
					Pause();
				}
			}
		}

		static void CreateJobMenu(IBackupService service)
		{
			Console.Clear();
			Console.WriteLine("=== CREATE BACKUP JOB ===");

			Console.Write("Backup name: ");
			string name = Console.ReadLine();

			Console.Write("Source path: ");
			string source = Console.ReadLine();

			Console.Write("Target path: ");
			string target = Console.ReadLine();

			Console.Write("Type (complete/differential): ");
			string type = Console.ReadLine();

			service.CreateBackupJob(name, source, target, type);
			Pause();
		}

		static void ListJobsMenu(IBackupService service)
		{
			Console.Clear();
			Console.WriteLine("=== BACKUP JOBS ===");

			var jobs = service.GetAllBackupJobs();
			if (jobs.Count == 0)
			{
				Console.WriteLine("No backup jobs created.");
			}
			else
			{
				for (int i = 0; i < jobs.Count; i++)
				{
					var job = jobs[i];
					Console.WriteLine($"{i + 1}. {job.Name}  ({job.SourcePath} -> {job.TargetPath})");
				}
			}

			Pause();
		}

		static void ExecuteOneJobMenu(IBackupService service)
		{
			Console.Clear();
			Console.WriteLine("=== EXECUTE ONE BACKUP JOB ===");

			var jobs = service.GetAllBackupJobs();
			if (jobs.Count == 0)
			{
				Console.WriteLine("No backup jobs created.");
				Pause();
				return;
			}

			for (int i = 0; i < jobs.Count; i++)
			{
				var job = jobs[i];
				Console.WriteLine($"{i + 1}. {job.Name}  ({job.SourcePath} -> {job.TargetPath})");
			}

			Console.Write("\nEnter job number to execute: ");
			if (int.TryParse(Console.ReadLine(), out int num))
			{
				service.ExecuteBackupJob(num - 1);
			}
			else
			{
				Console.WriteLine("Invalid number.");
			}

			Pause();
		}

		static void ExecuteMultipleJobsMenu(IBackupService service)
		{
			Console.Clear();
			Console.WriteLine("=== EXECUTE MULTIPLE BACKUP JOBS ===");

			var jobs = service.GetAllBackupJobs();
			if (jobs.Count == 0)
			{
				Console.WriteLine("No backup jobs created.");
				Pause();
				return;
			}

			for (int i = 0; i < jobs.Count; i++)
			{
				var job = jobs[i];
				Console.WriteLine($"{i + 1}. {job.Name}  ({job.SourcePath} -> {job.TargetPath})");
			}

			Console.Write("\nEnter indices (ex: 1-3 or 1;3): ");
			string input = Console.ReadLine();

			List<int> indices = ParseJobIndices(input);
			service.ExecuteMultipleBackupJobs(indices);
			Pause();
		}

		static void DeleteJobMenu(IBackupService service)
		{
			Console.Clear();
			Console.WriteLine("=== DELETE BACKUP JOB ===");

			var jobs = service.GetAllBackupJobs();
			if (jobs.Count == 0)
			{
				Console.WriteLine("No backup jobs created.");
				Pause();
				return;
			}

			for (int i = 0; i < jobs.Count; i++)
			{
				var job = jobs[i];
				Console.WriteLine($"{i + 1}. {job.Name}  ({job.SourcePath} -> {job.TargetPath})");
			}

			Console.Write("\nEnter job number to delete: ");
			if (int.TryParse(Console.ReadLine(), out int num))
			{
				service.DeleteBackupJob(num - 1);
			}
			else
			{
				Console.WriteLine("Invalid number.");
			}

			Pause();
		}

		// Gère "1-3", "1;3" ou "2"
		static List<int> ParseJobIndices(string text)
		{
			var indices = new List<int>();
			text = text.Trim();

			if (text.Contains("-"))
			{
				var parts = text.Split('-');
				int start = int.Parse(parts[0]);
				int end = int.Parse(parts[1]);

				for (int i = start; i <= end; i++)
					indices.Add(i - 1); // 0-based
			}
			else if (text.Contains(";"))
			{
				var parts = text.Split(';');
				foreach (var p in parts)
				{
					if (int.TryParse(p, out int value))
						indices.Add(value - 1);
				}
			}
			else
			{
				if (int.TryParse(text, out int value))
					indices.Add(value - 1);
			}

			return indices;
		}

		static void Pause()
		{
			Console.WriteLine("\nPress any key to continue...");
			Console.ReadKey();
		}
	}
}
