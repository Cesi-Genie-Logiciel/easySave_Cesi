using System;
using System.Diagnostics;
using System.IO;
using EasySave.Interfaces;
using EasySave.Models;
using EasySave.Services;
using ProSoft.EasyLog.Interfaces;
using ProSoft.EasyLog.Models;

namespace EasySave.Strategies
{
    public class DifferentialBackupStrategy : IBackupStrategy
    {
        private readonly ICryptoService? _cryptoService;
        private readonly string _encryptionKey;
        private readonly BusinessSoftwareMonitor _businessSoftwareMonitor;
        private readonly ILogger? _logger;

        private Action<BackupEventArgs>? _onFileTransferred;
        private string _backupName = string.Empty;
        private bool _interruptionLogged;

        public DifferentialBackupStrategy(ICryptoService? cryptoService = null, string? encryptionKey = null, ILogger? logger = null)
        {
            _cryptoService = cryptoService;
            _encryptionKey = !string.IsNullOrWhiteSpace(encryptionKey)
                ? encryptionKey
                : (Environment.GetEnvironmentVariable("EASY_SAVE_ENCRYPTION_KEY") ?? "EasySave");

            _businessSoftwareMonitor = new BusinessSoftwareMonitor();
            _logger = logger;
        }

        public void SetNotificationCallback(Action<BackupEventArgs> callback, string backupName)
        {
            _onFileTransferred = callback;
            _backupName = backupName;
        }

        public void ExecuteBackup(string sourcePath, string targetPath)
        {
            Console.WriteLine("  Strategy: Differential Backup (copy only modified files)");

            if (!Directory.Exists(targetPath))
            {
                Directory.CreateDirectory(targetPath);
            }

            var allFiles = Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories);
            int totalFiles = allFiles.Length;
            int processedFiles = 0;

            foreach (var file in allFiles)
            {
                // Business software detection: stop between files (finish current file, then stop).
                if (_businessSoftwareMonitor.IsRunning())
                {
                    Console.WriteLine($"  [BusinessSoftware] Backup '{_backupName}' interrupted: business software detected.");

                    if (!_interruptionLogged)
                    {
                        _interruptionLogged = true;
                        _logger?.LogJobEvent(
                            _backupName,
                            JobEventType.Interrupted,
                            "Business software detected during execution",
                            _businessSoftwareMonitor.ProcessName);
                    }

                    break;
                }

                string relativePath = Path.GetRelativePath(sourcePath, file);
                string destFile = Path.Combine(targetPath, relativePath);

                string? destDir = Path.GetDirectoryName(destFile);
                if (!string.IsNullOrEmpty(destDir) && !Directory.Exists(destDir))
                {
                    Directory.CreateDirectory(destDir);
                }

                if (!File.Exists(destFile) ||
                    File.GetLastWriteTime(file) > File.GetLastWriteTime(destFile))
                {
                    var stopwatch = Stopwatch.StartNew();
                    File.Copy(file, destFile, overwrite: true);

                    long encryptionTimeMs = 0;

                    // CryptoSoft integration (branch P4): encrypt copied file if service is available
                    if (_cryptoService?.IsAvailable() == true)
                    {
                        if (_cryptoService is EasySave.Services.CryptoSoftService crypto)
                        {
                            encryptionTimeMs = crypto.EncryptInPlaceWithDurationMs(destFile, _encryptionKey);
                        }
                        else
                        {
                            var encCode = _cryptoService.EncryptInPlace(destFile, _encryptionKey);
                            encryptionTimeMs = encCode < 0 ? encCode : 0;
                        }

                        if (encryptionTimeMs < 0)
                        {
                            Console.WriteLine($"[CryptoSoft] Encryption failed for '{destFile}' (code {encryptionTimeMs}).");
                        }
                    }

                    stopwatch.Stop();

                    processedFiles++;
                    Console.WriteLine($"    Copied (modified): {relativePath}");

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
                            EncryptionTimeMs = encryptionTimeMs,
                            TotalFiles = totalFiles,
                            ProcessedFiles = processedFiles,
                            Progress = (int)((processedFiles * 100.0) / totalFiles)
                        };
                        _onFileTransferred(eventArgs);
                    }
                }
                else
                {
                    Console.WriteLine($"    Skipped (unchanged): {relativePath}");
                }
            }
        }
    }
}
