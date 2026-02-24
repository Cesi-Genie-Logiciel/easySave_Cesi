using System.Diagnostics;
using EasySave.Interfaces;

namespace EasySave.Services
{
    /// <summary>
    /// Détecte si un logiciel métier configuré est en cours d'exécution.
    /// Implémente IBusinessSoftwareDetector avec surveillance active (polling) et événements.
    /// P4 - feat/v3-business-software-auto-pause
    /// </summary>
    public sealed class BusinessSoftwareDetector : IBusinessSoftwareDetector, IDisposable
    {
        private readonly string _processName;
        private readonly int _pollingIntervalMs;
        private CancellationTokenSource? _cancellationTokenSource;
        private Task? _monitoringTask;
        private bool _lastKnownState;
        private readonly object _lock = new object();

        public string BusinessSoftwareName => _processName;

        public event Action<bool>? BusinessSoftwareStateChanged;

        public BusinessSoftwareDetector(string processName, int pollingIntervalMs = 1000)
        {
            if (string.IsNullOrWhiteSpace(processName))
                throw new ArgumentException("Process name cannot be empty", nameof(processName));

            _processName = NormalizeProcessName(processName);
            _pollingIntervalMs = pollingIntervalMs;
            _lastKnownState = false;
        }

        public bool IsBusinessSoftwareRunning()
        {
            try
            {
                return Process.GetProcessesByName(_processName).Length > 0;
            }
            catch
            {
                return false;
            }
        }

        public void StartMonitoring()
        {
            lock (_lock)
            {
                if (_monitoringTask != null)
                    return;

                _cancellationTokenSource = new CancellationTokenSource();
                _monitoringTask = Task.Run(() => MonitoringLoop(_cancellationTokenSource.Token));
            }
        }

        public void StopMonitoring()
        {
            lock (_lock)
            {
                if (_cancellationTokenSource == null)
                    return;

                _cancellationTokenSource.Cancel();
                _monitoringTask?.Wait(TimeSpan.FromSeconds(5));
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
                _monitoringTask = null;
            }
        }

        private async Task MonitoringLoop(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    bool currentState = IsBusinessSoftwareRunning();

                    if (currentState != _lastKnownState)
                    {
                        _lastKnownState = currentState;
                        BusinessSoftwareStateChanged?.Invoke(currentState);
                    }

                    await Task.Delay(_pollingIntervalMs, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch
                {
                }
            }
        }

        private static string NormalizeProcessName(string name)
        {
            name = name.Trim();

            if (name.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                name = Path.GetFileNameWithoutExtension(name);

            return name;
        }

        public void Dispose()
        {
            StopMonitoring();
        }
    }
}
