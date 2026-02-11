using EasySave.Interfaces;
using EasySave.Models;

namespace EasySave.Strategies
{
    /// <summary>
    /// Differential backup: copies only new or modified files.
    /// </summary>
    public class DifferentialBackupStrategy : IBackupStrategy
    {
        public void Execute(BackupConfig config, BackupStats stats, Action<BackupEventArgs> notifyProgress)
        {
            if (!Directory.Exists(config.TargetPath))
            {
                Directory.CreateDirectory(config.TargetPath);
            }

            var files = Directory.GetFiles(config.SourcePath, "*.*", SearchOption.AllDirectories);
            int totalFiles = files.Length;
            stats.TotalFiles = totalFiles;

            int processed = 0;

            foreach (var file in files)
            {
                string relativePath = Path.GetRelativePath(config.SourcePath, file);
                string destFile = Path.Combine(config.TargetPath, relativePath);

                string? destDir = Path.GetDirectoryName(destFile);
                if (!string.IsNullOrEmpty(destDir) && !Directory.Exists(destDir))
                {
                    Directory.CreateDirectory(destDir);
                }

                bool shouldCopy = !File.Exists(destFile) ||
                                  File.GetLastWriteTime(file) > File.GetLastWriteTime(destFile);

                long fileSize = 0;
                double transferMs = 0;

                if (shouldCopy)
                {
                    var info = new FileInfo(file);
                    fileSize = info.Length;

                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    File.Copy(file, destFile, overwrite: true);
                    sw.Stop();
                    transferMs = sw.Elapsed.TotalMilliseconds;
                }

                processed++;

                var args = new BackupEventArgs
                {
                    BackupName = config.Name,
                    SourceFile = file,
                    DestFile = destFile,
                    FileSize = fileSize,
                    TransferTimeMs = transferMs,
                    TotalFiles = totalFiles,
                    ProcessedFiles = processed,
                    Progress = (int)(processed * 100.0 / totalFiles)
                };

                notifyProgress(args);
            }
        }
    }
}