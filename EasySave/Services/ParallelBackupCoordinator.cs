using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EasySave.Models;

namespace EasySave.Services
{
    /// <summary>
    /// V3/P1 (branche 2): orchestrateur parallèle minimal.
    ///
    /// Périmètre: lancer les jobs en parallèle et attendre la fin.
    /// Hors périmètre ici: priorités (P2), pause/stop (P3), logiciel métier/crypto mutex (P4).
    ///
    /// NOTE: BackupJob/strategies sont synchrones actuellement, donc on les exécute via Task.Run (adapter).
    /// </summary>
    public sealed class ParallelBackupCoordinator
    {
        private readonly ConcurrentDictionary<BackupJob, Task> _runningTasks = new();

        /// <summary>
        /// Expose une vue read-only des tâches en cours (utile plus tard pour P3).
        /// </summary>
        public IReadOnlyDictionary<BackupJob, Task> RunningTasks => _runningTasks;

        public BackupExecutionContext BuildExecutionContext(CancellationToken cancellationToken = default)
        {
            // P1: contexte minimal. P2/P3/P4 complèteront ce modèle.
            return new BackupExecutionContext(cancellationToken);
        }

        public Task ExecuteJobsInParallel(IEnumerable<BackupJob> jobs, CancellationToken cancellationToken = default)
        {
            var jobsSnapshot = jobs.ToList();
            var context = BuildExecutionContext(cancellationToken);
            return ExecuteJobsInParallel(jobsSnapshot, context);
        }

        public async Task ExecuteJobsInParallel(IReadOnlyCollection<BackupJob> jobsSnapshot, BackupExecutionContext context)
        {
            if (jobsSnapshot.Count == 0)
            {
                return;
            }

            var exceptions = new ConcurrentBag<Exception>();

            var tasks = jobsSnapshot.Select(job =>
            {
                var task = Task.Run(() =>
                {
                    try
                    {
                        job.Execute();
                    }
                    catch (Exception ex)
                    {
                        exceptions.Add(ex);
                    }
                }, context.CancellationToken);

                _runningTasks[job] = task;

                // Nettoyage quand terminé
                task.ContinueWith(_ =>
                {
                    _runningTasks.TryRemove(job, out Task? removedTask);
                }, TaskScheduler.Default);

                return task;
            }).ToArray();

            await Task.WhenAll(tasks);

            if (!exceptions.IsEmpty)
            {
                throw new AggregateException("One or more backup jobs failed during parallel execution.", exceptions);
            }
        }
    }
}
