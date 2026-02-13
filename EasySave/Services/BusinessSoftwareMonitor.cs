using System.Diagnostics;

namespace EasySave.Services
{
    /// <summary>
    /// Detects if a configured "business software" process is currently running.
    /// 
    /// For now, the process name is read from the environment variable:
    ///   EASY_SAVE_BUSINESS_PROCESS
    /// Example values: "calc", "notepad", "ged" (without extension).
    /// 
    /// This is intentionally isolated so switching to a real "general settings" store later is easy.
    /// </summary>
    public sealed class BusinessSoftwareMonitor
    {
        private readonly string? _processName;
        private readonly TimeSpan _minCheckInterval;

        private DateTime _lastCheckUtc = DateTime.MinValue;
        private bool _lastResult;

        public BusinessSoftwareMonitor(string? processName = null, TimeSpan? minCheckInterval = null)
        {
            _processName = NormalizeProcessName(
                string.IsNullOrWhiteSpace(processName)
                    ? Environment.GetEnvironmentVariable("EASY_SAVE_BUSINESS_PROCESS")
                    : processName);

            _minCheckInterval = minCheckInterval ?? TimeSpan.FromSeconds(1);
        }

        public bool IsConfigured => !string.IsNullOrWhiteSpace(_processName);

        public string? ProcessName => _processName;

        public bool IsRunning()
        {
            if (!IsConfigured)
                return false;

            var nowUtc = DateTime.UtcNow;
            if (nowUtc - _lastCheckUtc < _minCheckInterval)
                return _lastResult;

            _lastCheckUtc = nowUtc;

            try
            {
                // GetProcessesByName expects the name without extension.
                _lastResult = Process.GetProcessesByName(_processName).Length > 0;
            }
            catch
            {
                // If we can't query processes, fail "open" (do not block backups).
                _lastResult = false;
            }

            return _lastResult;
        }

        private static string? NormalizeProcessName(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            name = name.Trim();

            // If user provides "foo.exe", strip extension.
            if (name.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                name = Path.GetFileNameWithoutExtension(name);

            return name;
        }
    }
}
