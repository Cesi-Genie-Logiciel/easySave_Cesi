namespace EasySave.Interfaces
{
    public interface IBusinessSoftwareDetector
    {
        string BusinessSoftwareName { get; }
        bool IsBusinessSoftwareRunning();
        void StartMonitoring();
        void StopMonitoring();
        event Action<bool>? BusinessSoftwareStateChanged;
    }
}
