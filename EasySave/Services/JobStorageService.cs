using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using EasySave.Interfaces;
using EasySave.Models;

namespace EasySave.Services
{
    /// <summary>
    /// Service de stockage persistant des jobs dans un fichier JSON
    /// Permet de sauvegarder les jobs et de les recharger au démarrage
    /// </summary>
    public class JobStorageService : IJobStorageService
    {
        private readonly string _storageFilePath;

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="storageFilePath">Chemin du fichier de stockage (optionnel)</param>
        public JobStorageService(string? storageFilePath = null)
        {
            // Si aucun chemin n'est fourni, utiliser %APPDATA%/EasySave/jobs.json
            if (storageFilePath == null)
            {
                string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string easySaveFolder = Path.Combine(appDataPath, "EasySave");
                Directory.CreateDirectory(easySaveFolder);
                _storageFilePath = Path.Combine(easySaveFolder, "jobs.json");
            }
            else
            {
                _storageFilePath = storageFilePath;
            }
        }

        /// <summary>
        /// Charge toutes les configurations de jobs depuis le fichier
        /// </summary>
        public List<BackupConfig> LoadJobs()
        {
            try
            {
                if (!File.Exists(_storageFilePath))
                {
                    Console.WriteLine($"⚠️  No saved jobs found at: {_storageFilePath}");
                    return new List<BackupConfig>();
                }

                string jsonContent = File.ReadAllText(_storageFilePath);
                
                if (string.IsNullOrWhiteSpace(jsonContent))
                {
                    return new List<BackupConfig>();
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    WriteIndented = true
                };

                var jobs = JsonSerializer.Deserialize<List<BackupConfig>>(jsonContent, options);
                
                if (jobs == null)
                {
                    Console.WriteLine("⚠️  Failed to deserialize jobs");
                    return new List<BackupConfig>();
                }

                Console.WriteLine($"✅ Loaded {jobs.Count} job(s) from: {_storageFilePath}");
                return jobs;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error loading jobs: {ex.Message}");
                return new List<BackupConfig>();
            }
        }

        /// <summary>
        /// Sauvegarde toutes les configurations de jobs dans le fichier
        /// </summary>
        public void SaveJobs(List<BackupConfig> jobs)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                string jsonContent = JsonSerializer.Serialize(jobs, options);
                
                // Créer le répertoire si nécessaire
                string? directory = Path.GetDirectoryName(_storageFilePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.WriteAllText(_storageFilePath, jsonContent);
                Console.WriteLine($"✅ Saved {jobs.Count} job(s) to: {_storageFilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error saving jobs: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Vérifie si le fichier de stockage existe
        /// </summary>
        public bool StorageExists()
        {
            return File.Exists(_storageFilePath);
        }
    }
}
