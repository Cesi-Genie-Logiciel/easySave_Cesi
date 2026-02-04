using EasySave.Interfaces;
using EasySave.Models;
using EasySave.Strategies;
using EasySave.Observers;

namespace EasySave.Engine
{
    /// <summary>
    /// Main backup engine that manages backup jobs
    /// Implements IBackupEngine for API stability across versions
    /// </summary>
    public class BackupEngine : IBackupEngine
    {
        private readonly List<BackupJob> _jobs;
        private readonly List<IBackupObserver> _observers;
        private const int MaxJobs = 5;

        public BackupEngine()
        {
            _jobs = new List<BackupJob>();
            _observers = new List<IBackupObserver>();

            // Initialize observers
            string appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "EasySave"
            );

            string logPath = Path.Combine(appDataPath, "Logs");
            string statePath = Path.Combine(appDataPath, "State");

            _observers.Add(new ConsoleObserver());
            _observers.Add(new LoggerObserver(logPath));
            _observers.Add(new StateObserver(statePath));
        }

        public void CreateJob(string name, string sourcePath, string targetPath, string backupType)
        {
            if (_jobs.Count >= MaxJobs)
            {
                throw new InvalidOperationException($"Maximum de {MaxJobs} travaux de sauvegarde atteint.");
            }

            if (_jobs.Any(j => j.Config.Name == name))
            {
                throw new InvalidOperationException($"Un travail avec le nom '{name}' existe déjà.");
            }

            var config = new BackupConfig(name, sourcePath, targetPath, backupType);
            var job = new BackupJob(config);

            // Set strategy based on type
            IBackupStrategy strategy = backupType.ToLower() switch
            {
                "complete" => new CompleteBackupStrategy(),
                "differential" => new DifferentialBackupStrategy(),
                _ => new CompleteBackupStrategy()
            };

            job.SetStrategy(strategy);

            // Add all observers to the job
            foreach (var observer in _observers)
            {
                job.AddObserver(observer);
            }

            _jobs.Add(job);

            Console.WriteLine($"✅ Travail '{name}' créé avec succès ({backupType}).");
        }

        public void ExecuteJob(int jobIndex)
        {
            if (jobIndex < 0 || jobIndex >= _jobs.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(jobIndex), "Index de travail invalide.");
            }

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
                    // Continue with next job
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
            {
                throw new ArgumentOutOfRangeException(nameof(jobIndex), "Index de travail invalide.");
            }

            var jobName = _jobs[jobIndex].Config.Name;
            _jobs.RemoveAt(jobIndex);
            Console.WriteLine($"✅ Travail '{jobName}' supprimé.");
        }

        public void ModifyJob(int jobIndex, string name, string sourcePath, string targetPath, string backupType)
        {
            if (jobIndex < 0 || jobIndex >= _jobs.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(jobIndex), "Index de travail invalide.");
            }

            // Check if new name conflicts with existing jobs (except current)
            if (_jobs.Any(j => j.Config.Name == name && _jobs.IndexOf(j) != jobIndex))
            {
                throw new InvalidOperationException($"Un travail avec le nom '{name}' existe déjà.");
            }

            // Delete old job
            _jobs.RemoveAt(jobIndex);

            // Create new job at same position
            var config = new BackupConfig(name, sourcePath, targetPath, backupType);
            var job = new BackupJob(config);

            IBackupStrategy strategy = backupType.ToLower() switch
            {
                "complete" => new CompleteBackupStrategy(),
                "differential" => new DifferentialBackupStrategy(),
                _ => new CompleteBackupStrategy()
            };

            job.SetStrategy(strategy);

            foreach (var observer in _observers)
            {
                job.AddObserver(observer);
            }

            _jobs.Insert(jobIndex, job);

            Console.WriteLine($"✅ Travail modifié : {name}");
        }
    }
}
