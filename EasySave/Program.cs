using System;
using System.Collections.Generic;
using EasySave.Interfaces;
using EasySave.Services;

namespace EasySave
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            
            Console.WriteLine("=================================");
            Console.WriteLine("   EASYSAVE v1.0 - ProSoft");
            Console.WriteLine("=================================\n");
            
            IBackupService service = new BackupService();
            bool running = true;
            
            while (running)
            {
                DisplayMenu();
                string? choice = Console.ReadLine();
                
                try
                {
                    switch (choice)
                    {
                        case "1":
                            CreateJob(service);
                            break;
                        case "2":
                            ListJobs(service);
                            break;
                        case "3":
                            ExecuteJob(service);
                            break;
                        case "4":
                            ExecuteMultipleJobs(service);
                            break;
                        case "5":
                            DeleteJob(service);
                            break;
                        case "6":
                            running = false;
                            Console.WriteLine("\n👋 Goodbye!");
                            break;
                        default:
                            Console.WriteLine("❌ Invalid choice. Please try again.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\n❌ Error: {ex.Message}");
                }
                
                if (running)
                {
                    Console.WriteLine("\nPress Enter to continue...");
                    Console.ReadLine();
                }
            }
        }
        
        static void DisplayMenu()
        {
            try { Console.Clear(); } catch { }
            Console.WriteLine("\n╔════════════════════════════════════════════════╗");
            Console.WriteLine("║              MAIN MENU                         ║");
            Console.WriteLine("╠════════════════════════════════════════════════╣");
            Console.WriteLine("║  1. Create backup job                          ║");
            Console.WriteLine("║  2. List all backup jobs                       ║");
            Console.WriteLine("║  3. Execute a backup job                       ║");
            Console.WriteLine("║  4. Execute multiple backup jobs               ║");
            Console.WriteLine("║  5. Delete a backup job                        ║");
            Console.WriteLine("║  6. Quit                                       ║");
            Console.WriteLine("╚════════════════════════════════════════════════╝");
            Console.Write("\nYour choice: ");
        }
        
        static void CreateJob(IBackupService service)
        {
            try { Console.Clear(); } catch { }
            Console.WriteLine("\n--- CREATE BACKUP JOB ---\n");
            
            Console.Write("Job name: ");
            string? name = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("❌ Name cannot be empty.");
                return;
            }
            
            Console.Write("Source path: ");
            string? sourcePath = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(sourcePath))
            {
                Console.WriteLine("❌ Source path cannot be empty.");
                return;
            }
            
            Console.Write("Target path: ");
            string? targetPath = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(targetPath))
            {
                Console.WriteLine("❌ Target path cannot be empty.");
                return;
            }
            
            Console.Write("Backup type (1=Complete, 2=Differential): ");
            string? typeChoice = Console.ReadLine();
            string backupType = typeChoice == "2" ? "differential" : "complete";
            
            service.CreateBackupJob(name, sourcePath, targetPath, backupType);
            Console.WriteLine($"\n✅ Job '{name}' created successfully!");
        }
        
        static void ListJobs(IBackupService service)
        {
            try { Console.Clear(); } catch { }
            Console.WriteLine("\n--- ALL BACKUP JOBS ---\n");
            
            var jobs = service.GetAllBackupJobs();
            if (jobs.Count == 0)
            {
                Console.WriteLine("⚠️  No backup jobs found.");
                return;
            }
            
            for (int i = 0; i < jobs.Count; i++)
            {
                var job = jobs[i];
                Console.WriteLine($"[{i + 1}] {job.Name}");
                Console.WriteLine($"    Source: {job.SourcePath}");
                Console.WriteLine($"    Target: {job.TargetPath}");
                Console.WriteLine();
            }
            
            Console.WriteLine($"Total: {jobs.Count} job(s)");
        }
        
        static void ExecuteJob(IBackupService service)
        {
            try { Console.Clear(); } catch { }
            Console.WriteLine("\n--- EXECUTE BACKUP JOB ---\n");
            
            var jobs = service.GetAllBackupJobs();
            if (jobs.Count == 0)
            {
                Console.WriteLine("⚠️  No jobs to execute.");
                return;
            }
            
            // Afficher la liste
            for (int i = 0; i < jobs.Count; i++)
            {
                Console.WriteLine($"[{i + 1}] {jobs[i].Name}");
            }
            
            Console.Write("\nJob number to execute: ");
            if (!int.TryParse(Console.ReadLine(), out int index) || index < 1 || index > jobs.Count)
            {
                Console.WriteLine("❌ Invalid number.");
                return;
            }
            
            Console.WriteLine($"\n▶️  Executing job: {jobs[index - 1].Name}\n");
            Console.WriteLine("============================================================");
            
            // Prefer TryExecuteBackupJob when available (business software detection)
            if (service is EasySave.Services.BackupService concreteService)
            {
                var executed = concreteService.TryExecuteBackupJob(index - 1);
                Console.WriteLine("============================================================");
                Console.WriteLine(executed ? "✅ Backup completed!\n" : "⚠️  Backup not started (business software running).\n");
                return;
            }

            service.ExecuteBackupJob(index - 1);

            Console.WriteLine("============================================================");
            Console.WriteLine($"✅ Backup completed!\n");
        }
        
        static void ExecuteMultipleJobs(IBackupService service)
        {
            try { Console.Clear(); } catch { }
            Console.WriteLine("\n--- EXECUTE MULTIPLE BACKUP JOBS ---\n");
            
            var jobs = service.GetAllBackupJobs();
            if (jobs.Count == 0)
            {
                Console.WriteLine("⚠️  No jobs to execute.");
                return;
            }
            
            // Afficher la liste
            for (int i = 0; i < jobs.Count; i++)
            {
                Console.WriteLine($"[{i + 1}] {jobs[i].Name}");
            }
            
            Console.Write("\nEnter job numbers separated by commas (e.g., 1,2,3): ");
            string? input = Console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("❌ No jobs selected.");
                return;
            }
            
            var indices = new List<int>();
            foreach (var part in input.Split(','))
            {
                if (int.TryParse(part.Trim(), out int num) && num > 0 && num <= jobs.Count)
                {
                    indices.Add(num - 1);
                }
            }
            
            if (indices.Count == 0)
            {
                Console.WriteLine("❌ No valid jobs selected.");
                return;
            }
            
            Console.WriteLine($"\n▶️  Executing {indices.Count} job(s)...\n");
            Console.WriteLine("============================================================");
            
            service.ExecuteMultipleBackupJobs(indices);
            
            Console.WriteLine("============================================================");
            Console.WriteLine($"\n✅ All selected backups completed!\n");
        }
        
        static void DeleteJob(IBackupService service)
        {
            try { Console.Clear(); } catch { }
            Console.WriteLine("\n--- DELETE BACKUP JOB ---\n");
            
            var jobs = service.GetAllBackupJobs();
            if (jobs.Count == 0)
            {
                Console.WriteLine("⚠️  No jobs to delete.");
                return;
            }
            
            // Afficher la liste
            for (int i = 0; i < jobs.Count; i++)
            {
                Console.WriteLine($"[{i + 1}] {jobs[i].Name}");
            }
            
            Console.Write("\nJob number to delete: ");
            if (!int.TryParse(Console.ReadLine(), out int index) || index < 1 || index > jobs.Count)
            {
                Console.WriteLine("❌ Invalid number.");
                return;
            }
            
            var jobName = jobs[index - 1].Name;
            Console.Write($"\n⚠️  Are you sure you want to delete '{jobName}'? (y/n): ");
            string? confirm = Console.ReadLine();
            
            if (confirm?.ToLower() != "y")
            {
                Console.WriteLine("\n⚠️  Deletion cancelled.");
                return;
            }
            
            service.DeleteBackupJob(index - 1);
            Console.WriteLine($"\n✅ Job '{jobName}' deleted successfully!");
        }
    }
}
