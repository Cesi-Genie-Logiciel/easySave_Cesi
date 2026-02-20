using EasySave.Interfaces;
using EasySave.Models;
using EasySave.Services;
using System;
using System.Collections.Generic;
using System.Net;

namespace EasySave
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            ISettingsService settingsService = new SettingsService();
            IBackupService service = new BackupService();

            // Command line mode: EasySave.exe run 1 or EasySave.exe run 1,2,3
            if (args.Length >= 2 && args[0].ToLower() == "run")
            {
                LancerEnLigneDeCommande(service, args[1]);
                return;
            }

            bool running = true;
            while (running)
            {
                AfficherMenu();
                string? choix = Console.ReadLine();

                try
                {
                    switch (choix)
                    {
                        case "1": CreerJob(service); break;
                        case "2": ListerJobs(service); break;
                        case "3": ExecuterJob(service); break;
                        case "4": ExecuterPlusieurs(service); break;
                        case "5": ModifierJob(service); break;
                        case "6": SupprimerJob(service); break;
                        case "7": Parametres(settingsService); break;
                        case "8":
                            running = false;
                            Console.WriteLine("\nA bientot !");
                            break;
                        default:
                            Console.WriteLine("Choix invalide.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nErreur : {ex.Message}");
                }

                if (running)
                {
                    Console.WriteLine("\nAppuyez sur Entree...");
                    Console.ReadLine();
                }
            }
        }

        static void LancerEnLigneDeCommande(IBackupService service, string nums)
        {
            var jobs = service.GetAllBackupJobs();
            if (jobs.Count == 0)
            {
                Console.WriteLine("Aucun job configure.");
                return;
            }

            var indices = new List<int>();
            foreach (var part in nums.Split(','))
            {
                if (int.TryParse(part.Trim(), out int n) && n > 0 && n <= jobs.Count)
                    indices.Add(n - 1);
            }

            if (indices.Count == 0)
            {
                Console.WriteLine("Aucun numero de job valide.");
                return;
            }

            service.ExecuteMultipleBackupJobs(indices);
        }

        static void AfficherMenu()
        {
            try { Console.Clear(); } catch { }
            Console.WriteLine("\n========================================");
            Console.WriteLine("        EASYSAVE v1.0 - ProSoft");
            Console.WriteLine("========================================");
            Console.WriteLine("  1. Creer un job de sauvegarde");
            Console.WriteLine("  2. Lister les jobs");
            Console.WriteLine("  3. Executer un job");
            Console.WriteLine("  4. Executer plusieurs jobs");
            Console.WriteLine("  5. Modifier un job");
            Console.WriteLine("  6. Supprimer un job");
            Console.WriteLine("  7. Parametres");
            Console.WriteLine("  8. Quitter");
            Console.WriteLine("========================================");
            Console.Write("\nVotre choix : ");
        }

        static void CreerJob(IBackupService service)
        {
            Console.WriteLine("\n--- Creer un job ---\n");

            Console.Write("Nom du job : ");
            string? name = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(name)) { Console.WriteLine("Nom vide, annule."); return; }

            Console.Write("Chemin source : ");
            string? source = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(source)) { Console.WriteLine("Chemin vide, annule."); return; }

            Console.Write("Chemin destination : ");
            string? target = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(target)) { Console.WriteLine("Chemin vide, annule."); return; }

            Console.Write("Type (1=Complete, 2=Differentielle) : ");
            string? t = Console.ReadLine();
            string type = t == "2" ? "differential" : "complete";

            service.CreateBackupJob(name, source, target, type);
            Console.WriteLine($"\nJob '{name}' cree.");
        }

        static void ListerJobs(IBackupService service)
        {
            Console.WriteLine("\n--- Liste des jobs ---\n");

            var jobs = service.GetAllBackupJobs();
            if (jobs.Count == 0)
            {
                Console.WriteLine("Aucun job.");
                return;
            }

            for (int i = 0; i < jobs.Count; i++)
            {
                var j = jobs[i];
                Console.WriteLine($"[{i + 1}] {j.Name} ({j.BackupType})");
                Console.WriteLine($"    Source : {j.SourcePath}");
                Console.WriteLine($"    Dest   : {j.TargetPath}\n");
            }
        }

        static void ExecuterJob(IBackupService service)
        {
            var jobs = service.GetAllBackupJobs();
            if (jobs.Count == 0) { Console.WriteLine("Aucun job."); return; }

            for (int i = 0; i < jobs.Count; i++)
                Console.WriteLine($"[{i + 1}] {jobs[i].Name}");

            Console.Write("\nNumero du job : ");
            if (!int.TryParse(Console.ReadLine(), out int idx) || idx < 1 || idx > jobs.Count)
            {
                Console.WriteLine("Numero invalide.");
                return;
            }

            service.ExecuteBackupJob(idx - 1);
        }

        static void ExecuterPlusieurs(IBackupService service)
        {
            var jobs = service.GetAllBackupJobs();
            if (jobs.Count == 0) { Console.WriteLine("Aucun job."); return; }

            for (int i = 0; i < jobs.Count; i++)
                Console.WriteLine($"[{i + 1}] {jobs[i].Name}");

            Console.Write("\nNumeros separes par des virgules (ex: 1,2,3) : ");
            string? input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input)) { Console.WriteLine("Annule."); return; }

            var indices = new List<int>();
            foreach (var part in input.Split(','))
            {
                if (int.TryParse(part.Trim(), out int n) && n > 0 && n <= jobs.Count)
                    indices.Add(n - 1);
            }

            if (indices.Count == 0) { Console.WriteLine("Aucun numero valide."); return; }

            service.ExecuteMultipleBackupJobs(indices);
        }

        static void ModifierJob(IBackupService service)
        {
            var jobs = service.GetAllBackupJobs();
            if (jobs.Count == 0) { Console.WriteLine("Aucun job."); return; }

            Console.WriteLine("\n--- Modifier un job ---\n");

            for (int i = 0; i < jobs.Count; i++)
                Console.WriteLine($"[{i + 1}] {jobs[i].Name}");

            Console.Write("\nNumero du job a modifier : ");
            if (!int.TryParse(Console.ReadLine(), out int idx) || idx < 1 || idx > jobs.Count)
            {
                Console.WriteLine("Numero invalide.");
                return;
            }

            var old = jobs[idx - 1];
            Console.WriteLine($"\nJob actuel : {old.Name}");
            Console.WriteLine($"  Source : {old.SourcePath}");
            Console.WriteLine($"  Dest   : {old.TargetPath}");
            Console.WriteLine($"  Type   : {old.BackupType}\n");

            Console.Write($"Nouveau nom (Entree = garder '{old.Name}') : ");
            string? name = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(name)) name = old.Name;

            Console.Write($"Nouveau chemin source (Entree = garder) : ");
            string? source = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(source)) source = old.SourcePath;

            Console.Write($"Nouveau chemin destination (Entree = garder) : ");
            string? target = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(target)) target = old.TargetPath;

            Console.Write($"Type (1=Complete, 2=Differentielle, Entree = garder) : ");
            string? t = Console.ReadLine();
            string type = old.BackupType;
            if (t == "1") type = "complete";
            else if (t == "2") type = "differential";

            service.UpdateBackupJob(idx - 1, name, source, target, type);
        }

        static void SupprimerJob(IBackupService service)
        {
            var jobs = service.GetAllBackupJobs();
            if (jobs.Count == 0) { Console.WriteLine("Aucun job."); return; }

            for (int i = 0; i < jobs.Count; i++)
                Console.WriteLine($"[{i + 1}] {jobs[i].Name}");

            Console.Write("\nNumero du job a supprimer : ");
            if (!int.TryParse(Console.ReadLine(), out int idx) || idx < 1 || idx > jobs.Count)
            {
                Console.WriteLine("Numero invalide.");
                return;
            }

            Console.Write($"Supprimer '{jobs[idx - 1].Name}' ? (o/n) : ");
            if (Console.ReadLine()?.ToLower() != "o")
            {
                Console.WriteLine("Annule.");
                return;
            }

            service.DeleteBackupJob(idx - 1);
        }

        static void Parametres(ISettingsService settingsService)
        {
            var settings = settingsService.GetCurrent();

            Console.WriteLine("\n--- Parametres ---\n");
            Console.WriteLine($"  Format de log    : {settings.LogFormat}");
            Console.WriteLine($"  Extensions       : {string.Join(", ", settings.ExtensionsToEncrypt)}");
            Console.WriteLine($"  Logiciel metier  : {(string.IsNullOrEmpty(settings.BusinessSoftwareName) ? "Aucun" : settings.BusinessSoftwareName)}");

            Console.WriteLine("\n  1. Changer le format de log");
            Console.WriteLine("  2. Ajouter une extension a chiffrer");
            Console.WriteLine("  3. Definir le logiciel metier");
            Console.WriteLine("  4. Sauvegarder");
            Console.WriteLine("  5. Retour");

            Console.Write("\nChoix : ");
            string? choix = Console.ReadLine();

            switch (choix)
            {
                case "1":
                    Console.Write("Format (JSON/XML) : ");
                    string? fmt = Console.ReadLine()?.ToUpper();
                    if (fmt == "JSON") settings.LogFormat = LogFormat.JSON;
                    else if (fmt == "XML") settings.LogFormat = LogFormat.XML;
                    else Console.WriteLine("Format invalide.");
                    break;
                case "2":
                    Console.Write("Extension (ex: .doc) : ");
                    string? ext = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(ext) && !settings.ExtensionsToEncrypt.Contains(ext))
                        settings.ExtensionsToEncrypt.Add(ext);
                    break;
                case "3":
                    Console.Write("Nom du processus : ");
                    settings.BusinessSoftwareName = Console.ReadLine() ?? "";
                    break;
                case "4":
                    settingsService.Save(settings);
                    Console.WriteLine("Parametres sauvegardes.");
                    break;
                case "5":
                    return;
            }
        }
    }
}
