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
    // Only copies files that have been modified since the last backup.
    // Uses the file's last write timestamp to determine if it needs updating.
    // Same pause/stop mechanism as CompleteBackupStrategy.
    public class DifferentialBackupStrategy : IBackupStrategy
    {
        private readonly ICryptoService _crypto;
        private readonly string _encryptionKey;
        private readonly List<string> _extensionsToEncrypt;

        private Action<BackupEventArgs>? _onFileTransferred;
        private string _jobName = string.Empty;

        public DifferentialBackupStrategy(ICryptoService crypto = null, string encryptionKey = null,
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
            Console.WriteLine("  Mode: Differential backup");

            if (!Directory.Exists(targetPath))
                Directory.CreateDirectory(targetPath);

            string[] files = Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories);
            int total = files.Length;
            int done = 0;

            List<string> allExtensions = _extensionsToEncrypt
                .Concat(context.ExtensionsToEncrypt)
                .Distinct()
                .ToList();

            foreach (string file in files)
            {
                // Same pause/stop checkpoints as CompleteBackupStrategy
                context.PauseEvent.Wait(context.Token);
                context.Token.ThrowIfCancellationRequested();

                string relative = Path.GetRelativePath(sourcePath, file);
                string dest = Path.Combine(targetPath, relative);

                string destDir = Path.GetDirectoryName(dest);
                if (!string.IsNullOrEmpty(destDir) && !Directory.Exists(destDir))
                    Directory.CreateDirectory(destDir);

                // Only copy if the file is new or has been modified since last backup
                if (!File.Exists(dest) || File.GetLastWriteTime(file) > File.GetLastWriteTime(dest))
                {
                    Stopwatch sw = Stopwatch.StartNew();
                    File.Copy(file, dest, overwrite: true);

                    long encryptMs = TryEncrypt(dest, allExtensions);
                    sw.Stop();

                    done++;
                    Console.WriteLine($"    Copy (modified): {relative}");

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
                else
                {
                    Console.WriteLine($"    Skip (unchanged): {relative}");
                }
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