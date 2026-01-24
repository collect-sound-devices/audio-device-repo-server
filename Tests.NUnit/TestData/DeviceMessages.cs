using DeviceControllerLib.Models.RestApi;

namespace Tests.NUnit.TestData;

internal static class DeviceMessages
{
    public static EntireDeviceMessage CreateValidDevice(
        string pnpId = "pnp-1",
        string hostName = "HOST",
        string name = "Device")
        => new()
        {
            PnpId = pnpId,
            HostName = hostName,
            Name = name,
            OperationSystemName = "Windows",
            FlowType = DeviceFlowType.Render,
            RenderVolume = 100,
            CaptureVolume = 200,
            UpdateDate = DateTime.UtcNow,
            DeviceMessageType = DeviceMessageType.Confirmed
        };
}
