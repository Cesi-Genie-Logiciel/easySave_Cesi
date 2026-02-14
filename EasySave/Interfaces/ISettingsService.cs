using EasySave.Models;

namespace EasySave.Interfaces
{
    /// <summary>
    /// Interface pour le service de gestion des paramètres de l'application
    /// Permet de charger et sauvegarder la configuration depuis/vers appsettings.json
    /// </summary>
    public interface ISettingsService
    {
        /// <summary>
        /// Charge les paramètres depuis le fichier appsettings.json
        /// Si le fichier n'existe pas, crée un fichier avec les valeurs par défaut
        /// </summary>
        /// <returns>Les paramètres chargés</returns>
        AppSettings Load();

        /// <summary>
        /// Sauvegarde les paramètres dans le fichier appsettings.json
        /// </summary>
        /// <param name="settings">Les paramètres à sauvegarder</param>
        void Save(AppSettings settings);

        /// <summary>
        /// Obtient les paramètres actuellement chargés en mémoire
        /// </summary>
        /// <returns>Les paramètres actuels</returns>
        AppSettings GetCurrent();

        /// <summary>
        /// Recharge les paramètres depuis le fichier
        /// </summary>
        void Reload();
    }
}
