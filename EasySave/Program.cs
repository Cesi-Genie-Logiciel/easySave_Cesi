using EasySave.Engine;
using EasySave.Interfaces;

namespace EasySave
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            IBackupEngine engine = new BackupEngine();
            bool running = true;

            Console.WriteLine("╔════════════════════════════════════════════════╗");
            Console.WriteLine("║        EASYSAVE v1.0 - ProSoft 2026            ║");
            Console.WriteLine("╚════════════════════════════════════════════════╝");

            while (running)
            {
                DisplayMenu();
                string? choice = Console.ReadLine();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            CreateJob(engine);
                            break;
                        case "2":
                            ListJobs(engine);
                            break;
                        case "3":
                            ExecuteJob(engine);
                            break;
                        case "4":
                            ExecuteAllJobs(engine);
                            break;
                        case "5":
                            ModifyJob(engine);
                            break;
                        case "6":
                            DeleteJob(engine);
                            break;
                        case "7":
                            running = false;
                            Console.WriteLine("\n👋 Au revoir !");
                            break;
                        default:
                            Console.WriteLine("❌ Choix invalide. Veuillez réessayer.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\n❌ Erreur : {ex.Message}");
                }

                if (running)
                {
                    Console.WriteLine("\nAppuyez sur une touche pour continuer...");
                    Console.ReadKey();
                }
            }
        }

        static void DisplayMenu()
        {
            Console.Clear();
            Console.WriteLine("\n╔════════════════════════════════════════════════╗");
            Console.WriteLine("║              MENU PRINCIPAL                    ║");
            Console.WriteLine("╠════════════════════════════════════════════════╣");
            Console.WriteLine("║  1. Créer un travail de sauvegarde             ║");
            Console.WriteLine("║  2. Lister les travaux                         ║");
            Console.WriteLine("║  3. Exécuter un travail spécifique             ║");
            Console.WriteLine("║  4. Exécuter tous les travaux                  ║");
            Console.WriteLine("║  5. Modifier un travail                        ║");
            Console.WriteLine("║  6. Supprimer un travail                       ║");
            Console.WriteLine("║  7. Quitter                                    ║");
            Console.WriteLine("╚════════════════════════════════════════════════╝");
            Console.Write("\nVotre choix : ");
        }

        static void CreateJob(IBackupEngine engine)
        {
            Console.WriteLine("\n--- CRÉER UN NOUVEAU TRAVAIL ---");

            Console.Write("Nom du travail : ");
            string? name = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("❌ Le nom ne peut pas être vide.");
                return;
            }

            Console.Write("Chemin source : ");
            string? sourcePath = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(sourcePath) || !Directory.Exists(sourcePath))
            {
                Console.WriteLine("❌ Le chemin source est invalide ou n'existe pas.");
                return;
            }

            Console.Write("Chemin destination : ");
            string? targetPath = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(targetPath))
            {
                Console.WriteLine("❌ Le chemin destination ne peut pas être vide.");
                return;
            }

            Console.WriteLine("\nType de sauvegarde :");
            Console.WriteLine("  1. Complète (Complete)");
            Console.WriteLine("  2. Différentielle (Differential)");
            Console.Write("Votre choix (1 ou 2) : ");
            string? typeChoice = Console.ReadLine();

            string backupType = typeChoice switch
            {
                "1" => "Complete",
                "2" => "Differential",
                _ => "Complete"
            };

            engine.CreateJob(name, sourcePath, targetPath, backupType);
        }

        static void ListJobs(IBackupEngine engine)
        {
            Console.WriteLine("\n--- LISTE DES TRAVAUX ---");

            var jobs = engine.GetAllJobs();

            if (jobs.Count == 0)
            {
                Console.WriteLine("⚠️  Aucun travail de sauvegarde configuré.");
                return;
            }

            for (int i = 0; i < jobs.Count; i++)
            {
                var job = jobs[i];
                Console.WriteLine($"\n[{i + 1}] {job.Name}");
                Console.WriteLine($"    Type       : {job.BackupType}");
                Console.WriteLine($"    Source     : {job.SourcePath}");
                Console.WriteLine($"    Destination: {job.TargetPath}");
            }

            Console.WriteLine($"\nTotal : {jobs.Count}/5 travaux");
        }

        static void ExecuteJob(IBackupEngine engine)
        {
            var jobs = engine.GetAllJobs();

            if (jobs.Count == 0)
            {
                Console.WriteLine("⚠️  Aucun travail à exécuter.");
                return;
            }

            Console.WriteLine("\n--- EXÉCUTER UN TRAVAIL ---");
            ListJobs(engine);

            Console.Write("\nNuméro du travail à exécuter : ");
            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= jobs.Count)
            {
                engine.ExecuteJob(choice - 1);
            }
            else
            {
                Console.WriteLine("❌ Numéro invalide.");
            }
        }

        static void ExecuteAllJobs(IBackupEngine engine)
        {
            Console.WriteLine("\n--- EXÉCUTER TOUS LES TRAVAUX ---");

            var jobs = engine.GetAllJobs();
            if (jobs.Count == 0)
            {
                Console.WriteLine("⚠️  Aucun travail à exécuter.");
                return;
            }

            Console.Write($"Confirmer l'exécution de {jobs.Count} travaux ? (O/N) : ");
            string? confirm = Console.ReadLine();

            if (confirm?.ToUpper() == "O")
            {
                engine.ExecuteAllJobs();
            }
            else
            {
                Console.WriteLine("❌ Exécution annulée.");
            }
        }

        static void ModifyJob(IBackupEngine engine)
        {
            var jobs = engine.GetAllJobs();

            if (jobs.Count == 0)
            {
                Console.WriteLine("⚠️  Aucun travail à modifier.");
                return;
            }

            Console.WriteLine("\n--- MODIFIER UN TRAVAIL ---");
            ListJobs(engine);

            Console.Write("\nNuméro du travail à modifier : ");
            if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 1 || choice > jobs.Count)
            {
                Console.WriteLine("❌ Numéro invalide.");
                return;
            }

            int jobIndex = choice - 1;
            var job = jobs[jobIndex];

            Console.WriteLine($"\nModification de : {job.Name}");
            Console.WriteLine("(Appuyez sur Entrée pour conserver la valeur actuelle)\n");

            Console.Write($"Nouveau nom [{job.Name}] : ");
            string? name = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(name)) name = job.Name;

            Console.Write($"Nouveau chemin source [{job.SourcePath}] : ");
            string? sourcePath = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(sourcePath)) sourcePath = job.SourcePath;

            Console.Write($"Nouveau chemin destination [{job.TargetPath}] : ");
            string? targetPath = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(targetPath)) targetPath = job.TargetPath;

            Console.WriteLine($"\nType actuel : {job.BackupType}");
            Console.WriteLine("Nouveau type :");
            Console.WriteLine("  1. Complète (Complete)");
            Console.WriteLine("  2. Différentielle (Differential)");
            Console.Write("Votre choix (1 ou 2, ou Entrée pour conserver) : ");
            string? typeChoice = Console.ReadLine();

            string backupType = typeChoice switch
            {
                "1" => "Complete",
                "2" => "Differential",
                _ => job.BackupType
            };

            engine.ModifyJob(jobIndex, name, sourcePath, targetPath, backupType);
        }

        static void DeleteJob(IBackupEngine engine)
        {
            var jobs = engine.GetAllJobs();

            if (jobs.Count == 0)
            {
                Console.WriteLine("⚠️  Aucun travail à supprimer.");
                return;
            }

            Console.WriteLine("\n--- SUPPRIMER UN TRAVAIL ---");
            ListJobs(engine);

            Console.Write("\nNuméro du travail à supprimer : ");
            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= jobs.Count)
            {
                int jobIndex = choice - 1;
                string jobName = jobs[jobIndex].Name;

                Console.Write($"Confirmer la suppression de '{jobName}' ? (O/N) : ");
                string? confirm = Console.ReadLine();

                if (confirm?.ToUpper() == "O")
                {
                    engine.DeleteJob(jobIndex);
                }
                else
                {
                    Console.WriteLine("❌ Suppression annulée.");
                }
            }
            else
            {
                Console.WriteLine("❌ Numéro invalide.");
            }
        }
    }
}
