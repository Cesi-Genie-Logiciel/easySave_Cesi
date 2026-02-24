using System.Diagnostics;
using EasySave.Interfaces;

namespace EasySave.Services;

public sealed class CryptoSoftService : ICryptoService
{
    private readonly string _cryptoSoftExecutablePath;
    private readonly Mutex _mutex;
    private const int MutexTimeoutMs = 60_000;

    public CryptoSoftService(string cryptoSoftExecutablePath)
    {
        _cryptoSoftExecutablePath = cryptoSoftExecutablePath;
        _mutex = new Mutex(false, "Global\\EasySave_CryptoSoft_Mutex");
    }

    public bool IsAvailable()
    {
        return File.Exists(_cryptoSoftExecutablePath);
    }

    public int EncryptInPlace(string filePath, string encryptionKey)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return -10;

        if (!File.Exists(filePath))
            return -11;

        if (!IsAvailable())
            return -12;

        if (!AcquireMutex())
            return -15;

        try
        {
            var startInfo = BuildStartInfo(filePath, encryptionKey);

            using var process = new Process { StartInfo = startInfo };
            process.Start();

            if (!process.WaitForExit(30_000))
            {
                try { process.Kill(entireProcessTree: true); } catch { }
                return -14;
            }
            if (process.ExitCode < 0)
                return process.ExitCode;

            return 0;
        }
        catch
        {
            return -13;
        }
        finally
        {
            ReleaseMutex();
        }
    }

    public long EncryptInPlaceWithDurationMs(string filePath, string encryptionKey)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return -10;

        if (!File.Exists(filePath))
            return -11;

        if (!IsAvailable())
            return -12;

        if (!AcquireMutex())
            return -15;

        try
        {
            var startInfo = BuildStartInfo(filePath, encryptionKey);

            var sw = Stopwatch.StartNew();
            using var process = new Process { StartInfo = startInfo };
            process.Start();

            if (!process.WaitForExit(30_000))
            {
                try { process.Kill(entireProcessTree: true); } catch { }
                return -14;
            }

            sw.Stop();

            if (process.ExitCode < 0)
                return process.ExitCode;

            if (process.ExitCode > 0)
                return process.ExitCode;
            return Math.Max(0, sw.ElapsedMilliseconds);
        }
        catch
        {
            return -13;
        }
        finally
        {
            ReleaseMutex();
        }
    }

    private ProcessStartInfo BuildStartInfo(string filePath, string encryptionKey)
    {
        if (Path.GetExtension(_cryptoSoftExecutablePath).Equals(".dll", StringComparison.OrdinalIgnoreCase))
        {
            return new ProcessStartInfo
            {
                FileName = "dotnet",
                WorkingDirectory = Path.GetDirectoryName(_cryptoSoftExecutablePath)!,
                ArgumentList = { _cryptoSoftExecutablePath, filePath, encryptionKey },
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
        }

        return new ProcessStartInfo
        {
            FileName = _cryptoSoftExecutablePath,
            WorkingDirectory = Path.GetDirectoryName(_cryptoSoftExecutablePath)!,
            ArgumentList = { filePath, encryptionKey },
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
    }

    public static string? TryGetDefaultCryptoSoftPath()
    {
        var repoRoot = TryFindRepoRootDirectory(AppContext.BaseDirectory);
        if (string.IsNullOrWhiteSpace(repoRoot))
            return null;

        var directCandidates = new List<string>
        {
            // Root of net8.0
            Path.Combine(repoRoot, "CryptoSoft", "bin", "Release", "net8.0", "CryptoSoft"),
            Path.Combine(repoRoot, "CryptoSoft", "bin", "Debug", "net8.0", "CryptoSoft"),
            Path.Combine(repoRoot, "CryptoSoft", "bin", "Release", "net8.0", "CryptoSoft.exe"),
            Path.Combine(repoRoot, "CryptoSoft", "bin", "Debug", "net8.0", "CryptoSoft.exe"),
            Path.Combine(repoRoot, "CryptoSoft", "bin", "Release", "net8.0", "CryptoSoft.dll"),
            Path.Combine(repoRoot, "CryptoSoft", "bin", "Debug", "net8.0", "CryptoSoft.dll"),
        };

        var found = directCandidates.FirstOrDefault(File.Exists);
        if (!string.IsNullOrWhiteSpace(found))
            return found;

        foreach (var config in new[] { "Release", "Debug" })
        {
            var netDir = Path.Combine(repoRoot, "CryptoSoft", "bin", config, "net8.0");
            if (!Directory.Exists(netDir))
                continue;

            try
            {
                foreach (var dir in Directory.EnumerateDirectories(netDir))
                {
                    var dllPath = Path.Combine(dir, "CryptoSoft.dll");
                    if (File.Exists(dllPath))
                        return dllPath;

                    var exePath = Path.Combine(dir, "CryptoSoft");
                    if (File.Exists(exePath))
                        return exePath;

                    var winExePath = Path.Combine(dir, "CryptoSoft.exe");
                    if (File.Exists(winExePath))
                        return winExePath;
                }
            }
            catch
            {
            }
        }

        return null;
    }

    private static string? TryFindRepoRootDirectory(string startDirectory)
    {
        var markers = new[] { "EasyLog.slnx", "EasySave.slnx" };

        try
        {
            var dir = new DirectoryInfo(startDirectory);

            while (dir != null)
            {
                foreach (var markerFileName in markers)
                {
                    var markerPath = Path.Combine(dir.FullName, markerFileName);
                    if (File.Exists(markerPath))
                        return dir.FullName;
                }

                dir = dir.Parent;
            }
        }
        catch
        {
        }

        return null;
    }

    public static IReadOnlyList<string> GetDefaultCryptoSoftCandidatesForDebug()
    {
        var repoRoot = TryFindRepoRootDirectory(AppContext.BaseDirectory) ?? string.Empty;

        return new List<string>
        {
            Path.Combine(repoRoot, "CryptoSoft", "bin", "Release", "net8.0", "CryptoSoft"),
            Path.Combine(repoRoot, "CryptoSoft", "bin", "Debug", "net8.0", "CryptoSoft"),
            Path.Combine(repoRoot, "CryptoSoft", "bin", "Release", "net8.0", "CryptoSoft.exe"),
            Path.Combine(repoRoot, "CryptoSoft", "bin", "Debug", "net8.0", "CryptoSoft.exe"),
            Path.Combine(repoRoot, "CryptoSoft", "bin", "Release", "net8.0", "CryptoSoft.dll"),
            Path.Combine(repoRoot, "CryptoSoft", "bin", "Debug", "net8.0", "CryptoSoft.dll"),
            Path.Combine(repoRoot, "CryptoSoft", "bin", "Release", "net8.0", "linux-x64", "CryptoSoft.dll"),
            Path.Combine(repoRoot, "CryptoSoft", "bin", "Debug", "net8.0", "linux-x64", "CryptoSoft.dll"),
        };
    }

    private bool AcquireMutex()
    {
        try
        {
            return _mutex.WaitOne(MutexTimeoutMs);
        }
        catch
        {
            return false;
        }
    }

    private void ReleaseMutex()
    {
        try
        {
            _mutex.ReleaseMutex();
        }
        catch
        {
        }
    }
}
