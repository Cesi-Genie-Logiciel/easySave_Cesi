using System;
using System.Diagnostics;
using System.IO;
using EasySave.Interfaces;
using EasySave.Models;

namespace EasySave.Strategies
{
    public class CompleteBackupStrategy : IBackupStrategy
    {
        private readonly ICryptoService? _cryptoService;
        private readonly string _encryptionKey;

        private Action<BackupEventArgs>? _onFileTransferred;
        private string _backupName = string.Empty;

        public CompleteBackupStrategy(ICryptoService? cryptoService = null, string? encryptionKey = null)
        {
            _cryptoService = cryptoService;
            _encryptionKey = !string.IsNullOrWhiteSpace(encryptionKey)
                ? encryptionKey
                : (Environment.GetEnvironmentVariable("EASY_SAVE_ENCRYPTION_KEY") ?? "EasySave");
        }

        public void SetNotificationCallback(Action<BackupEventArgs> callback, string backupName)
        {
            _onFileTransferred = callback;
            _backupName = backupName;
        }

        public void ExecuteBackup(string sourcePath, string targetPath)
        {
            Console.WriteLine("  Strategy: Complete Backup (copy all files)");

            if (!Directory.Exists(targetPath))
            {
                Directory.CreateDirectory(targetPath);
            }

            var allFiles = Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories);
            int totalFiles = allFiles.Length;
            int processedFiles = 0;

            foreach (var file in allFiles)
            {
                string relativePath = Path.GetRelativePath(sourcePath, file);
                string destFile = Path.Combine(targetPath, relativePath);

                string? destDir = Path.GetDirectoryName(destFile);
                if (!string.IsNullOrEmpty(destDir) && !Directory.Exists(destDir))
                {
                    Directory.CreateDirectory(destDir);
                }

                var stopwatch = Stopwatch.StartNew();
                File.Copy(file, destFile, overwrite: true);

                // CryptoSoft integration (branch P4): encrypt copied file if service is available
                if (_cryptoService?.IsAvailable() == true)
                {
                    var encCode = _cryptoService.EncryptInPlace(destFile, _encryptionKey);
                    if (encCode < 0)
                    {
                        Console.WriteLine($"[CryptoSoft] Encryption failed for '{destFile}' (code {encCode}).");
                    }
                }

                stopwatch.Stop();

                processedFiles++;
                Console.WriteLine($"    Copied: {relativePath}");

                // Notifier le transfert de fichier
                if (_onFileTransferred != null)
                {
                    var fileInfo = new FileInfo(file);
                    var eventArgs = new BackupEventArgs
                    {
                        BackupName = _backupName,
                        SourceFile = file,
                        DestFile = destFile,
                        FileSize = fileInfo.Length,
                        TransferTimeMs = stopwatch.Elapsed.TotalMilliseconds,
                        TotalFiles = totalFiles,
                        ProcessedFiles = processedFiles,
                        Progress = (int)((processedFiles * 100.0) / totalFiles)
                    };
                    _onFileTransferred(eventArgs);
                }
            }
        }
    }
}
