using EasySave.Interfaces;
using EasySave.Models;
using System.Diagnostics;

namespace EasySave.Strategies
{
    /// <summary>
    /// Complete backup: copies all files from source to destination
    /// </summary>
    public class CompleteBackupStrategy : IBackupStrategy
    {
        private readonly ICryptoService? _cryptoService;

        // Temporary: pulled from environment to avoid hardcoding secrets in code.
        private readonly string _encryptionKey;

        public CompleteBackupStrategy(ICryptoService? cryptoService = null, string? encryptionKey = null)
        {
            _cryptoService = cryptoService;
            _encryptionKey = !string.IsNullOrWhiteSpace(encryptionKey)
                ? encryptionKey
                : (Environment.GetEnvironmentVariable("EASY_SAVE_ENCRYPTION_KEY") ?? "EasySave");
        }

        public void Execute(BackupConfig config, BackupStats stats, Action<BackupEventArgs> notifyProgress)
        {
            if (!Directory.Exists(config.SourcePath))
                throw new DirectoryNotFoundException($"Source directory not found: {config.SourcePath}");

            // Create destination if not exists
            Directory.CreateDirectory(config.TargetPath);

            // Get all files
            var files = Directory.GetFiles(config.SourcePath, "*.*", SearchOption.AllDirectories);

            stats.TotalFiles = files.Length;
            stats.FilesRemaining = files.Length;
            stats.TotalSize = files.Sum(f => new FileInfo(f).Length);
            stats.SizeRemaining = stats.TotalSize;

            int processedFiles = 0;

            foreach (var sourceFile in files)
            {
                var startTime = Stopwatch.StartNew();

                try
                {
                    // Calculate relative path
                    string relativePath = Path.GetRelativePath(config.SourcePath, sourceFile);
                    string targetFile = Path.Combine(config.TargetPath, relativePath);

                    // Create target directory
                    Directory.CreateDirectory(Path.GetDirectoryName(targetFile)!);

                    // Copy file
                    File.Copy(sourceFile, targetFile, overwrite: true);

                    // Optional encryption (branch 1: always attempt when service is available)
                    if (_cryptoService?.IsAvailable() == true)
                    {
                        var encCode = _cryptoService.EncryptInPlace(targetFile, _encryptionKey);
                        if (encCode < 0)
                        {
                            Console.WriteLine($"[CryptoSoft] Encryption failed for '{targetFile}' (code {encCode}).");
                        }
                    }

                    startTime.Stop();
                    var fileInfo = new FileInfo(sourceFile);

                    // Update stats
                    processedFiles++;
                    stats.FilesRemaining = stats.TotalFiles - processedFiles;
                    stats.SizeRemaining -= fileInfo.Length;
                    stats.CurrentSourceFile = sourceFile;
                    stats.CurrentDestFile = targetFile;

                    // Notify observers
                    notifyProgress(new BackupEventArgs
                    {
                        BackupName = config.Name,
                        SourceFile = sourceFile,
                        DestFile = targetFile,
                        FileSize = fileInfo.Length,
                        TransferTime = startTime.ElapsedMilliseconds,
                        TotalFiles = stats.TotalFiles,
                        ProcessedFiles = processedFiles,
                        Stats = stats
                    });
                }
                catch (Exception ex)
                {
                    // Notify error
                    notifyProgress(new BackupEventArgs
                    {
                        BackupName = config.Name,
                        SourceFile = sourceFile,
                        DestFile = "",
                        FileSize = 0,
                        TransferTime = -1,
                        TotalFiles = stats.TotalFiles,
                        ProcessedFiles = processedFiles,
                        Stats = stats
                    });

                    Console.WriteLine($"Error copying {sourceFile}: {ex.Message}");
                }
            }
        }
    }
}
