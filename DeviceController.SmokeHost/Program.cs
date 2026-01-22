using DeviceControllerLib.Controllers;
using DeviceControllerLib.Services;
using DeviveController.SmokeHost;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ICryptService, FakeCryptService>();
builder.Services.AddSingleton<IAudioDeviceStorage, InMemoryAudioDeviceStorage>();

builder.Services
    .AddControllers()
    .AddApplicationPart(typeof(AudioDevicesController).Assembly);

var app = builder.Build();

app.MapControllers();

app.Run();
