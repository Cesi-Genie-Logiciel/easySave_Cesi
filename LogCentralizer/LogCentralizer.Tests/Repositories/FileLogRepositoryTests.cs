using System;
using System.IO;
using LogCentralizer.Models;
using LogCentralizer.Repositories;
using Xunit;

namespace LogCentralizer.Tests.Repositories
{
    /// <summary>
    /// Tests unitaires pour FileLogRepository (P4 - service Docker).
    /// </summary>
    public class FileLogRepositoryTests : IDisposable
    {
        private readonly string _testDirectory;

        public FileLogRepositoryTests()
        {
            _testDirectory = Path.Combine(Path.GetTempPath(), $"LogCentralizerTests_{Guid.NewGuid()}");
            Directory.CreateDirectory(_testDirectory);
        }

        public void Dispose()
        {
            try
            {
                if (Directory.Exists(_testDirectory))
                    Directory.Delete(_testDirectory, recursive: true);
            }
            catch
            {
            }
        }

        [Fact]
        public void Constructor_CreatesBaseDirectory()
        {
            var subDir = Path.Combine(_testDirectory, "logs");
            var repo = new FileLogRepository(subDir);

            Assert.True(Directory.Exists(subDir));
        }

        [Fact]
        public void AppendLog_CreatesFileWithClientId()
        {
            var repo = new FileLogRepository(_testDirectory);
            var entry = new LogEntry
            {
                Timestamp = DateTime.Now,
                BackupName = "TestJob",
                SourcePath = "C:\\source\\file.txt",
                DestPath = "C:\\target\\file.txt",
                FileSize = 1024,
                TransferTimeMs = 100
            };

            repo.AppendLog("client1", entry);

            var logFileName = $"centralized_log_{DateTime.Now:yyyy-MM-dd}.json";
            var logFilePath = Path.Combine(_testDirectory, logFileName);

            Assert.True(File.Exists(logFilePath));
            var content = File.ReadAllText(logFilePath);
            Assert.Contains("client1", content);
            Assert.Contains("TestJob", content);
        }

        [Fact]
        public void AppendLog_AppendsMultipleEntriesFromDifferentClients()
        {
            var repo = new FileLogRepository(_testDirectory);
            var entry1 = new LogEntry { BackupName = "Job1", FileSize = 100, TransferTimeMs = 10 };
            var entry2 = new LogEntry { BackupName = "Job2", FileSize = 200, TransferTimeMs = 20 };

            repo.AppendLog("client1", entry1);
            repo.AppendLog("client2", entry2);

            var logFileName = $"centralized_log_{DateTime.Now:yyyy-MM-dd}.json";
            var logFilePath = Path.Combine(_testDirectory, logFileName);

            var content = File.ReadAllText(logFilePath);
            Assert.Contains("client1", content);
            Assert.Contains("client2", content);
            Assert.Contains("Job1", content);
            Assert.Contains("Job2", content);
        }

        [Fact]
        public void GetLogs_WithExistingDate_ReturnsContent()
        {
            var repo = new FileLogRepository(_testDirectory);
            var entry = new LogEntry { BackupName = "TestJob", FileSize = 1024, TransferTimeMs = 100 };
            repo.AppendLog("client1", entry);

            var today = DateTime.Now.ToString("yyyy-MM-dd");
            var logs = repo.GetLogs(today);

            Assert.NotEmpty(logs);
            Assert.Contains("client1", logs[0]);
        }

        [Fact]
        public void GetLogs_WithNonExistentDate_ReturnsEmptyList()
        {
            var repo = new FileLogRepository(_testDirectory);
            var logs = repo.GetLogs("2000-01-01");

            Assert.Empty(logs);
        }
    }
}
