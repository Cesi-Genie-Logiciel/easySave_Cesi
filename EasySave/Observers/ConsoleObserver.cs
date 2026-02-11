using EasySave.Interfaces;
using EasySave.Models;

namespace EasySave.Observers
{
    /// <summary>
    /// Observer that writes progress information to the console.
    /// </summary>
    public class ConsoleObserver : IBackupObserver
    {
        public void OnBackupStarted(string backupName)
        {
            Console.WriteLine($"  [Console] Backup '{backupName}' started");
        }

        public void OnBackupProgress(BackupEventArgs eventArgs)
        {
            Console.WriteLine($"  [Console] Progress: {eventArgs.Progress}% " +
                              $"({eventArgs.ProcessedFiles}/{eventArgs.TotalFiles} files)");
        }

        public void OnBackupCompleted(string backupName, bool success)
        {
            var status = success ? "completed successfully" : "completed with errors";
            Console.WriteLine($"  [Console] Backup '{backupName}' {status}");
        }
    }
}
