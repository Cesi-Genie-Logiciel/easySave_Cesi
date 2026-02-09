using System;
using System.IO;

namespace EasySave.Core.Strategies
{
	public class CompleteBackupStrategy : IBackupStrategy
	{
		public void ExecuteBackup(string sourcePath, string targetPath)
		{
			Console.WriteLine("  Strategy: Complete Backup (copy all files)");

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

				File.Copy(file, destFile, overwrite: true);
				Console.WriteLine($"    Copied: {relativePath}");
			}
		}
	}
}