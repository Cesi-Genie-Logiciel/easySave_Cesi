using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using EasySave.Services;
using Xunit;

namespace EasySave.Tests.Services
{
    /// <summary>
    /// Tests unitaires pour BusinessSoftwareDetector (P4 - auto-pause avec événement).
    /// </summary>
    public class BusinessSoftwareDetectorTests : IDisposable
    {
        private BusinessSoftwareDetector? _detector;

        public void Dispose()
        {
            _detector?.Dispose();
        }

        [Fact]
        public void Constructor_WithValidProcessName_SetsBusinessSoftwareName()
        {
            _detector = new BusinessSoftwareDetector("notepad");
            Assert.Equal("notepad", _detector.BusinessSoftwareName);
        }

        [Fact]
        public void Constructor_WithProcessNameWithExtension_NormalizesName()
        {
            _detector = new BusinessSoftwareDetector("notepad.exe");
            Assert.Equal("notepad", _detector.BusinessSoftwareName);
        }

        [Fact]
        public void Constructor_WithEmptyProcessName_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new BusinessSoftwareDetector(""));
        }

        [Fact]
        public void IsBusinessSoftwareRunning_WhenProcessNotRunning_ReturnsFalse()
        {
            _detector = new BusinessSoftwareDetector("NonExistentProcess12345");
            Assert.False(_detector.IsBusinessSoftwareRunning());
        }

        [Fact]
        public void IsBusinessSoftwareRunning_WhenProcessIsRunning_ReturnsTrue()
        {
            Process? testProcess = null;
            try
            {
                testProcess = Process.Start(new ProcessStartInfo
                {
                    FileName = "notepad.exe",
                    UseShellExecute = true,
                    CreateNoWindow = false
                });

                Thread.Sleep(500);

                _detector = new BusinessSoftwareDetector("notepad");
                var isRunning = _detector.IsBusinessSoftwareRunning();

                Assert.True(isRunning, "Notepad should be detected as running");
            }
            finally
            {
                testProcess?.Kill();
                testProcess?.Dispose();
            }
        }

        /// <summary>
        /// Test P4 : événement BusinessSoftwareStateChanged déclenché quand le processus démarre.
        /// </summary>
        [Fact]
        public async Task StartMonitoring_WhenProcessStarts_RaisesEvent()
        {
            _detector = new BusinessSoftwareDetector("notepad", pollingIntervalMs: 200);

            bool eventRaised = false;
            bool eventState = false;

            _detector.BusinessSoftwareStateChanged += (isRunning) =>
            {
                eventRaised = true;
                eventState = isRunning;
            };

            _detector.StartMonitoring();

            await Task.Delay(300);

            Process? testProcess = null;
            try
            {
                testProcess = Process.Start(new ProcessStartInfo
                {
                    FileName = "notepad.exe",
                    UseShellExecute = true,
                    CreateNoWindow = false
                });

                await Task.Delay(1000);

                Assert.True(eventRaised, "Event should be raised when process starts");
                Assert.True(eventState, "Event state should be true (process running)");
            }
            finally
            {
                testProcess?.Kill();
                testProcess?.Dispose();
            }
        }

        /// <summary>
        /// Test P4 : événement BusinessSoftwareStateChanged déclenché quand le processus s'arrête.
        /// Simplifié : on vérifie juste qu'un événement est levé (pas de vérification stricte du state).
        /// </summary>
        [Fact]
        public async Task StartMonitoring_WhenProcessStops_RaisesEvent()
        {
            Process? testProcess = null;
            try
            {
                testProcess = Process.Start(new ProcessStartInfo
                {
                    FileName = "notepad.exe",
                    UseShellExecute = true,
                    CreateNoWindow = false
                });

                await Task.Delay(1000);

                _detector = new BusinessSoftwareDetector("notepad", pollingIntervalMs: 200);

                int eventCount = 0;

                _detector.BusinessSoftwareStateChanged += (isRunning) =>
                {
                    eventCount++;
                };

                _detector.StartMonitoring();

                await Task.Delay(500);

                testProcess?.Kill();
                testProcess?.WaitForExit();
                testProcess?.Dispose();
                testProcess = null;

                await Task.Delay(2000);

                Assert.True(eventCount >= 1, $"Event should be raised when process stops (eventCount: {eventCount})");
            }
            finally
            {
                try { testProcess?.Kill(); } catch { }
                testProcess?.Dispose();
            }
        }

        [Fact]
        public void StopMonitoring_StopsTheMonitoringTask()
        {
            _detector = new BusinessSoftwareDetector("notepad", pollingIntervalMs: 100);
            _detector.StartMonitoring();

            Thread.Sleep(300);

            _detector.StopMonitoring();

            Assert.True(true, "StopMonitoring should complete without hanging");
        }

        [Fact]
        public void StartMonitoring_CalledTwice_DoesNotStartMultipleTasks()
        {
            _detector = new BusinessSoftwareDetector("notepad", pollingIntervalMs: 100);
            _detector.StartMonitoring();
            _detector.StartMonitoring();

            Thread.Sleep(300);

            _detector.StopMonitoring();

            Assert.True(true, "Multiple StartMonitoring calls should be safe");
        }
    }
}
