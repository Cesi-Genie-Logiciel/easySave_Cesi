using System.Collections.Concurrent;
using System.IO;

namespace EasySave.Coordination
{
	/// <summary>
	/// Manages file transfer priority during parallel backup execution.
	/// Files with priority extensions are always transferred before non-priority files.
	/// As long as priority files are pending on any job, non-priority transfers are blocked.
	/// </summary>
	public class PriorityFileManager
	{
		// List of file extensions considered as priority (e.g. ".pdf", ".docx")
		private readonly List<string> _priorityExtensions;

		// Queue containing files with priority extensions, processed first
		private readonly ConcurrentQueue<string> _priorityFiles = new();

		// Queue containing non-priority files, processed only when priority queue is empty
		private readonly ConcurrentQueue<string> _nonPriorityFiles = new();

		// Gate that blocks non-priority transfers while priority files are pending
		// Starts open (true) and closes when priority files are detected
		private readonly ManualResetEventSlim _priorityCompleted = new(true);

		// Lock used to ensure thread-safe categorization during scan
		private readonly object _lock = new();

		/// <summary>
		/// Initializes the manager with the list of priority extensions.
		/// Extensions are normalized to lowercase for case-insensitive comparison.
		/// </summary>
		/// <param name="priorityExtensions">List of extensions to treat as priority (e.g. ".pdf")</param>
		public PriorityFileManager(List<string> priorityExtensions)
		{
			_priorityExtensions = priorityExtensions.Select(e => e.ToLower()).ToList();
		}

		/// <summary>
		/// Scans the provided file list and categorizes each file as priority or non-priority.
		/// Clears any previously queued files before scanning.
		/// Closes the priority gate if priority files are found.
		/// </summary>
		/// <param name="files">Full list of files to be transferred in this backup job</param>
		public void ScanAndCategorize(List<string> files)
		{
			lock (_lock)
			{
				// Clear previous state before scanning
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

				// If priority files exist, close the gate to block non-priority transfers
				if (!_priorityFiles.IsEmpty)
					_priorityCompleted.Reset();
			}
		}

		/// <summary>
		/// Returns true if there are still priority files waiting to be transferred.
		/// </summary>
		public bool HasPendingPriorityFiles() => !_priorityFiles.IsEmpty;

		/// <summary>
		/// Returns true if the given file path has a priority extension.
		/// </summary>
		/// <param name="filePath">Full path of the file to check</param>
		public bool IsPriorityExtension(string filePath) =>
			_priorityExtensions.Contains(Path.GetExtension(filePath).ToLower());

		/// <summary>
		/// Dequeues and returns the next file to transfer.
		/// Priority files are always returned first.
		/// When the priority queue becomes empty, the gate is opened for non-priority files.
		/// Returns null if both queues are empty.
		/// </summary>
		public string? DequeueNextFile()
		{
			// Always process priority files first
			if (_priorityFiles.TryDequeue(out string? priorityFile))
			{
				// If the priority queue is now empty, open the gate for non-priority files
				if (_priorityFiles.IsEmpty)
					_priorityCompleted.Set();

				return priorityFile;
			}

			// No priority files remaining, dequeue from non-priority queue
			_nonPriorityFiles.TryDequeue(out string? nonPriorityFile);
			return nonPriorityFile;
		}

		/// <summary>
		/// Returns true if non-priority files are allowed to be transferred.
		/// This is the case when no priority files are pending.
		/// </summary>
		public bool CanTransferNonPriority() => _priorityFiles.IsEmpty;

		/// <summary>
		/// Blocks the calling thread until all priority files have been transferred.
		/// Uses ManualResetEventSlim for efficient waiting without CPU busy-looping.
		/// Call this before transferring a non-priority file to ensure priority compliance.
		/// </summary>
		public void WaitForPriorityCompletion()
		{
			_priorityCompleted.Wait();
		}
	}
}