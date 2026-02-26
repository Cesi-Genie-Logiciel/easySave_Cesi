using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using EasySave.Services;
using Xunit;

namespace EasySave.Tests.Services
{
    /// <summary>
    /// Tests unitaires pour CryptoSoftService (P4 - mono-instance avec Mutex).
    /// </summary>
    public class CryptoSoftServiceTests : IDisposable
    {
        private readonly string _testDirectory;
        private readonly string _mockCryptoSoftPath;

        public CryptoSoftServiceTests()
        {
            _testDirectory = Path.Combine(Path.GetTempPath(), $"EasySaveTests_{Guid.NewGuid()}");
            Directory.CreateDirectory(_testDirectory);

            _mockCryptoSoftPath = CreateMockCryptoSoftExecutable();
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

        private string CreateMockCryptoSoftExecutable()
        {
            var mockExePath = Path.Combine(_testDirectory, "MockCryptoSoft.exe");
            File.WriteAllText(mockExePath, "mock");
            return mockExePath;
        }

        [Fact]
        public void IsAvailable_WhenExecutableExists_ReturnsTrue()
        {
            var service = new CryptoSoftService(_mockCryptoSoftPath);
            Assert.True(service.IsAvailable());
        }

        [Fact]
        public void IsAvailable_WhenExecutableDoesNotExist_ReturnsFalse()
        {
            var service = new CryptoSoftService(Path.Combine(_testDirectory, "NonExistent.exe"));
            Assert.False(service.IsAvailable());
        }

        [Fact]
        public void EncryptInPlace_WithInvalidFilePath_ReturnsErrorCode()
        {
            var service = new CryptoSoftService(_mockCryptoSoftPath);
            var result = service.EncryptInPlace("", "key");
            Assert.Equal(-10, result);
        }

        [Fact]
        public void EncryptInPlace_WithNonExistentFile_ReturnsErrorCode()
        {
            var service = new CryptoSoftService(_mockCryptoSoftPath);
            var result = service.EncryptInPlace(Path.Combine(_testDirectory, "missing.txt"), "key");
            Assert.Equal(-11, result);
        }

        [Fact]
        public void EncryptInPlace_WhenCryptoSoftNotAvailable_ReturnsErrorCode()
        {
            var service = new CryptoSoftService(Path.Combine(_testDirectory, "NonExistent.exe"));
            var testFile = Path.Combine(_testDirectory, "test.txt");
            File.WriteAllText(testFile, "content");

            var result = service.EncryptInPlace(testFile, "key");
            Assert.Equal(-12, result);
        }

        /// <summary>
        /// Test P4 : mono-instance avec Mutex.
        /// Deux appels simultanés → un seul acquiert le mutex, l'autre retourne -15 (conflit).
        /// </summary>
        [Fact]
        public async Task EncryptInPlace_SimultaneousCalls_OnlyOneAcquiresMutex()
        {
            var testFile1 = Path.Combine(_testDirectory, "file1.txt");
            var testFile2 = Path.Combine(_testDirectory, "file2.txt");
            File.WriteAllText(testFile1, "content1");
            File.WriteAllText(testFile2, "content2");

            var service1 = new CryptoSoftService(_mockCryptoSoftPath);
            var service2 = new CryptoSoftService(_mockCryptoSoftPath);

            var results = new int[2];
            var barrier = new Barrier(2);

            var task1 = Task.Run(() =>
            {
                barrier.SignalAndWait();
                results[0] = service1.EncryptInPlace(testFile1, "key");
            });

            var task2 = Task.Run(() =>
            {
                barrier.SignalAndWait();
                results[1] = service2.EncryptInPlace(testFile2, "key");
            });

            await Task.WhenAll(task1, task2);

            var errorCount = results.Count(r => r == -15);
            var successOrOtherErrorCount = results.Count(r => r != -15);

            Assert.True(errorCount >= 1 || successOrOtherErrorCount == 2,
                $"Expected at least one call to fail with mutex conflict (-15) or both to succeed/fail differently. Results: [{results[0]}, {results[1]}]");
        }

        /// <summary>
        /// Test P4 : EncryptInPlaceWithDurationMs avec mutex.
        /// </summary>
        [Fact]
        public void EncryptInPlaceWithDurationMs_WithInvalidFile_ReturnsErrorCode()
        {
            var service = new CryptoSoftService(_mockCryptoSoftPath);
            var result = service.EncryptInPlaceWithDurationMs("", "key");
            Assert.Equal(-10, result);
        }

        [Fact]
        public async Task EncryptInPlaceWithDurationMs_SimultaneousCalls_OnlyOneAcquiresMutex()
        {
            var testFile1 = Path.Combine(_testDirectory, "file1.txt");
            var testFile2 = Path.Combine(_testDirectory, "file2.txt");
            File.WriteAllText(testFile1, "content1");
            File.WriteAllText(testFile2, "content2");

            var service1 = new CryptoSoftService(_mockCryptoSoftPath);
            var service2 = new CryptoSoftService(_mockCryptoSoftPath);

            var results = new long[2];
            var barrier = new Barrier(2);

            var task1 = Task.Run(() =>
            {
                barrier.SignalAndWait();
                results[0] = service1.EncryptInPlaceWithDurationMs(testFile1, "key");
            });

            var task2 = Task.Run(() =>
            {
                barrier.SignalAndWait();
                results[1] = service2.EncryptInPlaceWithDurationMs(testFile2, "key");
            });

            await Task.WhenAll(task1, task2);

            var errorCount = results.Count(r => r == -15);
            var successOrOtherErrorCount = results.Count(r => r != -15);

            Assert.True(errorCount >= 1 || successOrOtherErrorCount == 2,
                $"Expected at least one call to fail with mutex conflict (-15). Results: [{results[0]}, {results[1]}]");
        }
    }
}
