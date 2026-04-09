using DeviceControllerLib.Models.MongoDb;
using DeviceControllerLib.Settings;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace DeviceRepoAspNetCore.Services;

public class MongoDbInitializationService(
    IOptions<MongoDbSettings> mongoDbSettings,
    ILogger<MongoDbInitializationService> logger)
    : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        var userName = mongoDbSettings.Value.DatabaseUser;
        var password = mongoDbSettings.Value.DatabasePassword;

        var mongoUrl = new MongoUrlBuilder(mongoDbSettings.Value.ConnectionStringAnonymous)
        {
            Username = userName,
            Password = password
        }.ToMongoUrl();

        var client = new MongoClient(mongoUrl);
        var database = client.GetDatabase(mongoDbSettings.Value.DatabaseName);
        var devicesCollection = database.GetCollection<AudioDeviceDocument>("devices");

        var indexKeysDefinition = Builders<AudioDeviceDocument>.IndexKeys
            .Ascending(d => d.PnpId)
            .Ascending(d => d.HostName);
        var indexOptions = new CreateIndexOptions { Unique = true };
        var indexModel = new CreateIndexModel<AudioDeviceDocument>(indexKeysDefinition, indexOptions);

        return devicesCollection.Indexes.CreateOneAsync(indexModel, cancellationToken: cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("MongoDB initialization service stopped.");
        return Task.CompletedTask;
    }
}
