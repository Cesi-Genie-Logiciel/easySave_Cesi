using System;
using System.IO;
using System.Text.Json;
using EasySave.Interfaces;
using EasySave.Models;

namespace EasySave.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly string _filePath;
        private AppSettings _current;

        public SettingsService(string? filePath = null)
        {
            _filePath = filePath ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
            _current = Load();
        }

        public AppSettings Load()
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    var defaults = new AppSettings();
                    Save(defaults);
                    return defaults;
                }

                string json = File.ReadAllText(_filePath);
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var settings = JsonSerializer.Deserialize<AppSettings>(json, options);
                _current = settings ?? new AppSettings();
                return _current;
            }
            catch
            {
                return new AppSettings();
            }
        }

        public void Save(AppSettings settings)
        {
            var dir = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
            _current = settings;
        }

        public AppSettings GetCurrent() => _current;
        public void Reload() => _current = Load();
    }
}