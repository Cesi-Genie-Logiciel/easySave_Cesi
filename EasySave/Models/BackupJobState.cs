namespace EasySave.Models
{
    /// <summary>
    /// État d'un job de sauvegarde (conforme schéma)
    /// </summary>
    public enum BackupJobState
    {
        Pending,
        Running,
        Paused,
        Stopped,
        Completed,
        Error
    }
}
