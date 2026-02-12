using DeviceControllerLib.Models.RestApi;

namespace Tests.NUnit.TestData;

internal static class DeviceMessages
{
    public static EntireDeviceMessage GenerateValidDeviceMessage(
        string hostName = "NUNIT-HOST",
        string? devicePnpId = null,
        string deviceName = "Device")
        => new()
        {
            PnpId = devicePnpId ?? Guid.NewGuid().ToString("D").ToLower(),
            HostName = hostName,
            Name = deviceName,
            OperationSystemName = "Windows",
            FlowType = DeviceFlowType.Render,
            RenderVolume = 100,
            CaptureVolume = 200,
            UpdateDate = DateTime.UtcNow,
            DeviceMessageType = DeviceMessageType.Confirmed
        };
}
