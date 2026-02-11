using System;
using System.IO;
using System.Diagnostics;
using EasySave.Interfaces;
using EasySave.Models;

namespace EasySave.Strategies
{
    public class CompleteBackupStrategy : IBackupStrategy
    {
        public void ExecuteBackup(string sourcePath, string targetPath, Action<BackupEventArgs> onFileTransferred, string backupName)
        {
            Console.WriteLine("  Strategy: Complete Backup (copy all files)");
            
            if (!Directory.Exists(targetPath))
            {
                Directory.CreateDirectory(targetPath);
            }
            
            var files = Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories);
            int totalFiles = files.Length;
            int processedFiles = 0;
            
            foreach (var file in files)
            {
                string relativePath = Path.GetRelativePath(sourcePath, file);
                string destFile = Path.Combine(targetPath, relativePath);
                
                string? destDir = Path.GetDirectoryName(destFile);
                if (!string.IsNullOrEmpty(destDir) && !Directory.Exists(destDir))
                {
                    Directory.CreateDirectory(destDir);
                }
                
                var fileInfo = new FileInfo(file);
                var stopwatch = Stopwatch.StartNew();
                
                File.Copy(file, destFile, overwrite: true);
                
                stopwatch.Stop();
                processedFiles++;
                
                Console.WriteLine($"    Copied: {relativePath}");
                
                // Notify observers
                onFileTransferred(new BackupEventArgs
                {
                    BackupName = backupName,
                    SourceFile = file,
                    DestFile = destFile,
                    FileSize = fileInfo.Length,
                    TransferTimeMs = stopwatch.Elapsed.TotalMilliseconds,
                    TotalFiles = totalFiles,
                    ProcessedFiles = processedFiles,
                    Progress = (int)((processedFiles * 100.0) / totalFiles)
                });
            }
        }
    }
}
