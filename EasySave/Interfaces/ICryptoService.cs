namespace EasySave.Interfaces;

/// <summary>
/// Abstraction around CryptoSoft (external encryption tool).
/// Kept minimal on purpose for v2 P4 branch 1.
/// </summary>
public interface ICryptoService
{
    /// <summary>
    /// Returns true if CryptoSoft is available on this machine.
    /// </summary>
    bool IsAvailable();

    /// <summary>
    /// Encrypts the given file (in place by default).
    /// - Returns 0 when encryption succeeded.
    /// - Returns a negative value when an error occurred.
    /// 
    /// Note: encryption duration is integrated in branch feat/log-add-encryption-time.
    /// </summary>
    int EncryptInPlace(string filePath, string encryptionKey);
}
