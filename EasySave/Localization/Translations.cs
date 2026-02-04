namespace EasySave.Localization
{
    /// <summary>
    /// Translation system for French and English
    /// </summary>
    public static class Translations
    {
        private static string _currentLanguage = "fr";

        // Menu principal
        public static string MenuTitle => _currentLanguage == "fr" ? "MENU PRINCIPAL" : "MAIN MENU";
        public static string MenuCreate => _currentLanguage == "fr" ? "Créer un travail de sauvegarde" : "Create a backup job";
        public static string MenuList => _currentLanguage == "fr" ? "Lister les travaux" : "List jobs";
        public static string MenuExecuteOne => _currentLanguage == "fr" ? "Exécuter un travail spécifique" : "Execute a specific job";
        public static string MenuExecuteAll => _currentLanguage == "fr" ? "Exécuter tous les travaux" : "Execute all jobs";
        public static string MenuModify => _currentLanguage == "fr" ? "Modifier un travail" : "Modify a job";
        public static string MenuDelete => _currentLanguage == "fr" ? "Supprimer un travail" : "Delete a job";
        public static string MenuQuit => _currentLanguage == "fr" ? "Quitter" : "Quit";
        public static string YourChoice => _currentLanguage == "fr" ? "Votre choix" : "Your choice";

        // Messages généraux
        public static string PressKeyToContinue => _currentLanguage == "fr" ? "Appuyez sur une touche pour continuer..." : "Press any key to continue...";
        public static string InvalidChoice => _currentLanguage == "fr" ? "Choix invalide. Veuillez réessayer." : "Invalid choice. Please try again.";
        public static string Error => _currentLanguage == "fr" ? "Erreur" : "Error";
        public static string Goodbye => _currentLanguage == "fr" ? "Au revoir !" : "Goodbye!";

        // Création de travail
        public static string CreateJobTitle => _currentLanguage == "fr" ? "CRÉER UN NOUVEAU TRAVAIL" : "CREATE A NEW JOB";
        public static string JobName => _currentLanguage == "fr" ? "Nom du travail" : "Job name";
        public static string SourcePath => _currentLanguage == "fr" ? "Chemin source" : "Source path";
        public static string TargetPath => _currentLanguage == "fr" ? "Chemin destination" : "Target path";
        public static string BackupType => _currentLanguage == "fr" ? "Type de sauvegarde" : "Backup type";
        public static string Complete => _currentLanguage == "fr" ? "Complète" : "Complete";
        public static string Differential => _currentLanguage == "fr" ? "Différentielle" : "Differential";
        public static string JobCreated => _currentLanguage == "fr" ? "Travail créé avec succès" : "Job created successfully";
        public static string NameCannotBeEmpty => _currentLanguage == "fr" ? "Le nom ne peut pas être vide." : "Name cannot be empty.";
        public static string InvalidSourcePath => _currentLanguage == "fr" ? "Le chemin source est invalide ou n'existe pas." : "Source path is invalid or does not exist.";
        public static string InvalidTargetPath => _currentLanguage == "fr" ? "Le chemin destination ne peut pas être vide." : "Target path cannot be empty.";

        // Liste des travaux
        public static string ListJobsTitle => _currentLanguage == "fr" ? "LISTE DES TRAVAUX" : "JOB LIST";
        public static string NoJobs => _currentLanguage == "fr" ? "Aucun travail de sauvegarde configuré." : "No backup jobs configured.";
        public static string Type => _currentLanguage == "fr" ? "Type" : "Type";
        public static string Source => _currentLanguage == "fr" ? "Source" : "Source";
        public static string Destination => _currentLanguage == "fr" ? "Destination" : "Destination";
        public static string Total => _currentLanguage == "fr" ? "Total" : "Total";
        public static string Jobs => _currentLanguage == "fr" ? "travaux" : "jobs";

        // Exécution
        public static string ExecuteJobTitle => _currentLanguage == "fr" ? "EXÉCUTER UN TRAVAIL" : "EXECUTE A JOB";
        public static string ExecuteAllTitle => _currentLanguage == "fr" ? "EXÉCUTER TOUS LES TRAVAUX" : "EXECUTE ALL JOBS";
        public static string NoJobsToExecute => _currentLanguage == "fr" ? "Aucun travail à exécuter." : "No jobs to execute.";
        public static string JobNumber => _currentLanguage == "fr" ? "Numéro du travail à exécuter" : "Job number to execute";
        public static string InvalidNumber => _currentLanguage == "fr" ? "Numéro invalide." : "Invalid number.";
        public static string ConfirmExecute => _currentLanguage == "fr" ? "Confirmer l'exécution de {0} travaux ? (O/N)" : "Confirm execution of {0} jobs? (Y/N)";
        public static string ExecutionCancelled => _currentLanguage == "fr" ? "Exécution annulée." : "Execution cancelled.";
        public static string ExecutingJob => _currentLanguage == "fr" ? "Exécution du travail" : "Executing job";
        public static string BackupStarted => _currentLanguage == "fr" ? "Démarrage de la sauvegarde" : "Starting backup";
        public static string BackupCompleted => _currentLanguage == "fr" ? "Sauvegarde terminée" : "Backup completed";
        public static string AllJobsCompleted => _currentLanguage == "fr" ? "Exécution terminée pour tous les travaux." : "Execution completed for all jobs.";

        // Modification
        public static string ModifyJobTitle => _currentLanguage == "fr" ? "MODIFIER UN TRAVAIL" : "MODIFY A JOB";
        public static string NoJobsToModify => _currentLanguage == "fr" ? "Aucun travail à modifier." : "No jobs to modify.";
        public static string JobToModify => _currentLanguage == "fr" ? "Numéro du travail à modifier" : "Job number to modify";
        public static string ModifyingJob => _currentLanguage == "fr" ? "Modification de" : "Modifying";
        public static string PressEnterToKeep => _currentLanguage == "fr" ? "(Appuyez sur Entrée pour conserver la valeur actuelle)" : "(Press Enter to keep current value)";
        public static string NewName => _currentLanguage == "fr" ? "Nouveau nom" : "New name";
        public static string NewSourcePath => _currentLanguage == "fr" ? "Nouveau chemin source" : "New source path";
        public static string NewTargetPath => _currentLanguage == "fr" ? "Nouveau chemin destination" : "New target path";
        public static string CurrentType => _currentLanguage == "fr" ? "Type actuel" : "Current type";
        public static string NewType => _currentLanguage == "fr" ? "Nouveau type" : "New type";
        public static string JobModified => _currentLanguage == "fr" ? "Travail modifié" : "Job modified";

        // Suppression
        public static string DeleteJobTitle => _currentLanguage == "fr" ? "SUPPRIMER UN TRAVAIL" : "DELETE A JOB";
        public static string NoJobsToDelete => _currentLanguage == "fr" ? "Aucun travail à supprimer." : "No jobs to delete.";
        public static string JobToDelete => _currentLanguage == "fr" ? "Numéro du travail à supprimer" : "Job number to delete";
        public static string ConfirmDelete => _currentLanguage == "fr" ? "Confirmer la suppression de '{0}' ? (O/N)" : "Confirm deletion of '{0}'? (Y/N)";
        public static string DeletionCancelled => _currentLanguage == "fr" ? "Suppression annulée." : "Deletion cancelled.";
        public static string JobDeleted => _currentLanguage == "fr" ? "Travail supprimé." : "Job deleted.";

        // Progression
        public static string NoFilesToCopy => _currentLanguage == "fr" ? "Aucun fichier à copier (tous à jour)." : "No files to copy (all up to date).";

        // Méthode pour changer la langue
        public static void SetLanguage(string language)
        {
            if (language == "fr" || language == "en")
            {
                _currentLanguage = language;
            }
        }

        public static string GetCurrentLanguage()
        {
            return _currentLanguage;
        }
    }
}
