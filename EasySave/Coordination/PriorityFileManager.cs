using System.Collections.Concurrent;

namespace EasySave.Coordination
{
	public class PriorityFileManager
	{
		private readonly List<string> _priorityExtensions;
		private readonly ConcurrentQueue<string> _priorityFiles = new();
		private readonly ConcurrentQueue<string> _nonPriorityFiles = new();
		private readonly ManualResetEventSlim _priorityCompleted = new(true);
		private readonly object _lock = new();

		public PriorityFileManager(List<string> priorityExtensions)
		{
			_priorityExtensions = priorityExtensions.Select(e => e.ToLower()).ToList();
		}

		public void ScanAndCategorize(List<string> files)
		{
			lock (_lock)
			{
				_priorityFiles.Clear();
				_nonPriorityFiles.Clear();

				foreach (string file in files)
				{
					string ext = Path.GetExtension(file).ToLower();
					if (_priorityExtensions.Contains(ext))
						_priorityFiles.Enqueue(file);
					else
						_nonPriorityFiles.Enqueue(file);
				}

				// S'il y a des fichiers prioritaires, on ferme la porte aux non-prioritaires
				if (!_priorityFiles.IsEmpty)
					_priorityCompleted.Reset();
			}
		}

		public bool HasPendingPriorityFiles() => !_priorityFiles.IsEmpty;

		public bool IsPriorityExtension(string filePath) =>
			_priorityExtensions.Contains(Path.GetExtension(filePath).ToLower());

		public string? DequeueNextFile()
		{
			// On prend d'abord dans les prioritaires
			if (_priorityFiles.TryDequeue(out string? priorityFile))
			{
				// Si la queue prioritaire est maintenant vide, on ouvre la porte
				if (_priorityFiles.IsEmpty)
					_priorityCompleted.Set();

				return priorityFile;
			}

			// Sinon on prend dans les non-prioritaires
			_nonPriorityFiles.TryDequeue(out string? nonPriorityFile);
			return nonPriorityFile;
		}

		public bool CanTransferNonPriority() => _priorityFiles.IsEmpty;

		// Bloque proprement sans consommer de ressources
		public void WaitForPriorityCompletion()
		{
			_priorityCompleted.Wait();
		}
	}
}