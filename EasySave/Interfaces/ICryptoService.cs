namespace EasySave.Interfaces;

public interface ICryptoService
{
    bool IsAvailable();
    int EncryptInPlace(string filePath, string encryptionKey);
    long EncryptInPlaceWithDurationMs(string filePath, string encryptionKey);
}
