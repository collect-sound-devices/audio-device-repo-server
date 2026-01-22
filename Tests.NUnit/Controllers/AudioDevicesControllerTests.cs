using DeviceControllerLib.Controllers;
using DeviceControllerLib.Models.RestApi;
using DeviceControllerLib.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace Tests.NUnit.Controllers;

[TestFixture]
public class AudioDevicesControllerWithMockedDependenciesTests
{
    private static EntireDeviceMessage CreateValidDevice(string pnpId = "pnp-1", string hostName = "HOST", string name = "Device")
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


    [Test]
    public void Add_WhenValid_AddsDeviceWithHashedHostName_AndReturnsCreatedAtAction()
    {
        const string hostName = "MyPc";
        const string hashedHostName = "7d1a6195";
        var device = CreateValidDevice(hostName: hostName);
        var expectedPnpId = device.PnpId;

        var storage = new Mock<IAudioDeviceStorage>(MockBehavior.Strict);
        storage
            .Setup(s => s.Add(It.Is<EntireDeviceMessage>(d =>
                d.PnpId == expectedPnpId && d.HostName == hashedHostName)));

        var cryptService = new Mock<ICryptService>(MockBehavior.Strict);
        cryptService.Setup(c => c.ComputeChecksum(device.HostName)).Returns(hashedHostName);

        var controller = new AudioDevicesController(storage.Object, cryptService.Object);

        var result = controller.Add(device);

        Assert.That(result, Is.InstanceOf<CreatedAtActionResult>());
        var created = (CreatedAtActionResult)result;
        Assert.That(created.ActionName, Is.EqualTo(nameof(AudioDevicesController.GetByKey)));
        Assert.That(created.Value, Is.TypeOf<EntireDeviceMessage>());

        var returned = (EntireDeviceMessage)created.Value!;
        Assert.That(returned.HostName, Is.EqualTo(hashedHostName));
        Assert.That(returned.PnpId, Is.EqualTo(expectedPnpId));
    }

    [Test]
    public void Remove_WhenDeviceDoesNotExist_ReturnsNotFound()
    {
        var storage = new Mock<IAudioDeviceStorage>(MockBehavior.Strict);
        storage.Setup(s => s.GetAll()).Returns([]);

        var cryptService = new Mock<ICryptService>(MockBehavior.Strict);
        cryptService.Setup(c => c.ComputeChecksum(It.IsAny<string>())).Returns("checksum");

        var controller = new AudioDevicesController(storage.Object, cryptService.Object);

        var result = controller.Remove("pnp", "host");

        Assert.That(result, Is.InstanceOf<NotFoundResult>());
        storage.Verify(s => s.GetAll(), Times.Exactly(2));
        storage.VerifyNoOtherCalls();
    }
}
