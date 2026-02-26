using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EasySave.GUI
{
    // Handles all translations for the application.
    // Works as a shared resource so every view can bind to it
    // and refresh instantly when the user switches language.
    public class LanguageManager : INotifyPropertyChanged
    {
        private static LanguageManager? _instance;
        public static LanguageManager Instance => _instance ?? new LanguageManager();

        private string _currentLanguage = "fr";

        // Dictionary that holds all labels grouped by language code
        private Dictionary<string, Dictionary<string, string>> _translations;

        public event PropertyChangedEventHandler? PropertyChanged;

        public LanguageManager()
        {
            _instance = this;
            _translations = new Dictionary<string, Dictionary<string, string>>
            {
                ["fr"] = new Dictionary<string, string>
                {
                    // Main window
                    ["AppTitle"] = "EasySave v3.0 - Gestion des Sauvegardes",
                    ["NewJob"] = "Nouveau Job",
                    ["Edit"] = "Modifier",
                    ["RunSelected"] = "Lancer",
                    ["RunAll"] = "Lancer Tous",
                    ["Delete"] = "Supprimer",
                    ["Refresh"] = "Actualiser",
                    ["Settings"] = "Parametres",
                    ["JobName"] = "Nom du Job",
                    ["SourcePath"] = "Chemin Source",
                    ["TargetPath"] = "Chemin Cible",
                    ["Type"] = "Type",
                    ["State"] = "Etat",
                    ["Progress"] = "Progression",
                    ["Actions"] = "Actions",
                    ["GlobalProgress"] = "Progression Globale:",
                    ["Ready"] = "Pret",
                    ["JobsLoaded"] = "job(s) charge(s)",
                    ["JobCreated"] = "Job cree",
                    ["JobModified"] = "Job modifie",
                    ["JobDeleted"] = "Job supprime",
                    ["Running"] = "Execution",
                    ["AllDone"] = "Execution terminee",
                    ["Error"] = "Erreur",
                    ["ErrorLoading"] = "Erreur chargement",
                    ["ErrorExecution"] = "Erreur execution",
                    ["ConfirmDelete"] = "Supprimer ce job ?",
                    ["Confirmation"] = "Confirmation",
                    ["Language"] = "Langue",

                    // Detail panel
                    ["JobDetails"] = "Details du Job",
                    ["NameLabel"] = "Nom :",
                    ["SourceLabel"] = "Source :",
                    ["TargetLabel"] = "Cible :",
                    ["TypeLabel"] = "Type :",
                    ["StateLabel"] = "Etat :",
                    ["ProgressLabel"] = "Progression :",
                    ["FilesLabel"] = "Fichiers :",
                    ["Play"] = "Lancer",
                    ["Pause"] = "Pause",
                    ["Stop"] = "Arreter",

                    // Job states
                    ["Stopped"] = "Arrete",
                    ["InProgress"] = "En cours",
                    ["Completed"] = "Termine",

                    // Create/Edit dialog
                    ["CreateJobTitle"] = "Creer un Job de Sauvegarde",
                    ["EditJobTitle"] = "Modifier le Job de Sauvegarde",
                    ["JobNameField"] = "Nom du Job:",
                    ["SourcePathField"] = "Chemin Source:",
                    ["TargetPathField"] = "Chemin Cible:",
                    ["BackupTypeField"] = "Type de Sauvegarde:",
                    ["Complete"] = "Complete",
                    ["Differential"] = "Differentielle",
                    ["Browse"] = "Parcourir",
                    ["Create"] = "Creer",
                    ["Save"] = "Enregistrer",
                    ["Cancel"] = "Annuler",
                    ["ValidationName"] = "Le nom du job est requis.",
                    ["ValidationSource"] = "Le chemin source est requis.",
                    ["ValidationTarget"] = "Le chemin cible est requis.",
                    ["Validation"] = "Validation",

                    // Settings window
                    ["SettingsTitle"] = "Parametres - EasySave",
                    ["LogFormat"] = "Format des logs:",
                    ["LogDestination"] = "Destination des logs:",
                    ["LogServerUrl"] = "URL serveur logs (Docker):",
                    ["PriorityExtensions"] = "Extensions prioritaires (choisir puis Ajouter):",
                    ["EncryptExtensions"] = "Extensions a chiffrer (choisir puis Ajouter):",
                    ["Add"] = "Ajouter",
                    ["RemoveSelected"] = "Retirer la selection",
                    ["LargeFileThreshold"] = "Seuil fichier gros (Ko):",
                    ["BusinessSoftware"] = "Ne pas lancer de sauvegarde si cette application est ouverte (optionnel)",
                    ["BusinessSoftwareHint"] = "Ex. : Excel, notepad - laisser vide pour desactiver.",
                    ["Apply"] = "Appliquer",
                    ["SettingsSaved"] = "Parametres appliques.",

                    // Folder dialog
                    ["SelectSourceFolder"] = "Selectionnez le dossier source",
                    ["SelectTargetFolder"] = "Selectionnez le dossier cible",
                    ["FolderNotFound"] = "Le dossier n'existe pas",
                    ["CannotOpenFolder"] = "Impossible d'ouvrir le dossier"
                },

                ["en"] = new Dictionary<string, string>
                {
                    // Main window
                    ["AppTitle"] = "EasySave v3.0 - Backup Manager",
                    ["NewJob"] = "New Job",
                    ["Edit"] = "Edit",
                    ["RunSelected"] = "Run",
                    ["RunAll"] = "Run All",
                    ["Delete"] = "Delete",
                    ["Refresh"] = "Refresh",
                    ["Settings"] = "Settings",
                    ["JobName"] = "Job Name",
                    ["SourcePath"] = "Source Path",
                    ["TargetPath"] = "Target Path",
                    ["Type"] = "Type",
                    ["State"] = "State",
                    ["Progress"] = "Progress",
                    ["Actions"] = "Actions",
                    ["GlobalProgress"] = "Global Progress:",
                    ["Ready"] = "Ready",
                    ["JobsLoaded"] = "job(s) loaded",
                    ["JobCreated"] = "Job created",
                    ["JobModified"] = "Job modified",
                    ["JobDeleted"] = "Job deleted",
                    ["Running"] = "Running",
                    ["AllDone"] = "Execution completed",
                    ["Error"] = "Error",
                    ["ErrorLoading"] = "Loading error",
                    ["ErrorExecution"] = "Execution error",
                    ["ConfirmDelete"] = "Delete this job?",
                    ["Confirmation"] = "Confirmation",
                    ["Language"] = "Language",

                    // Detail panel
                    ["JobDetails"] = "Job Details",
                    ["NameLabel"] = "Name:",
                    ["SourceLabel"] = "Source:",
                    ["TargetLabel"] = "Target:",
                    ["TypeLabel"] = "Type:",
                    ["StateLabel"] = "State:",
                    ["ProgressLabel"] = "Progress:",
                    ["FilesLabel"] = "Files:",
                    ["Play"] = "Play",
                    ["Pause"] = "Pause",
                    ["Stop"] = "Stop",

                    // Job states
                    ["Stopped"] = "Stopped",
                    ["InProgress"] = "Running",
                    ["Completed"] = "Completed",

                    // Create/Edit dialog
                    ["CreateJobTitle"] = "Create a Backup Job",
                    ["EditJobTitle"] = "Edit Backup Job",
                    ["JobNameField"] = "Job Name:",
                    ["SourcePathField"] = "Source Path:",
                    ["TargetPathField"] = "Target Path:",
                    ["BackupTypeField"] = "Backup Type:",
                    ["Complete"] = "Complete",
                    ["Differential"] = "Differential",
                    ["Browse"] = "Browse",
                    ["Create"] = "Create",
                    ["Save"] = "Save",
                    ["Cancel"] = "Cancel",
                    ["ValidationName"] = "Job name is required.",
                    ["ValidationSource"] = "Source path is required.",
                    ["ValidationTarget"] = "Target path is required.",
                    ["Validation"] = "Validation",

                    // Settings window
                    ["SettingsTitle"] = "Settings - EasySave",
                    ["LogFormat"] = "Log format:",
                    ["LogDestination"] = "Log destination:",
                    ["LogServerUrl"] = "Log server URL (Docker):",
                    ["PriorityExtensions"] = "Priority extensions (select then Add):",
                    ["EncryptExtensions"] = "Extensions to encrypt (select then Add):",
                    ["Add"] = "Add",
                    ["RemoveSelected"] = "Remove selected",
                    ["LargeFileThreshold"] = "Large file threshold (KB):",
                    ["BusinessSoftware"] = "Do not run backups if this application is open (optional)",
                    ["BusinessSoftwareHint"] = "e.g.: Excel, notepad - leave empty to disable.",
                    ["Apply"] = "Apply",
                    ["SettingsSaved"] = "Settings applied.",

                    // Folder dialog
                    ["SelectSourceFolder"] = "Select the source folder",
                    ["SelectTargetFolder"] = "Select the target folder",
                    ["FolderNotFound"] = "Folder does not exist",
                    ["CannotOpenFolder"] = "Cannot open folder"
                }
            };
        }

        // Current language code (fr or en)
        public string CurrentLanguage
        {
            get => _currentLanguage;
            set
            {
                if (_currentLanguage == value) return;
                _currentLanguage = value;

                // "Item[]" tells WPF that every indexer binding needs to refresh.
                // Without this specific name, the XAML bindings like {Binding [AppTitle]}
                // would never update when the language changes.
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item[]"));
            }
        }

        // Indexer used by XAML bindings, for example: {Binding [AppTitle]}
        public string this[string key]
        {
            get
            {
                if (_translations.ContainsKey(_currentLanguage)
                    && _translations[_currentLanguage].ContainsKey(key))
                {
                    return _translations[_currentLanguage][key];
                }

                // Fallback to french if key is missing
                if (_translations.ContainsKey("fr")
                    && _translations["fr"].ContainsKey(key))
                {
                    return _translations["fr"][key];
                }

                return key;
            }
        }

        // Shorthand for code-behind usage (same as indexer)
        public string Translate(string key)
        {
            return this[key];
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}