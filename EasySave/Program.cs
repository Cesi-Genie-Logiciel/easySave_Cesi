using EasySave.Engine;
using EasySave.Interfaces;
using EasySave.Localization;

namespace EasySave
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // Choix de la langue au démarrage
            SelectLanguage();

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
                            Console.WriteLine($"\n👋 {Translations.Goodbye}");
                            break;
                        default:
                            Console.WriteLine($"❌ {Translations.InvalidChoice}");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\n❌ {Translations.Error} : {ex.Message}");
                }

                if (running)
                {
                    Console.WriteLine($"\n{Translations.PressKeyToContinue}");
                    Console.ReadKey();
                }
            }
        }

        static void SelectLanguage()
        {
            // Some environments (like non-interactive consoles) don't support Clear().
            try
            {
                Console.Clear();
            }
            catch (IOException)
            {
                // Ignore and continue; purely cosmetic.
            }
            Console.WriteLine("╔════════════════════════════════════════════════╗");
            Console.WriteLine("║    SELECT LANGUAGE / CHOISIR LA LANGUE         ║");
            Console.WriteLine("╠════════════════════════════════════════════════╣");
            Console.WriteLine("║  1. Français                                   ║");
            Console.WriteLine("║  2. English                                    ║");
            Console.WriteLine("╚════════════════════════════════════════════════╝");
            Console.Write("\nYour choice / Votre choix : ");

            string? choice = Console.ReadLine();

            if (choice == "2")
            {
                Translations.SetLanguage("en");
            }
            else
            {
                Translations.SetLanguage("fr"); // Par défaut
            }

            try
            {
                Console.Clear();
            }
            catch (IOException)
            {
                // Ignore in restricted console environments.
            }
        }

        static void DisplayMenu()
        {
            Console.Clear();
            Console.WriteLine("\n╔════════════════════════════════════════════════╗");
            Console.WriteLine($"║              {Translations.MenuTitle,-30}║");
            Console.WriteLine("╠════════════════════════════════════════════════╣");
            Console.WriteLine($"║  1. {Translations.MenuCreate,-45}║");
            Console.WriteLine($"║  2. {Translations.MenuList,-45}║");
            Console.WriteLine($"║  3. {Translations.MenuExecuteOne,-45}║");
            Console.WriteLine($"║  4. {Translations.MenuExecuteAll,-45}║");
            Console.WriteLine($"║  5. {Translations.MenuModify,-45}║");
            Console.WriteLine($"║  6. {Translations.MenuDelete,-45}║");
            Console.WriteLine($"║  7. {Translations.MenuQuit,-45}║");
            Console.WriteLine("╚════════════════════════════════════════════════╝");
            Console.Write($"\n{Translations.YourChoice} : ");
        }

        static void CreateJob(IBackupEngine engine)
        {
            Console.Clear();
            Console.WriteLine($"\n--- {Translations.CreateJobTitle} ---\n");

            // Nom du travail
            Console.Write($"{Translations.JobName} : ");
            string? name = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine($"❌ {Translations.NameCannotBeEmpty}");
                return;
            }

            // Chemin source
            Console.Write($"{Translations.SourcePath} : ");
            string? sourcePath = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(sourcePath) || !Directory.Exists(sourcePath))
            {
                Console.WriteLine($"❌ {Translations.InvalidSourcePath}");
                return;
            }

            // Chemin destination
            Console.Write($"{Translations.TargetPath} : ");
            string? targetPath = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(targetPath))
            {
                Console.WriteLine($"❌ {Translations.InvalidTargetPath}");
                return;
            }

            // Type de sauvegarde
            Console.Write($"{Translations.BackupType} (1={Translations.Complete}, 2={Translations.Differential}) : ");
            string? typeChoice = Console.ReadLine();
            string backupType = typeChoice == "2" ? "Differential" : "Complete";

            engine.CreateJob(name, sourcePath, targetPath, backupType);
            Console.WriteLine($"\n✅ {Translations.JobCreated} : {name}");
        }

        static void ListJobs(IBackupEngine engine)
        {
            Console.Clear();
            Console.WriteLine($"\n--- {Translations.ListJobsTitle} ---\n");

            var jobs = engine.GetAllJobs();
            if (jobs.Count == 0)
            {
                Console.WriteLine($"⚠️  {Translations.NoJobs}");
                return;
            }

            for (int i = 0; i < jobs.Count; i++)
            {
                var job = jobs[i];
                string typeLabel = job.BackupType == "Complete" ? Translations.Complete : Translations.Differential;

                Console.WriteLine($"[{i + 1}] {job.Name}");
                Console.WriteLine($"    {Translations.Type}: {typeLabel}");
                Console.WriteLine($"    {Translations.Source}: {job.SourcePath}");
                Console.WriteLine($"    {Translations.Destination}: {job.TargetPath}");
                Console.WriteLine();
            }

            Console.WriteLine($"{Translations.Total}: {jobs.Count} {Translations.Jobs}");
        }

        static void ExecuteJob(IBackupEngine engine)
        {
            Console.Clear();
            Console.WriteLine($"\n--- {Translations.ExecuteJobTitle} ---\n");

            var jobs = engine.GetAllJobs();
            if (jobs.Count == 0)
            {
                Console.WriteLine($"⚠️  {Translations.NoJobsToExecute}");
                return;
            }

            // Afficher la liste
            for (int i = 0; i < jobs.Count; i++)
            {
                Console.WriteLine($"[{i + 1}] {jobs[i].Name}");
            }

            Console.Write($"\n{Translations.JobNumber} : ");
            if (!int.TryParse(Console.ReadLine(), out int index) || index < 1 || index > jobs.Count)
            {
                Console.WriteLine($"❌ {Translations.InvalidNumber}");
                return;
            }

            var jobToExecute = jobs[index - 1];
            Console.WriteLine($"\n▶️  {Translations.ExecutingJob} : {jobToExecute.Name}\n");
            Console.WriteLine($"🚀 {Translations.BackupStarted} : {jobToExecute.Name}");
            Console.WriteLine("============================================================");

            engine.ExecuteJob(index - 1); // L'interface attend un index (0-based)

            Console.WriteLine("============================================================");
            Console.WriteLine($"✅ {Translations.BackupCompleted} : {jobToExecute.Name}\n");
        }

        static void ExecuteAllJobs(IBackupEngine engine)
        {
            Console.Clear();
            Console.WriteLine($"\n--- {Translations.ExecuteAllTitle} ---");

            var jobs = engine.GetAllJobs();
            if (jobs.Count == 0)
            {
                Console.WriteLine($"⚠️  {Translations.NoJobsToExecute}");
                return;
            }

            Console.Write(string.Format(Translations.ConfirmExecute, jobs.Count) + " : ");
            string? confirm = Console.ReadLine();

            if (confirm?.ToLower() != "o" && confirm?.ToLower() != "y")
            {
                Console.WriteLine($"\n⚠️  {Translations.ExecutionCancelled}");
                return;
            }

            Console.WriteLine($"\n▶️  {Translations.ExecutingJob} {jobs.Count} {Translations.Jobs.ToLower()}...\n");

            engine.ExecuteAllJobs(); // L'interface gère l'exécution de tous les jobs

            Console.WriteLine($"\n✅ {Translations.AllJobsCompleted}");
        }

        static void ModifyJob(IBackupEngine engine)
        {
            Console.Clear();
            Console.WriteLine($"\n--- {Translations.ModifyJobTitle} ---\n");

            var jobs = engine.GetAllJobs();
            if (jobs.Count == 0)
            {
                Console.WriteLine($"⚠️  {Translations.NoJobsToModify}");
                return;
            }

            // Afficher la liste
            for (int i = 0; i < jobs.Count; i++)
            {
                Console.WriteLine($"[{i + 1}] {jobs[i].Name}");
            }

            Console.Write($"\n{Translations.JobToModify} : ");
            if (!int.TryParse(Console.ReadLine(), out int index) || index < 1 || index > jobs.Count)
            {
                Console.WriteLine($"❌ {Translations.InvalidNumber}");
                return;
            }

            var jobToModify = jobs[index - 1];

            Console.WriteLine($"\n{Translations.ModifyingJob} : {jobToModify.Name}");
            Console.WriteLine($"{Translations.PressEnterToKeep}\n");

            // Nouveau nom
            Console.Write($"{Translations.NewName} [{jobToModify.Name}] : ");
            string? newName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(newName))
                newName = jobToModify.Name;

            // Nouveau chemin source
            Console.Write($"{Translations.NewSourcePath} [{jobToModify.SourcePath}] : ");
            string? newSource = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(newSource))
                newSource = jobToModify.SourcePath;

            // Nouveau chemin destination
            Console.Write($"{Translations.NewTargetPath} [{jobToModify.TargetPath}] : ");
            string? newTarget = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(newTarget))
                newTarget = jobToModify.TargetPath;

            // Nouveau type
            string currentTypeLabel = jobToModify.BackupType == "Complete" ? Translations.Complete : Translations.Differential;
            Console.Write($"{Translations.NewType} (1={Translations.Complete}, 2={Translations.Differential}) [{currentTypeLabel}] : ");
            string? typeChoice = Console.ReadLine();
            string newType = jobToModify.BackupType;
            if (typeChoice == "1")
                newType = "Complete";
            else if (typeChoice == "2")
                newType = "Differential";

            engine.ModifyJob(index - 1, newName, newSource, newTarget, newType);
            Console.WriteLine($"\n✅ {Translations.JobModified} : {newName}");
        }

        static void DeleteJob(IBackupEngine engine)
        {
            Console.Clear();
            Console.WriteLine($"\n--- {Translations.DeleteJobTitle} ---\n");

            var jobs = engine.GetAllJobs();
            if (jobs.Count == 0)
            {
                Console.WriteLine($"⚠️  {Translations.NoJobsToDelete}");
                return;
            }

            // Afficher la liste
            for (int i = 0; i < jobs.Count; i++)
            {
                Console.WriteLine($"[{i + 1}] {jobs[i].Name}");
            }

            Console.Write($"\n{Translations.JobToDelete} : ");
            if (!int.TryParse(Console.ReadLine(), out int index) || index < 1 || index > jobs.Count)
            {
                Console.WriteLine($"❌ {Translations.InvalidNumber}");
                return;
            }

            var jobToDelete = jobs[index - 1];
            Console.Write(string.Format(Translations.ConfirmDelete, jobToDelete.Name) + " : ");
            string? confirm = Console.ReadLine();

            if (confirm?.ToLower() != "o" && confirm?.ToLower() != "y")
            {
                Console.WriteLine($"\n⚠️  {Translations.DeletionCancelled}");
                return;
            }

            engine.DeleteJob(index - 1);
            Console.WriteLine($"\n✅ {Translations.JobDeleted} : {jobToDelete.Name}");
        }
    }
}
