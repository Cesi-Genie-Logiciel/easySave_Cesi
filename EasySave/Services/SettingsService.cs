using System;
using System.IO;
using System.Text.Json;
using EasySave.Interfaces;
using EasySave.Models;

namespace EasySave.Services
{
    /// <summary>
    /// Service de gestion des paramètres de l'application
    /// Charge et sauvegarde la configuration depuis/vers appsettings.json
    /// </summary>
    public class SettingsService : ISettingsService
    {
        private readonly string _settingsFilePath;
        private AppSettings _currentSettings;

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="settingsFilePath">Chemin du fichier de configuration (optionnel)</param>
        public SettingsService(string? settingsFilePath = null)
        {
            // Si aucun chemin n'est fourni, utiliser le chemin par défaut
            _settingsFilePath = settingsFilePath ?? 
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");

            // Charger les paramètres au démarrage
            _currentSettings = Load();
        }

        /// <summary>
        /// Charge les paramètres depuis le fichier appsettings.json
        /// </summary>
        public AppSettings Load()
        {
            try
            {
                if (!File.Exists(_settingsFilePath))
                {
                    Console.WriteLine($"⚠️  Configuration file not found: {_settingsFilePath}");
                    Console.WriteLine("Creating default configuration...");
                    
                    // Créer un fichier de configuration par défaut
                    var defaultSettings = new AppSettings();
                    Save(defaultSettings);
                    return defaultSettings;
                }

                string jsonContent = File.ReadAllText(_settingsFilePath);
                
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    WriteIndented = true
                };

                var settings = JsonSerializer.Deserialize<AppSettings>(jsonContent, options);
                
                if (settings == null)
                {
                    Console.WriteLine("⚠️  Failed to deserialize settings, using defaults");
                    return new AppSettings();
                }

                _currentSettings = settings;
                Console.WriteLine($"✅ Configuration loaded from: {_settingsFilePath}");
                Console.WriteLine($"   Log Format: {settings.LogFormat}");
                Console.WriteLine($"   Extensions to encrypt: {settings.ExtensionsToEncrypt.Count}");
                Console.WriteLine($"   Business Software: {(string.IsNullOrEmpty(settings.BusinessSoftwareName) ? "None" : settings.BusinessSoftwareName)}");
                
                return settings;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error loading settings: {ex.Message}");
                Console.WriteLine("Using default settings...");
                return new AppSettings();
            }
        }

        /// <summary>
        /// Sauvegarde les paramètres dans le fichier appsettings.json
        /// </summary>
        public void Save(AppSettings settings)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                string jsonContent = JsonSerializer.Serialize(settings, options);
                File.WriteAllText(_settingsFilePath, jsonContent);

                _currentSettings = settings;
                Console.WriteLine($"✅ Configuration saved to: {_settingsFilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error saving settings: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Obtient les paramètres actuellement chargés
        /// </summary>
        public AppSettings GetCurrent()
        {
            return _currentSettings;
        }

        /// <summary>
        /// Recharge les paramètres depuis le fichier
        /// </summary>
        public void Reload()
        {
            _currentSettings = Load();
        }
    }
}
