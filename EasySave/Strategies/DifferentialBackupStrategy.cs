using EasySave.Interfaces;
using EasySave.Models;
using System.Diagnostics;

namespace EasySave.Strategies
{
    /// <summary>
    /// Differential backup: copies only files that are new or modified
    /// </summary>
    public class DifferentialBackupStrategy : IBackupStrategy
    {
        public void Execute(BackupConfig config, BackupStats stats, Action<BackupEventArgs> notifyProgress)
        {
            if (!Directory.Exists(config.SourcePath))
                throw new DirectoryNotFoundException($"Source directory not found: {config.SourcePath}");

            Directory.CreateDirectory(config.TargetPath);

            var files = Directory.GetFiles(config.SourcePath, "*.*", SearchOption.AllDirectories);

            // Filter: only new or modified files
            var filesToCopy = new List<string>();

            foreach (var sourceFile in files)
            {
                string relativePath = Path.GetRelativePath(config.SourcePath, sourceFile);
                string targetFile = Path.Combine(config.TargetPath, relativePath);

                // Copy if target doesn't exist OR source is newer
                if (!File.Exists(targetFile) ||
                    File.GetLastWriteTime(sourceFile) > File.GetLastWriteTime(targetFile))
                {
                    filesToCopy.Add(sourceFile);
                }
            }

            stats.TotalFiles = filesToCopy.Count;
            stats.FilesRemaining = filesToCopy.Count;
            stats.TotalSize = filesToCopy.Sum(f => new FileInfo(f).Length);
            stats.SizeRemaining = stats.TotalSize;

            int processedFiles = 0;

            foreach (var sourceFile in filesToCopy)
            {
                var startTime = Stopwatch.StartNew();

                try
                {
                    string relativePath = Path.GetRelativePath(config.SourcePath, sourceFile);
                    string targetFile = Path.Combine(config.TargetPath, relativePath);

                    Directory.CreateDirectory(Path.GetDirectoryName(targetFile)!);
                    File.Copy(sourceFile, targetFile, overwrite: true);

                    startTime.Stop();
                    var fileInfo = new FileInfo(sourceFile);

                    processedFiles++;
                    stats.FilesRemaining = stats.TotalFiles - processedFiles;
                    stats.SizeRemaining -= fileInfo.Length;
                    stats.CurrentSourceFile = sourceFile;
                    stats.CurrentDestFile = targetFile;

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
