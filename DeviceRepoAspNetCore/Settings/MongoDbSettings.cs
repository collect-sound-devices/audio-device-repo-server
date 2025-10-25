namespace DeviceRepoAspNetCore.Settings;

public class MongoDbSettings
{   // read out of configuration (appsettings.json)
    public required string ConnectionStringAnonymous { get; init; }
    public required string DatabaseName { get; init; }
    public required string DatabaseUser { get; init; }
    public required string DatabasePassword { get; init; }
}