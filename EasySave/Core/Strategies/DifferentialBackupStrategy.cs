using System;
using System.IO;

namespace EasySave.Core.Strategies
{
	public class DifferentialBackupStrategy : IBackupStrategy
	{
		public void ExecuteBackup(string sourcePath, string targetPath)
		{
			Console.WriteLine("  Strategy: Differential Backup (copy only modified files)");

			if (!Directory.Exists(targetPath))
			{
				Directory.CreateDirectory(targetPath);
			}

			foreach (var file in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
			{
				string relativePath = Path.GetRelativePath(sourcePath, file);
				string destFile = Path.Combine(targetPath, relativePath);

				string destDir = Path.GetDirectoryName(destFile);
				if (!Directory.Exists(destDir))
				{
					Directory.CreateDirectory(destDir);
				}

				if (!File.Exists(destFile) ||
					File.GetLastWriteTime(file) > File.GetLastWriteTime(destFile))
				{
					File.Copy(file, destFile, overwrite: true);
					Console.WriteLine($"    Copied (modified): {relativePath}");
				}
				else
				{
					Console.WriteLine($"    Skipped (unchanged): {relativePath}");
				}
			}
		}
	}
}