using System.Threading;

namespace EasySave.Coordination
{
	/// <summary>
	/// Prevents simultaneous transfer of large files across parallel backup jobs.
	/// Only one large file (above the configured threshold) can be transferred at a time.
	/// Small files are not affected and can always transfer freely.
	/// This avoids saturating network bandwidth during parallel backups.
	/// </summary>
	public class LargeFileTransferGuard
	{
		// SemaphoreSlim with 1 slot: only one large file transfer allowed at a time
		private readonly SemaphoreSlim _semaphore = new(1, 1);

		// Size threshold in kilobytes - files above this are considered "large"
		private readonly long _thresholdKo;

		/// <summary>
		/// The configured size threshold in kilobytes.
		/// Files strictly above this value are considered large files.
		/// </summary>
		public long ThresholdKo => _thresholdKo;

		/// <summary>
		/// Initializes the guard with the given size threshold.
		/// </summary>
		/// <param name="thresholdKo">Size threshold in kilobytes (configurable by the user in AppSettings)</param>
		public LargeFileTransferGuard(long thresholdKo)
		{
			_thresholdKo = thresholdKo;
		}

		/// <summary>
		/// Returns true if the given file size exceeds the configured threshold.
		/// </summary>
		/// <param name="fileSizeBytes">File size in bytes (retrieved from the file system before transfer)</param>
		public bool IsLargeFile(long fileSizeBytes)
		{
			return fileSizeBytes / 1024 > _thresholdKo;
		}

		/// <summary>
		/// Acquires the transfer slot if the file is considered large.
		/// If another large file is currently being transferred, this call blocks asynchronously
		/// until that transfer completes and releases the slot.
		/// Small files pass through immediately without acquiring the slot.
		/// </summary>
		/// <param name="fileSizeBytes">File size in bytes</param>
		public async Task AcquireIfLargeFile(long fileSizeBytes)
		{
			if (IsLargeFile(fileSizeBytes))
				await _semaphore.WaitAsync();
		}

		/// <summary>
		/// Releases the transfer slot after a large file transfer is complete.
		/// Must always be called after AcquireIfLargeFile to avoid deadlocks.
		/// Has no effect if the file was not considered large.
		/// </summary>
		/// <param name="fileSizeBytes">File size in bytes (same value used during acquire)</param>
		public void Release(long fileSizeBytes)
		{
			if (IsLargeFile(fileSizeBytes))
				_semaphore.Release();
		}
	}
}














