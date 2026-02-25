using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private string _largeFileThresholdKoText = "1024";
        private string _businessSoftwareName = "";
        private string? _selectedPriorityExtension;
        private string? _selectedEncryptExtension;

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

        public ObservableCollection<string> PriorityExtensionsList { get; } = new ObservableCollection<string>();
        public ObservableCollection<string> ExtensionsToEncryptList { get; } = new ObservableCollection<string>();

        public string? SelectedPriorityExtension
        {
            get => _selectedPriorityExtension;
            set => SetProperty(ref _selectedPriorityExtension, value);
        }

        public string? SelectedEncryptExtension
        {
            get => _selectedEncryptExtension;
            set => SetProperty(ref _selectedEncryptExtension, value);
        }

        private string? _selectedPriorityToRemove;
        private string? _selectedEncryptToRemove;

        public string? SelectedPriorityToRemove
        {
            get => _selectedPriorityToRemove;
            set => SetProperty(ref _selectedPriorityToRemove, value);
        }

        public string? SelectedEncryptToRemove
        {
            get => _selectedEncryptToRemove;
            set => SetProperty(ref _selectedEncryptToRemove, value);
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
        public ICommand ApplyCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand AddPriorityExtensionCommand { get; }
        public ICommand RemovePriorityExtensionCommand { get; }
        public ICommand AddEncryptExtensionCommand { get; }
        public ICommand RemoveEncryptExtensionCommand { get; }

        public IEnumerable<LogFormat> LogFormatValues => Enum.GetValues<LogFormat>();
        public string[] LogDestinationValues { get; } = { "Local", "Centralized", "Both" };
        public string[] AvailableExtensions { get; } = { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".jpg", ".jpeg", ".png", ".txt", ".zip", ".xml", ".json" };

        public SettingsViewModel(ISettingsService settingsService)
        {
            _settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));
            SaveCommand = new RelayCommand(Save);
            ApplyCommand = new RelayCommand(Apply);
            CancelCommand = new RelayCommand(Cancel);
            AddPriorityExtensionCommand = new RelayCommand(AddPriorityExtension);
            RemovePriorityExtensionCommand = new RelayCommand(RemovePriorityExtension);
            AddEncryptExtensionCommand = new RelayCommand(AddEncryptExtension);
            RemoveEncryptExtensionCommand = new RelayCommand(RemoveEncryptExtension);
            LoadFromSettings();
        }

        private void LoadFromSettings()
        {
            var s = _settingsService.GetCurrent();
            LogFormat = s.LogFormat;
            LogDestination = s.LogDestination ?? "Local";
            LogServerUrl = s.LogServerUrl ?? "http://localhost:5000";
            PriorityExtensionsList.Clear();
            if (s.PriorityExtensions != null)
                foreach (var ext in s.PriorityExtensions)
                    PriorityExtensionsList.Add(ext);
            ExtensionsToEncryptList.Clear();
            if (s.ExtensionsToEncrypt != null)
                foreach (var ext in s.ExtensionsToEncrypt)
                    ExtensionsToEncryptList.Add(ext);
            LargeFileThresholdKoText = s.LargeFileThresholdKo.ToString();
            BusinessSoftwareName = s.BusinessSoftwareName ?? "";
        }

        private void AddPriorityExtension(object? parameter)
        {
            if (string.IsNullOrEmpty(SelectedPriorityExtension)) return;
            var ext = SelectedPriorityExtension.Trim().ToLowerInvariant();
            if (!ext.StartsWith(".")) ext = "." + ext;
            if (!PriorityExtensionsList.Contains(ext))
                PriorityExtensionsList.Add(ext);
        }

        private void RemovePriorityExtension(object? parameter)
        {
            if (SelectedPriorityToRemove != null)
            {
                PriorityExtensionsList.Remove(SelectedPriorityToRemove);
                SelectedPriorityToRemove = null;
            }
        }

        private void AddEncryptExtension(object? parameter)
        {
            if (string.IsNullOrEmpty(SelectedEncryptExtension)) return;
            var ext = SelectedEncryptExtension.Trim().ToLowerInvariant();
            if (!ext.StartsWith(".")) ext = "." + ext;
            if (!ExtensionsToEncryptList.Contains(ext))
                ExtensionsToEncryptList.Add(ext);
        }

        private void RemoveEncryptExtension(object? parameter)
        {
            if (SelectedEncryptToRemove != null)
            {
                ExtensionsToEncryptList.Remove(SelectedEncryptToRemove);
                SelectedEncryptToRemove = null;
            }
        }

        private void PersistToSettings()
        {
            var s = _settingsService.GetCurrent();
            s.LogFormat = LogFormat;
            s.LogDestination = (LogDestination ?? "Local").Trim();
            s.LogServerUrl = LogServerUrl?.Trim() ?? "http://localhost:5000";
            s.PriorityExtensions = new List<string>(PriorityExtensionsList);
            s.ExtensionsToEncrypt = new List<string>(ExtensionsToEncryptList);
            s.LargeFileThresholdKo = long.TryParse(LargeFileThresholdKoText?.Trim(), out var ko) ? ko : 1024;
            s.BusinessSoftwareName = BusinessSoftwareName?.Trim() ?? "";
            try
            {
                _settingsService.Save(s);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Impossible d'enregistrer les paramètres : {ex.Message}", "Erreur",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                throw;
            }
        }

        private void Save(object? parameter)
        {
            PersistToSettings();
            CloseRequested?.Invoke(this, true);
        }

        private void Apply(object? parameter)
        {
            PersistToSettings();
            ApplyRequested?.Invoke(this, EventArgs.Empty);
        }

        private void Cancel(object? parameter)
        {
            CloseRequested?.Invoke(this, false);
        }

        public event Action<SettingsViewModel, bool>? CloseRequested;
        public event EventHandler? ApplyRequested;
    }
}
