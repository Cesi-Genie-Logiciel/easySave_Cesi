using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using EasyLog.Interfaces;

namespace EasyLog.Implementation
{
	public class Logger : ILogger
	{
		// Singleton pattern
		private static Logger _instance = null;
		private static readonly object _lock = new object();

		private string _logFilePath;
		private string _stateFilePath;

		// Constructeur privé
		private Logger()
		{
			string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			string easySaveFolder = Path.Combine(appDataPath, "ProSoft", "EasySave");

			Directory.CreateDirectory(easySaveFolder);

			_logFilePath = Path.Combine(easySaveFolder, $"log_{DateTime.Now:yyyy-MM-dd}.json");
			_stateFilePath = Path.Combine(easySaveFolder, "state.json");
		}

		// Point d'accès unique (thread-safe)
		public static Logger Instance
		{
			get
			{
				if (_instance == null)
				{
					lock (_lock)
					{
						if (_instance == null)
						{
							_instance = new Logger();
						}
					}
				}
				return _instance;
			}
		}

		public void LogFileTransfer(string backupName, string sourceFile, string destFile, long fileSize, double duration)
		{
			var logEntry = new
			{
				timestamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"),
				backupName,
				sourceFile,
				destFile,
				fileSize,
				transferTimeMs = duration
			};

			lock (_lock)
			{
				var options = new JsonSerializerOptions { WriteIndented = true };
				string json = JsonSerializer.Serialize(logEntry, options);
				File.AppendAllText(_logFilePath, json + ",\n");
			}
		}

		public void UpdateState(object state)
		{
			lock (_lock)
			{
				var options = new JsonSerializerOptions { WriteIndented = true };
				string json = JsonSerializer.Serialize(state, options);
				File.WriteAllText(_stateFilePath, json);
			}
		}

		public void UpdateState(List<object> states)
		{
			lock (_lock)
			{
				var options = new JsonSerializerOptions { WriteIndented = true };
				string json = JsonSerializer.Serialize(states, options);
				File.WriteAllText(_stateFilePath, json);
			}
		}
	}
}