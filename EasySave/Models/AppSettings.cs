using System.Collections.Generic;

namespace EasySave.Models
{
    /// <summary>
    /// Enumération des formats de log supportés
    /// </summary>
    public enum LogFormat
    {
        JSON,
        XML
    }

    /// <summary>
    /// Configuration globale de l'application EasySave
    /// Utilisée par P3 (EasyLog XML/JSON) et P4 (Cryptage + Logiciel métier)
    /// </summary>
    public class AppSettings
    {
        /// <summary>
        /// Format de log à utiliser (JSON ou XML)
        /// Par défaut: JSON
        /// </summary>
        public LogFormat LogFormat { get; set; } = LogFormat.JSON;

        /// <summary>
        /// Liste des extensions de fichiers à crypter
        /// Exemple: [".doc", ".docx", ".pdf"]
        /// </summary>
        public List<string> ExtensionsToEncrypt { get; set; } = new List<string>();

        /// <summary>
        /// Nom du logiciel métier à détecter
        /// Si ce processus est actif, les sauvegardes sont mises en pause
        /// </summary>
        public string BusinessSoftwareName { get; set; } = "";

        /// <summary>
        /// Destination des logs : Local, Centralized, Both (P4)
        /// </summary>
        public string LogDestination { get; set; } = "Local";

        /// <summary>
        /// URL du serveur LogCentralizer pour mode Centralized ou Both (P4)
        /// </summary>
        public string LogServerUrl { get; set; } = "http://localhost:5000";

        /// <summary>
        /// Extensions de fichiers prioritaires (transfert avant les autres en parallèle)
        /// </summary>
        public List<string> PriorityExtensions { get; set; } = new List<string>();

        /// <summary>
        /// Seuil en Ko au-delà duquel un fichier est considéré comme "gros" (un seul transfert à la fois)
        /// </summary>
        public long LargeFileThresholdKo { get; set; } = 1024;

        /// <summary>
        /// Constructeur par défaut avec valeurs initiales
        /// </summary>
        public AppSettings()
        {
            // Configuration par défaut
        }
    }
}
