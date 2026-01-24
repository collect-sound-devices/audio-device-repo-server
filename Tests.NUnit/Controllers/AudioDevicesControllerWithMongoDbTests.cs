using DeviceControllerLib.Controllers;
using DeviceControllerLib.Models.RestApi;
using DeviceControllerLib.Services;
using DeviceControllerLib.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using Tests.NUnit.TestData;

namespace Tests.NUnit.Controllers;

[TestFixture]
[Category("MongoDB")]
public class AudioDevicesControllerWithMongoDbTests
{
    private sealed class FakeCryptService : ICryptService
    {
        public string TryDecryptOrReturnOriginal(string originalText, string passphrase) => originalText;

        public string ComputeChecksum(string str) => "7d1a6195";
    }

    private static MongoDbAudioDeviceStorage CreateStorage()
    {
        var settings = new MongoDbSettings
        {
            ConnectionStringAnonymous = "mongodb+srv://cluster0.mdlxo.mongodb.net/?retryWrites=true&w=majority&connectTimeoutMS=30000&socketTimeoutMS=30000&serverSelectionTimeoutMS=30000",
            DatabaseName = "audio_device_repository",
            DatabaseUser = "ed",
            DatabasePassword = "Qwer.1234"
        };

        var options = Options.Create(settings);
        return new MongoDbAudioDeviceStorage(options, new FakeCryptService(), NullLogger<MongoDbAudioDeviceStorage>.Instance);
    }

    [Test]
    public void Add_Check_Remove()
    {
        var storage = CreateStorage();
        var cryptService = new FakeCryptService();
        var controller = new AudioDevicesController(storage, cryptService);

        var allLengthBeforeAdd = storage.GetAll().ToArray().Length;

        var device = DeviceMessages.CreateValidDevice(hostName: "MyPc");
        var result = controller.Add(device);

        Assert.That(result, Is.InstanceOf<CreatedAtActionResult>());

        var created = (CreatedAtActionResult)result;
        Assert.That(created.ActionName, Is.EqualTo(nameof(AudioDevicesController.GetByKey)));

        var all = storage.GetAll().ToArray();
        Assert.That(all.Length, Is.EqualTo(allLengthBeforeAdd + 1));
        Assert.That(all[allLengthBeforeAdd].PnpId, Is.EqualTo(device.PnpId));
        Assert.That(all[allLengthBeforeAdd].HostName, Is.EqualTo(cryptService.ComputeChecksum("MyPc")));

        result = controller.Remove(device.PnpId, device.HostName);

        Assert.That(result, Is.InstanceOf<NoContentResult>());
        Assert.That(storage.GetAll().ToArray().Length, Is.EqualTo(allLengthBeforeAdd));

    }
}
