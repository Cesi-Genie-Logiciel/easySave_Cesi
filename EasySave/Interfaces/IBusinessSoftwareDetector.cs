namespace EasySave.Interfaces
{
    /// <summary>
    /// Interface pour la détection de logiciel métier (P4 - auto-pause).
    /// Permet de surveiller un processus et de notifier les changements d'état.
    /// </summary>
    public interface IBusinessSoftwareDetector
    {
        /// <summary>
        /// Nom du logiciel métier surveillé.
        /// </summary>
        string BusinessSoftwareName { get; }

        /// <summary>
        /// Vérifie si le logiciel métier est en cours d'exécution.
        /// </summary>
        bool IsBusinessSoftwareRunning();

        /// <summary>
        /// Démarre la surveillance (polling en arrière-plan).
        /// </summary>
        void StartMonitoring();

        /// <summary>
        /// Arrête la surveillance.
        /// </summary>
        void StopMonitoring();

        /// <summary>
        /// Événement déclenché quand l'état du logiciel métier change.
        /// Paramètre : true si le logiciel démarre, false s'il s'arrête.
        /// </summary>
        event Action<bool>? BusinessSoftwareStateChanged;
    }
}
