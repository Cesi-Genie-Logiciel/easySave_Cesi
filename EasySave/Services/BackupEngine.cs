using System.Text.Json;
using EasySave.Interfaces;
using EasySave.Models;
using EasySave.Observers;
using EasySave.Factories;

namespace EasySave.Engine
{
    /// <summary>
    /// Main backup engine that manages backup jobs.
    /// Implements IBackupEngine for API stability across versions.
    /// </summary>
    public class BackupEngine : IBackupEngine
    {
        private readonly List<BackupJob> _jobs;
        private readonly List<IBackupObserver> _observers;
        private readonly string _configFilePath;
        private const int MaxJobs = 5;

        public BackupEngine()
        {
            _jobs = new List<BackupJob>();
            _observers = new List<IBackupObserver>();

            // Dossier d'application dans AppData
            string appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "EasySave"
            );

            // Sous-dossiers pour les logs et l'état
            string logPath = Path.Combine(appDataPath, "Logs");
            string statePath = Path.Combine(appDataPath, "State");

            // Fichier de configuration des jobs
            _configFilePath = Path.Combine(appDataPath, "backup_configs.json");

            // S'assurer que les dossiers existent
            Directory.CreateDirectory(appDataPath);
            Directory.CreateDirectory(logPath);
            Directory.CreateDirectory(statePath);

            // Initialiser les observers
            _observers.Add(new ConsoleObserver());
            _observers.Add(new LoggerObserver(logPath));
            _observers.Add(new StateObserver(statePath));

            // Charger les jobs existants depuis le fichier de config
            LoadJobsFromConfig();
        }

        public void CreateJob(string name, string sourcePath, string targetPath, string backupType)
        {
            if (_jobs.Count >= MaxJobs)
                throw new InvalidOperationException($"Maximum de {MaxJobs} travaux de sauvegarde atteint.");

            if (_jobs.Any(j => j.Config.Name == name))
                throw new InvalidOperationException($"Un travail avec le nom '{name}' existe déjà.");

            var config = new BackupConfig(name, sourcePath, targetPath, backupType);

            // Utilisation de la factory pour créer un BackupJob complet (strategy + observers)
            var job = BackupJobFactory.Create(config, _observers);

            _jobs.Add(job);
            SaveJobsToConfig();

            Console.WriteLine($"✅ Travail '{name}' créé avec succès ({backupType}).");
        }

        public void ExecuteJob(int jobIndex)
        {
            if (jobIndex < 0 || jobIndex >= _jobs.Count)
                throw new ArgumentOutOfRangeException(nameof(jobIndex), "Index de travail invalide.");

            var job = _jobs[jobIndex];
            Console.WriteLine($"\n▶️  Exécution du travail : {job.Config.Name}");

            try
            {
                job.Execute();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur lors de l'exécution : {ex.Message}");
                throw;
            }
        }

        public void ExecuteAllJobs()
        {
            if (_jobs.Count == 0)
            {
                Console.WriteLine("⚠️  Aucun travail de sauvegarde à exécuter.");
                return;
            }

            Console.WriteLine($"\n▶️  Exécution de {_jobs.Count} travaux de sauvegarde...\n");

            for (int i = 0; i < _jobs.Count; i++)
            {
                try
                {
                    ExecuteJob(i);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Erreur avec le travail {i + 1} : {ex.Message}");
                    // On continue avec le suivant
                }
            }

            Console.WriteLine("\n✅ Exécution terminée pour tous les travaux.");
        }

        public List<BackupConfig> GetAllJobs()
        {
            return _jobs.Select(j => j.Config).ToList();
        }

        public void DeleteJob(int jobIndex)
        {
            if (jobIndex < 0 || jobIndex >= _jobs.Count)
                throw new ArgumentOutOfRangeException(nameof(jobIndex), "Index de travail invalide.");

            var jobName = _jobs[jobIndex].Config.Name;
            _jobs.RemoveAt(jobIndex);
            SaveJobsToConfig();

            Console.WriteLine($"✅ Travail '{jobName}' supprimé.");
        }

        public void ModifyJob(int jobIndex, string name, string sourcePath, string targetPath, string backupType)
        {
            if (jobIndex < 0 || jobIndex >= _jobs.Count)
                throw new ArgumentOutOfRangeException(nameof(jobIndex), "Index de travail invalide.");

            // Vérifie que le nouveau nom ne rentre pas en conflit (sauf avec le job actuel)
            if (_jobs.Any(j => j.Config.Name == name && _jobs.IndexOf(j) != jobIndex))
                throw new InvalidOperationException($"Un travail avec le nom '{name}' existe déjà.");

            var config = new BackupConfig(name, sourcePath, targetPath, backupType);

            // Recréation complète du job via la factory
            var newJob = BackupJobFactory.Create(config, _observers);

            _jobs[jobIndex] = newJob;
            SaveJobsToConfig();

            Console.WriteLine($"✅ Travail modifié : {name}");
        }

        private void LoadJobsFromConfig()
        {
            if (!File.Exists(_configFilePath))
                return;

            try
            {
                string json = File.ReadAllText(_configFilePath);
                if (string.IsNullOrWhiteSpace(json))
                    return;

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var configs = JsonSerializer.Deserialize<List<BackupConfig>>(json, options);
                if (configs == null)
                    return;

                _jobs.Clear();

                foreach (var config in configs)
                {
                    var job = BackupJobFactory.Create(config, _observers);
                    _jobs.Add(job);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️  Impossible de charger les travaux depuis '{_configFilePath}' : {ex.Message}");
            }
        }

        private void SaveJobsToConfig()
        {
            try
            {
                var configs = _jobs.Select(j => j.Config).ToList();

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                string json = JsonSerializer.Serialize(configs, options);
                File.WriteAllText(_configFilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️  Impossible d'enregistrer les travaux dans '{_configFilePath}' : {ex.Message}");
            }
        }
    }
}
