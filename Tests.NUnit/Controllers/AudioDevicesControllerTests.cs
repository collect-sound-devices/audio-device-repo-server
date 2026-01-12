using DeviceRepoAspNetCore.Controllers;
using DeviceRepoAspNetCore.Models.RestApi;
using DeviceRepoAspNetCore.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace Tests.NUnit.Controllers;

[TestFixture]
public class AudioDevicesControllerTests
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
        var device = CreateValidDevice(hostName: "MyPc");
        var expectedHashedHost = CryptService.ComputeChecksum(device.HostName);
        var expectedPnpId = device.PnpId;

        var storage = new Mock<IAudioDeviceStorage>(MockBehavior.Strict);
        storage
            .Setup(s => s.Add(It.Is<EntireDeviceMessage>(d =>
                d.PnpId == expectedPnpId && d.HostName == expectedHashedHost)));

        var controller = new AudioDevicesController(storage.Object);

        var result = controller.Add(device);

        Assert.That(result, Is.InstanceOf<CreatedAtActionResult>());
        var created = (CreatedAtActionResult)result;
        Assert.That(created.ActionName, Is.EqualTo(nameof(AudioDevicesController.GetByKey)));
        Assert.That(created.Value, Is.TypeOf<EntireDeviceMessage>());

        var returned = (EntireDeviceMessage)created.Value!;
        Assert.That(returned.HostName, Is.EqualTo(expectedHashedHost));
        Assert.That(returned.PnpId, Is.EqualTo(expectedPnpId));
    }

    [Test]
    public void Remove_WhenDeviceExists_RemovesAndReturnsNoContent()
    {
        const string pnpId = "pnp-1";
        const string realHost = "MyPc";
        var hashedHost = CryptService.ComputeChecksum(realHost);
        var device = CreateValidDevice(pnpId: pnpId, hostName: hashedHost);

        var storage = new Mock<IAudioDeviceStorage>(MockBehavior.Strict);
        storage.Setup(s => s.GetAll()).Returns([device]);
        storage.Setup(s => s.Remove(pnpId, hashedHost)).Verifiable();

        var controller = new AudioDevicesController(storage.Object);

        var result = controller.Remove(pnpId, realHost);

        Assert.That(result, Is.InstanceOf<NoContentResult>());
        storage.Verify(s => s.Remove(pnpId, hashedHost), Times.Once);
        storage.Verify(s => s.GetAll(), Times.Exactly(2));
        storage.VerifyNoOtherCalls();
    }

    [Test]
    public void Remove_WhenDeviceDoesNotExist_ReturnsNotFound()
    {
        var storage = new Mock<IAudioDeviceStorage>(MockBehavior.Strict);
        storage.Setup(s => s.GetAll()).Returns([]);

        var controller = new AudioDevicesController(storage.Object);

        var result = controller.Remove("pnp", "host");

        Assert.That(result, Is.InstanceOf<NotFoundResult>());
        storage.Verify(s => s.GetAll(), Times.Exactly(2));
        storage.VerifyNoOtherCalls();
    }
}
