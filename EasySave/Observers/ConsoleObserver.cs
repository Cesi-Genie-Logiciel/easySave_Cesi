using EasySave.Interfaces;
using EasySave.Models;
using System;

namespace EasySave.Observers
{
    // Prints backup events to the console for real-time feedback in CLI mode.
    public class ConsoleObserver : IBackupObserver
    {
        public void OnBackupStarted(string backupName)
        {
            Console.WriteLine($"  Backup '{backupName}' started");
        }

        public void OnFileTransferred(BackupEventArgs args)
        {
            Console.WriteLine($"  Progress: {args.Progress}% ({args.ProcessedFiles}/{args.TotalFiles} files)");
        }

        public void OnBackupCompleted(string backupName)
        {
            Console.WriteLine($"  Backup '{backupName}' completed");
        }

        public void OnBackupStateChanged(string backupName, BackupJobState state)
        {
            Console.WriteLine($"  Backup '{backupName}' state changed to: {state}");
        }
    }
}