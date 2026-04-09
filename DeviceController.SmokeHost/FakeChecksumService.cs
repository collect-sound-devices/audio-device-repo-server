using DeviceControllerLib.Services;

namespace DeviceController.SmokeHost;

public sealed class FakeChecksumService : IChecksumService
{
    public string ComputeChecksum(string str) => $"checksum:{str}";
}
