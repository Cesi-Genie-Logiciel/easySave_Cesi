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
        private readonly ICryptoService? _crypto;
        private readonly string _encryptionKey;
        private readonly BusinessSoftwareMonitor _monitor;
        private readonly ILogger? _logger;
        private readonly List<string> _extensionsToEncrypt;

        private Action<BackupEventArgs>? _onFileTransferred;
        private string _jobName = string.Empty;
        private bool _interruptionLogged;

        public DifferentialBackupStrategy(ICryptoService? cryptoService = null, string? encryptionKey = null,
                                          ILogger? logger = null, List<string>? extensionsToEncrypt = null)
        {
            _crypto = cryptoService;
            _encryptionKey = encryptionKey
                ?? Environment.GetEnvironmentVariable("EASY_SAVE_ENCRYPTION_KEY")
                ?? "EasySave";
            _monitor = new BusinessSoftwareMonitor();
            _logger = logger;
            _extensionsToEncrypt = extensionsToEncrypt ?? new List<string>();
        }

        public void SetNotificationCallback(Action<BackupEventArgs> callback, string jobName)
        {
            _onFileTransferred = callback;
            _jobName = jobName;
        }

        public void ExecuteBackup(string sourcePath, string targetPath)
        {
            Console.WriteLine("  Mode : Sauvegarde differentielle");

            if (!Directory.Exists(targetPath))
                Directory.CreateDirectory(targetPath);

            var files = Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories);
            int total = files.Length;
            int done = 0;

            foreach (var file in files)
            {
                if (_monitor.IsRunning())
                {
                    Console.WriteLine($"  Sauvegarde '{_jobName}' interrompue : logiciel metier detecte.");
                    if (!_interruptionLogged)
                    {
                        _interruptionLogged = true;
                        _logger?.LogJobEvent(_jobName, JobEventType.Interrupted,
                            "Logiciel metier detecte", _monitor.ProcessName);
                    }
                    break;
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
                        Progress = (int)(done * 100.0 / total)
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