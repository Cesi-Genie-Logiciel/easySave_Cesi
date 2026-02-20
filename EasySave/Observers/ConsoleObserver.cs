using EasySave.Interfaces;
using EasySave.Models;

namespace EasySave.Observers
{
    public class ConsoleObserver : IBackupObserver
    {
        public void OnBackupStarted(string backupName)
        {
            Console.WriteLine($"  Sauvegarde '{backupName}' demarree");
        }

        public void OnFileTransferred(BackupEventArgs e)
        {
            Console.WriteLine($"  Progression : {e.Progress}% ({e.ProcessedFiles}/{e.TotalFiles} fichiers)");
        }

        public void OnBackupCompleted(string backupName)
        {
            Console.WriteLine($"  Sauvegarde '{backupName}' terminee");
        }
    }
}