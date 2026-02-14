using System;
using System.Threading.Tasks;
using EasySave.Models;

namespace EasySave.Services.Interfaces
{
    /// Contract for backup service implementations.
    /// Allows dependency injection and easy testing.
    public interface IBackupService
    {
        /// Executes a backup job asynchronously with progress reporting.
        Task ExecuteBackupAsync(BackupJob job, Action<int>? progressCallback = null);
        /// Validates that a job has valid configuration.
        bool ValidateJob(BackupJob job);
        /// Cancels a running backup job.
        void CancelJob(string jobName);
    }
}