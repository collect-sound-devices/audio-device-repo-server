using DeviceRepoAspNetCore.Services;

namespace DeviveController.SmokeHost;

public sealed class FakeCryptService : ICryptService
{
    public string TryDecryptOrReturnOriginal(string originalText, string passphrase) => originalText;

    public string ComputeChecksum(string str) => $"checksum:{str}";
}
