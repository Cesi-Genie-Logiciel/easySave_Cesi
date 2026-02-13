using System;
using EasySave.Interfaces;
using EasySave.Models;

namespace EasySave.Observers
{
    public class ConsoleObserver : IBackupObserver
    {
        public void OnBackupStarted(string backupName)
        {
            Console.WriteLine($"  [Console] Backup '{backupName}' started");
        }
        
        public void OnFileTransferred(BackupEventArgs e)
        {
            Console.WriteLine($"  [Console] Progress: {e.Progress}% ({e.ProcessedFiles}/{e.TotalFiles} files)");
        }
        
        public void OnBackupCompleted(string backupName)
        {
            Console.WriteLine($"  [Console] Backup '{backupName}' completed successfully");
        }
    }
}
