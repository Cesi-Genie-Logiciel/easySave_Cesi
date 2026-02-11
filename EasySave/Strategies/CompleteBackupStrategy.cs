using EasySave.Interfaces;
using EasySave.Models;

namespace EasySave.Strategies
{
    /// <summary>
    /// Complete backup: copies all files from source to target.
    /// </summary>
    public class CompleteBackupStrategy : IBackupStrategy
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

            for (int i = 0; i < totalFiles; i++)
            {
                string file = files[i];
                string relativePath = Path.GetRelativePath(config.SourcePath, file);
                string destFile = Path.Combine(config.TargetPath, relativePath);

                string? destDir = Path.GetDirectoryName(destFile);
                if (!string.IsNullOrEmpty(destDir) && !Directory.Exists(destDir))
                {
                    Directory.CreateDirectory(destDir);
                }

                var fileInfo = new FileInfo(file);
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();

                File.Copy(file, destFile, overwrite: true);

                stopwatch.Stop();

                var args = new BackupEventArgs
                {
                    BackupName = config.Name,
                    SourceFile = file,
                    DestFile = destFile,
                    FileSize = fileInfo.Length,
                    TransferTimeMs = stopwatch.Elapsed.TotalMilliseconds,
                    TotalFiles = totalFiles,
                    ProcessedFiles = i + 1,
                    Progress = (int)((i + 1) * 100.0 / totalFiles)
                };

                notifyProgress(args);
            }
        }
    }
}