using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EasySave.Models;

namespace EasySave.Interfaces
{
    // Service layer that manages backup jobs.
    // The GUI and CLI both use this interface to create, execute and control jobs.
    public interface IBackupService
    {
        void CreateBackupJob(string name, string source, string target, string type);
        List<BackupJob> GetAllBackupJobs();
        void ExecuteBackupJob(int index);
        void ExecuteMultipleBackupJobs(List<int> indices);
        void DeleteBackupJob(int index);
        Task ExecuteAllBackupJobsParallel();

        BackupJob GetJobByIndex(int index);
        BackupJob GetJobByName(string name);
        void UpdateBackupJob(int index, string name, string source, string target, string type);

        // Play/Pause/Stop controls for individual jobs
        void PauseBackupJob(BackupJob job);
        void ResumeBackupJob(BackupJob job);
        void StopBackupJob(BackupJob job);

        // Batch controls for all running jobs
        void PauseAllBackupJobs();
        void ResumeAllBackupJobs();
        void StopAllBackupJobs();

        event EventHandler<BackupJob>? JobCreated;
        event EventHandler<BackupJob>? JobDeleted;
        event EventHandler<BackupJob>? JobUpdated;
    }
}