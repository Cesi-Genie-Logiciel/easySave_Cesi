using System;
using System.Collections.Generic;
using EasySave.Models;

namespace EasySave.Interfaces
{
    public interface IBackupService
    {
        // Méthodes existantes (v1.0)
        void CreateBackupJob(string name, string source, string target, string type);
        List<BackupJob> GetAllBackupJobs();
        void ExecuteBackupJob(int index);
        void ExecuteMultipleBackupJobs(List<int> indices);
        void DeleteBackupJob(int index);
        
        // ✅ FEATURE P2: Nouvelles méthodes pour GUI/MVVM (v2.0)
        
        /// <summary>
        /// Obtient un job par son index
        /// </summary>
        BackupJob? GetJobByIndex(int index);
        
        /// <summary>
        /// Obtient un job par son nom
        /// </summary>
        BackupJob? GetJobByName(string name);
        
        /// <summary>
        /// Met à jour un job existant
        /// </summary>
        void UpdateBackupJob(int index, string name, string source, string target, string type);
        
        /// <summary>
        /// Met en pause un job en cours d'exécution
        /// </summary>
        void PauseBackupJob(BackupJob job);
        
        /// <summary>
        /// Arrête un job en cours d'exécution
        /// </summary>
        void StopBackupJob(BackupJob job);
        
        // Events pour notifier la GUI des changements
        event EventHandler<BackupJob>? JobCreated;
        event EventHandler<BackupJob>? JobDeleted;
        event EventHandler<BackupJob>? JobUpdated;
    }
}
