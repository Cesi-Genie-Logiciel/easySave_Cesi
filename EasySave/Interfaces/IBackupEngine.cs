using EasySave.Models;

namespace EasySave.Interfaces
{
    /// <summary>
    /// Interface for the main backup engine
    /// Stabilizes API across versions (v1.0, v1.1, v2.0...)
    /// </summary>
    public interface IBackupEngine
    {
        /// <summary>
        /// Create a new backup job
        /// </summary>
        void CreateJob(string name, string sourcePath, string targetPath, string backupType);

        /// <summary>
        /// Execute a specific backup job by index
        /// </summary>
        void ExecuteJob(int jobIndex);

        /// <summary>
        /// Execute all backup jobs sequentially
        /// </summary>
        void ExecuteAllJobs();

        /// <summary>
        /// Get list of all backup jobs
        /// </summary>
        List<BackupConfig> GetAllJobs();

        /// <summary>
        /// Delete a backup job
        /// </summary>
        void DeleteJob(int jobIndex);

        /// <summary>
        /// Modify an existing backup job
        /// </summary>
        void ModifyJob(int jobIndex, string name, string sourcePath, string targetPath, string backupType);
    }
}
