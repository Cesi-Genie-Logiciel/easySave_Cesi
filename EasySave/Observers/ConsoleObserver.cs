using EasySave.Interfaces;
using EasySave.Models;

namespace EasySave.Observers
{
    /// <summary>
    /// Observer that displays backup progress in console
    /// </summary>
    public class ConsoleObserver : IBackupObserver
    {
        public void OnBackupStarted(string backupName)
        {
            Console.WriteLine($"\n🚀 Démarrage de la sauvegarde : {backupName}");
            Console.WriteLine(new string('=', 60));
        }

        public void OnBackupProgress(BackupEventArgs eventArgs)
        {
            if (eventArgs.TransferTime >= 0)
            {
                // Succès
                Console.WriteLine(
                    $"✅ [{eventArgs.ProcessedFiles}/{eventArgs.TotalFiles}] " +
                    $"{Path.GetFileName(eventArgs.SourceFile)} " +
                    $"({eventArgs.FileSize / 1024} KB, {eventArgs.TransferTime}ms) " +
                    $"- {eventArgs.Stats.Progress}%"
                );
            }
            else
            {
                // Erreur
                Console.WriteLine(
                    $"❌ [{eventArgs.ProcessedFiles}/{eventArgs.TotalFiles}] " +
                    $"{Path.GetFileName(eventArgs.SourceFile)} - ERREUR"
                );
            }
        }

        public void OnBackupCompleted(string backupName, bool success)
        {
            Console.WriteLine(new string('=', 60));
            if (success)
            {
                Console.WriteLine($"✅ Sauvegarde terminée : {backupName}");
            }
            else
            {
                Console.WriteLine($"❌ Sauvegarde échouée : {backupName}");
            }
            Console.WriteLine();
        }
    }
}
