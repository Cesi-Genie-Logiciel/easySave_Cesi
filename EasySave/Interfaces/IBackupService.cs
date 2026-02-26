using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        // ✅ V3/P1: Exécution parallèle (orchestration minimale)
        /// <summary>
        /// Exécute tous les jobs en parallèle. Implémentation minimale (P1) : snapshot + Task.WhenAll.
        /// Note: ne pas implémenter ici les règles P2/P3/P4 (priorités/pause/logiciel métier).
        /// </summary>
        Task ExecuteAllBackupJobsParallel();

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

        /// <summary>
        /// Reprend un job en pause
        /// </summary>
        void ResumeBackupJob(BackupJob job);

        /// <summary>
        /// Met en pause tous les jobs en cours
        /// </summary>
        void PauseAllBackupJobs();

        /// <summary>
        /// Reprend tous les jobs en pause
        /// </summary>
        void ResumeAllBackupJobs();

        /// <summary>
        /// Arrête tous les jobs en cours
        /// </summary>
        void StopAllBackupJobs();

        // Events pour notifier la GUI des changements
        event EventHandler<BackupJob>? JobCreated;
        event EventHandler<BackupJob>? JobDeleted;
        event EventHandler<BackupJob>? JobUpdated;
    }
}
