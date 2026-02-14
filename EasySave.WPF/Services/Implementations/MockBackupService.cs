using System;
using System.Threading.Tasks;
using EasySave.Models;
using EasySave.Services.Interfaces;

namespace EasySave.Services.Implementations
{
    /// Mock implementation of backup service for P1.
    /// Simulates backup execution with artificial delays.
    /// Will be replaced with real implementation in P2.
    public class MockBackupService : IBackupService
    {
        public async Task ExecuteBackupAsync(BackupJob job, Action<int>? progressCallback = null)
        {
            if (!ValidateJob(job))
            {
                throw new InvalidOperationException($"Job '{job.Name}' is not valid.");
            }

            // Simulate backup with 5 seconds duration
            for (int progress = 0; progress <= 100; progress += 10)
            {
                await Task.Delay(500);
                progressCallback?.Invoke(progress);
            }

            await Task.Delay(200);
        }

        public bool ValidateJob(BackupJob job)
        {
            if (job == null) return false;
            if (string.IsNullOrWhiteSpace(job.Name)) return false;
            if (string.IsNullOrWhiteSpace(job.SourcePath)) return false;
            if (string.IsNullOrWhiteSpace(job.TargetPath)) return false;
            if (string.IsNullOrWhiteSpace(job.BackupType)) return false;
            return true;
        }

        public void CancelJob(string jobName)
        {
            // Not implemented in P1, will use CancellationToken in P2
        }
    }
}