using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using EasySave.Interfaces;
using EasySave.Models;
using EasySave.Services;
using ProSoft.EasyLog.Interfaces;

namespace EasySave.Strategies
{
    public class DifferentialBackupStrategy : IBackupStrategy
    {
        private readonly ICryptoService? _crypto;
        private readonly string _encryptionKey;
        private readonly ILogger? _logger;
        private readonly List<string> _extensionsToEncrypt;

        private Action<BackupEventArgs>? _onFileTransferred;
        private string _jobName = string.Empty;

        // Reference to the parent job so we can read its cancellation token and pause gate
        private BackupJob? _parentJob;

        public DifferentialBackupStrategy(ICryptoService? cryptoService = null, string? encryptionKey = null,
                                          ILogger? logger = null, List<string>? extensionsToEncrypt = null)
        {
            _crypto = cryptoService;
            _encryptionKey = encryptionKey
                ?? Environment.GetEnvironmentVariable("EASY_SAVE_ENCRYPTION_KEY")
                ?? "EasySave";
            _logger = logger;
            _extensionsToEncrypt = extensionsToEncrypt ?? new List<string>();
        }

        public void SetNotificationCallback(Action<BackupEventArgs> callback, string jobName)
        {
            _onFileTransferred = callback;
            _jobName = jobName;
        }

        // Called by BackupJob.Execute() so the strategy knows which job it belongs to
        public void SetParentJob(BackupJob job)
        {
            _parentJob = job;
        }

        public void ExecuteBackup(string sourcePath, string targetPath)
        {
            Console.WriteLine("  Mode : Sauvegarde differentielle");

            if (!Directory.Exists(targetPath))
                Directory.CreateDirectory(targetPath);

            string[] files = Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories);
            int total = files.Length;
            int done = 0;

            foreach (string file in files)
            {
                // Before processing each file, check if the user asked to stop
                if (_parentJob != null)
                {
                    _parentJob.CancellationToken.ThrowIfCancellationRequested();

                    // If the job is paused, this call blocks until Resume() opens the gate
                    _parentJob.PauseEvent.Wait(_parentJob.CancellationToken);
                }

                string relative = Path.GetRelativePath(sourcePath, file);
                string dest = Path.Combine(targetPath, relative);

                string? destDir = Path.GetDirectoryName(dest);
                if (!string.IsNullOrEmpty(destDir) && !Directory.Exists(destDir))
                    Directory.CreateDirectory(destDir);

                if (!File.Exists(dest) || File.GetLastWriteTime(file) > File.GetLastWriteTime(dest))
                {
                    var sw = Stopwatch.StartNew();
                    File.Copy(file, dest, overwrite: true);

                    long encryptMs = TryEncrypt(dest);
                    sw.Stop();

                    done++;
                    Console.WriteLine($"    Copie (modifie) : {relative}");

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
                        Progress = total == 0 ? 100 : (int)(done * 100.0 / total)
                    });
                }
                else
                {
                    Console.WriteLine($"    Ignore (inchange) : {relative}");
                }
            }
        }

        private long TryEncrypt(string filePath)
        {
            var ext = Path.GetExtension(filePath).ToLower();
            bool shouldEncrypt = _extensionsToEncrypt.Any(e => e.ToLower() == ext);

            if (_crypto?.IsAvailable() != true || !shouldEncrypt)
                return 0;

            if (_crypto is CryptoSoftService svc)
                return svc.EncryptInPlaceWithDurationMs(filePath, _encryptionKey);

            int code = _crypto.EncryptInPlace(filePath, _encryptionKey);
            return code < 0 ? code : 0;
        }
    }
}