using System.Collections.Generic;
using EasySave.Models;

namespace EasySave.Interfaces
{
    /// <summary>
    /// Interface pour le service de stockage persistant des jobs
    /// Permet de sauvegarder et charger les configurations de jobs depuis un fichier JSON
    /// </summary>
    public interface IJobStorageService
    {
        /// <summary>
        /// Charge toutes les configurations de jobs depuis le fichier de stockage
        /// </summary>
        /// <returns>Liste des configurations de jobs</returns>
        List<BackupConfig> LoadJobs();

        /// <summary>
        /// Sauvegarde toutes les configurations de jobs dans le fichier de stockage
        /// </summary>
        /// <param name="jobs">Liste des configurations de jobs à sauvegarder</param>
        void SaveJobs(List<BackupConfig> jobs);

        /// <summary>
        /// Vérifie si le fichier de stockage existe
        /// </summary>
        /// <returns>True si le fichier existe, False sinon</returns>
        bool StorageExists();
    }
}
