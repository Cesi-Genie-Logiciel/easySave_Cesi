using System.Diagnostics;
using EasySave.Interfaces;

namespace EasySave.Services;

/// <summary>
/// Wrapper that calls CryptoSoft as an external process.
/// 
/// In this repo, CryptoSoft exists as a separate .NET project (OutputType Exe).
/// For now we locate it relative to the EasySave executable directory.
/// This keeps EasySave independent from any UI and avoids hard dependencies.
/// </summary>
public sealed class CryptoSoftService : ICryptoService
{
    private readonly string _cryptoSoftExecutablePath;

    public CryptoSoftService(string cryptoSoftExecutablePath)
    {
        _cryptoSoftExecutablePath = cryptoSoftExecutablePath;
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

        try
        {
            var startInfo = BuildStartInfo(filePath, encryptionKey);

            using var process = new Process { StartInfo = startInfo };
            process.Start();

            if (!process.WaitForExit(30_000))
            {
                try { process.Kill(entireProcessTree: true); } catch { /* ignore */ }
                return -14; // timeout
            }

            // Drain output for diagnosis (CryptoSoft currently writes args / errors to console)
            // Keeping it internal: returned via negative codes only; messages are printed by strategies on failure.
            var stdErr = process.StandardError.ReadToEnd();
            var stdOut = process.StandardOutput.ReadToEnd();

            if (process.ExitCode < 0)
                return process.ExitCode;

            // If CryptoSoft exits >=0 it's elapsed ms; normalize to success.
            return 0;
        }
        catch
        {
            return -13;
        }
    }

    private ProcessStartInfo BuildStartInfo(string filePath, string encryptionKey)
    {
        // If we got a DLL (common when using `dotnet build` without publish), run it through dotnet.
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

        // Otherwise assume native / apphost executable.
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

    /// <summary>
    /// Helper to build the default executable path when running from bin/Debug|Release.
    /// </summary>
    public static string? TryGetDefaultCryptoSoftPath()
    {
        var repoRoot = TryFindRepoRootDirectory(AppContext.BaseDirectory);
        if (string.IsNullOrWhiteSpace(repoRoot))
            return null;

        // Prefer exe/apphost if present, otherwise fall back to DLL.
        // Note: CryptoSoft project may output under net8.0/<rid>/ (e.g. linux-x64).
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

        // Fallback: search under net8.0/* (RID folders like linux-x64)
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
                // ignore and continue
            }
        }

        return null;
    }

    private static string? TryFindRepoRootDirectory(string startDirectory)
    {
        // We use a stable marker present at repo root.
        const string markerFileName = "EasyLog.slnx";

        try
        {
            var dir = new DirectoryInfo(startDirectory);

            while (dir != null)
            {
                var markerPath = Path.Combine(dir.FullName, markerFileName);
                if (File.Exists(markerPath))
                    return dir.FullName;

                dir = dir.Parent;
            }
        }
        catch
        {
            // ignore
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
}
