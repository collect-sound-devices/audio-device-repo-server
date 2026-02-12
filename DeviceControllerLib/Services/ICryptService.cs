namespace DeviceControllerLib.Services;

public interface ICryptService
{
    string TryDecryptOrReturnOriginal(string originalText, string passphrase);
    string ComputeChecksum(string str);
}
