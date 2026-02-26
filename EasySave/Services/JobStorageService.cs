using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using EasySave.Interfaces;
using EasySave.Models;

namespace EasySave.Services
{
    public class JobStorageService : IJobStorageService
    {
        private readonly string _filePath;

        public JobStorageService(string? filePath = null)
        {
            if (filePath == null)
            {
                string folder = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EasySave");
                Directory.CreateDirectory(folder);
                _filePath = Path.Combine(folder, "jobs.json");
            }
            else
            {
                _filePath = filePath;
            }
        }

        public List<BackupConfig> LoadJobs()
        {
            if (!File.Exists(_filePath))
                return new List<BackupConfig>();

            try
            {
                string json = File.ReadAllText(_filePath);
                if (string.IsNullOrWhiteSpace(json))
                    return new List<BackupConfig>();

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<List<BackupConfig>>(json, options)
                       ?? new List<BackupConfig>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lecture jobs : {ex.Message}");
                return new List<BackupConfig>();
            }
        }

        public void SaveJobs(List<BackupConfig> jobs)
        {
            string? dir = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            string json = JsonSerializer.Serialize(jobs, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }
    }
}