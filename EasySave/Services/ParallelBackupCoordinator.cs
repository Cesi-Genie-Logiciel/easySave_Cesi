using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EasySave.Models;

namespace EasySave.Services
{
    // Runs multiple backup jobs in parallel and provides methods to
    // pause, resume and stop individual jobs or all of them at once.
    public sealed class ParallelBackupCoordinator
    {
        private readonly ConcurrentDictionary<BackupJob, Task> _runningTasks = new();

        public IReadOnlyDictionary<BackupJob, Task> RunningTasks => _runningTasks;

        public BackupExecutionContext BuildExecutionContext(CancellationToken cancellationToken = default)
        {
            return new BackupExecutionContext(cancellationToken);
        }

        public Task ExecuteJobsInParallel(IEnumerable<BackupJob> jobs, CancellationToken cancellationToken = default)
        {
            List<BackupJob> jobsSnapshot = jobs.ToList();
            BackupExecutionContext context = BuildExecutionContext(cancellationToken);
            return ExecuteJobsInParallel(jobsSnapshot, context);
        }

        public async Task ExecuteJobsInParallel(IReadOnlyCollection<BackupJob> jobsSnapshot, BackupExecutionContext context)
        {
            if (jobsSnapshot.Count == 0)
                return;

            ConcurrentBag<Exception> exceptions = new ConcurrentBag<Exception>();

            Task[] tasks = jobsSnapshot.Select(job =>
            {
                Task task = Task.Run(async () =>
                {
                    try
                    {
                        await job.Execute(context);
                    }
                    catch (Exception ex)
                    {
                        exceptions.Add(ex);
                    }
                });

                _runningTasks[job] = task;

                task.ContinueWith(_ =>
                {
                    _runningTasks.TryRemove(job, out Task? removedTask);
                }, TaskScheduler.Default);

                return task;
            }).ToArray();

            await Task.WhenAll(tasks);

            if (!exceptions.IsEmpty)
                throw new AggregateException("One or more backup jobs failed during parallel execution.", exceptions);
        }

        // Delegates to the job's own Pause/Resume/Stop methods.
        // The job controls its own ManualResetEventSlim and CancellationTokenSource.
        public void PauseJob(BackupJob job) => job.Pause();
        public void ResumeJob(BackupJob job) => job.Resume();
        public void StopJob(BackupJob job) => job.Stop();

        public void PauseAll()
        {
            foreach (BackupJob job in _runningTasks.Keys)
                job.Pause();
        }

        public void ResumeAll()
        {
            foreach (BackupJob job in _runningTasks.Keys)
                job.Resume();
        }

        public void StopAll()
        {
            foreach (BackupJob job in _runningTasks.Keys)
                job.Stop();
        }
    }
}