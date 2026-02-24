using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using EasySave.Interfaces;
using EasySave.Models;
using EasySave.Services;

namespace EasySave.Strategies
{
    // Copies every file from source to target, regardless of whether it changed.
    // Between each file copy, checks if the job has been paused or stopped
    // through the execution context.
    public class CompleteBackupStrategy : IBackupStrategy
    {
        private readonly ICryptoService _crypto;
        private readonly string _encryptionKey;
        private readonly List<string> _extensionsToEncrypt;

        private Action<BackupEventArgs>? _onFileTransferred;
        private string _jobName = string.Empty;

        public CompleteBackupStrategy(ICryptoService crypto = null, string encryptionKey = null,
                                      object logger = null, List<string> extensionsToEncrypt = null)
        {
            _crypto = crypto;
            _encryptionKey = encryptionKey
                ?? Environment.GetEnvironmentVariable("EASY_SAVE_ENCRYPTION_KEY")
                ?? "EasySave";
            _extensionsToEncrypt = extensionsToEncrypt ?? new List<string>();
        }

        public void SetNotificationCallback(Action<BackupEventArgs> callback, string jobName)
        {
            _onFileTransferred = callback;
            _jobName = jobName;
        }

        public async Task ExecuteBackup(string sourcePath, string targetPath, BackupExecutionContext context)
        {
            Console.WriteLine("  Mode: Complete backup");

            if (!Directory.Exists(targetPath))
                Directory.CreateDirectory(targetPath);

            string[] files = Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories);
            int total = files.Length;
            int done = 0;

            // Merge extensions from constructor and from context
            List<string> allExtensions = _extensionsToEncrypt
                .Concat(context.ExtensionsToEncrypt)
                .Distinct()
                .ToList();

            foreach (string file in files)
            {
                // PAUSE CHECK: if Pause() was called, the event is in non-signaled state.
                // Wait() blocks here until Resume() calls Set().
                // We pass the token so Stop() during pause throws and exits cleanly.
                context.PauseEvent.Wait(context.Token);

                // STOP CHECK: if Stop() was called, this throws OperationCanceledException
                // which BackupJob catches and sets state to Stopped.
                context.Token.ThrowIfCancellationRequested();

                string relative = Path.GetRelativePath(sourcePath, file);
                string dest = Path.Combine(targetPath, relative);

                string destDir = Path.GetDirectoryName(dest);
                if (!string.IsNullOrEmpty(destDir) && !Directory.Exists(destDir))
                    Directory.CreateDirectory(destDir);

                Stopwatch sw = Stopwatch.StartNew();
                File.Copy(file, dest, overwrite: true);

                long encryptMs = TryEncrypt(dest, allExtensions);
                sw.Stop();

                done++;
                Console.WriteLine($"    Copy: {relative}");

                _onFileTransferred?.Invoke(new BackupEventArgs
                {
                    BackupName = _jobName,
                    SourceFile = file,
                    DestFile = dest,
                    FileSize = new FileInfo(file).Length,
                    TransferTimeMs = sw.Elapsed.TotalMilliseconds,
                    EncryptionTimeMs = encryptMs,
                    TotalFiles = total,
                    ProcessedFiles = done,
                    Progress = (int)(done * 100.0 / total)
                });
            }
        }

        private long TryEncrypt(string filePath, List<string> extensions)
        {
            string ext = Path.GetExtension(filePath).ToLower();
            bool shouldEncrypt = extensions.Any(e => e.ToLower() == ext);

            if (_crypto == null || !_crypto.IsAvailable() || !shouldEncrypt)
                return 0;

            return _crypto.EncryptInPlaceWithDurationMs(filePath, _encryptionKey);
        }
    }
}