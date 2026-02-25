using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using EasySave.GUI.Commands;
using EasySave.Interfaces;
using EasySave.Models;

namespace EasySave.GUI.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        private readonly ISettingsService _settingsService;
        private LogFormat _logFormat;
        private string _logDestination = "Local";
        private string _logServerUrl = "http://localhost:5000";
        private string _priorityExtensionsText = "";
        private string _extensionsToEncryptText = "";
        private string _largeFileThresholdKoText = "1024";
        private string _businessSoftwareName = "";

        public LogFormat LogFormat
        {
            get => _logFormat;
            set => SetProperty(ref _logFormat, value);
        }

        public string LogDestination
        {
            get => _logDestination;
            set => SetProperty(ref _logDestination, value);
        }

        public string LogServerUrl
        {
            get => _logServerUrl;
            set => SetProperty(ref _logServerUrl, value);
        }

        public string PriorityExtensionsText
        {
            get => _priorityExtensionsText;
            set => SetProperty(ref _priorityExtensionsText, value);
        }

        public string ExtensionsToEncryptText
        {
            get => _extensionsToEncryptText;
            set => SetProperty(ref _extensionsToEncryptText, value);
        }

        public string LargeFileThresholdKoText
        {
            get => _largeFileThresholdKoText;
            set => SetProperty(ref _largeFileThresholdKoText, value);
        }

        public string BusinessSoftwareName
        {
            get => _businessSoftwareName;
            set => SetProperty(ref _businessSoftwareName, value);
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public IEnumerable<LogFormat> LogFormatValues => Enum.GetValues<LogFormat>();

        public SettingsViewModel(ISettingsService settingsService)
        {
            _settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));
            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);
            LoadFromSettings();
        }

        private void LoadFromSettings()
        {
            var s = _settingsService.GetCurrent();
            LogFormat = s.LogFormat;
            LogDestination = s.LogDestination ?? "Local";
            LogServerUrl = s.LogServerUrl ?? "http://localhost:5000";
            PriorityExtensionsText = s.PriorityExtensions != null ? string.Join("\r\n", s.PriorityExtensions) : "";
            ExtensionsToEncryptText = s.ExtensionsToEncrypt != null ? string.Join("\r\n", s.ExtensionsToEncrypt) : "";
            LargeFileThresholdKoText = s.LargeFileThresholdKo.ToString();
            BusinessSoftwareName = s.BusinessSoftwareName ?? "";
        }

        private void Save(object? parameter)
        {
            var s = _settingsService.GetCurrent();
            s.LogFormat = LogFormat;
            s.LogDestination = LogDestination.Trim();
            s.LogServerUrl = LogServerUrl?.Trim() ?? "http://localhost:5000";
            s.PriorityExtensions = SplitLines(PriorityExtensionsText);
            s.ExtensionsToEncrypt = SplitLines(ExtensionsToEncryptText);
            s.LargeFileThresholdKo = long.TryParse(LargeFileThresholdKoText?.Trim(), out var ko) ? ko : 1024;
            s.BusinessSoftwareName = BusinessSoftwareName?.Trim() ?? "";
            _settingsService.Save(s);
            CloseRequested?.Invoke(this, true);
        }

        private void Cancel(object? parameter)
        {
            CloseRequested?.Invoke(this, false);
        }

        private static List<string> SplitLines(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return new List<string>();
            return text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => x.Length > 0)
                .ToList();
        }

        /// <summary>
        /// Raised when the window should close (true = saved, false = cancelled).
        /// </summary>
        public event Action<SettingsViewModel, bool>? CloseRequested;
    }
}
