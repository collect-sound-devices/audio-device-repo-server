# audio-device-repo-server (Audio Device Repository Server, AudioDeviceRepoServer)

Audio Device Repository Server is a ASP.NET-Core-with REST-API-backend for storing and serving sound device data.

## Architecture

<div style="zoom: 0.5;">

```mermaid
flowchart BT

classDef dottedBox fill:transparent,fill-opacity:0.55, stroke-dasharray:20 5,stroke-width:2px;
classDef stressedBox fill:#f0f0f0,fill-opacity:0.2,stroke-width:4px;
classDef invisibleNode fill:transparent,stroke:transparent;

coreAudioApi["Core Audio<br>(Windows API) or<br>Pulse Lib<br>(Linux PulseAudio)"]

subgraph scannerService["win-sound-scanner-go or linux-sound-scanner"]
    invisible1["<br><br><br><br><br>"]
    class invisible1 invisibleNode
    winSoundScannerService["WinSoundScanner<br>(Windows Service) or<br>LinuxSoundScanner<br>(Docker Container)"]
    invisible2["<br><br><br><br><br>"]
    class invisible2 invisibleNode
end
class scannerService dottedBox

subgraph requestQueueMicroservice["<br>"]
    requestQueue[("Request Queue<br>(RabbitMQ channel)")]
    rabbitMqRestForwarder["RmqToRestApiForwarder<br>(.NET microservice)"]
end
class requestQueueMicroservice dottedBox

subgraph repoServer["<br>"]
  invisible1["<br><br><br>"]
  class invisible1 invisibleNode
  deviceRepositoryApi["Device Repository Server<br>(REST API)"]
  invisible2["<br><br><br>"]
  class invisible2 invisibleNode
end
class repoServer stressedBox

winSoundScannerService --> |Access device| coreAudioApi
coreAudioApi -->|Device events| winSoundScannerService

winSoundScannerService -->|Publish request messages| requestQueue

requestQueue -->|Fetch request messages| rabbitMqRestForwarder
rabbitMqRestForwarder --> |Detect request messages| requestQueue
rabbitMqRestForwarder -->|POST/PUT requests| deviceRepositoryApi

```
</div>

## Functions

- Exposes a REST API for audio device messages and queries.
- Persists and reads audio device data in MongoDB.
- Receives forwarded request messages from the RabbitMQ-to-REST forwarder.
- Serves the backend used by the React client.
- API details: [rest-api-documentation.md](DeviceRepoAspNetCore/rest-api-documentation.md)

## Technologies Used

- C# with .NET 8 (ASP.NET Core).
- MongoDB via `MongoDB.Driver`.
- Razor Pages and REST controllers.
- NUnit and Moq for automated tests.
- Postman collection for API checks.
- LibMan for client-side web assets.

## Build and Debug

### Prerequisites

- .NET SDK 8.0
- LibMan CLI installed

### Build

```powershell
git submodule update --init --recursive
dotnet tool install -g Microsoft.Web.LibraryManager.Cli
dotnet restore
dotnet build
```

### Run and Debug

#### Database and Environment Variables:

The MongoDB setting should to be overridden via environment variables, as the default `appsettings.json`
are specific to the developer defaults:
- `ConnectionStringAnonymous` (override it via `MongoDbSettings__ConnectionStringAnonymous`) is used to connect to MongoDB instance (not necessarily without credentials)
- `DatabaseName` (override it via `MongoDbSettings__DatabaseName`) is the name of the database to use in MongoDB.
- `MongoDbSettings__DatabaseUser` and `MongoDbSettings__DatabasePassword` environment variables
must be set to override MongoDB credentials from `appsettings.json`.
- `MaxConnectRetries` (override it via `MongoDbSettings__MaxConnectRetries`) defines how often MongoDB startup connection retries are attempted. The default in `appsettings.json` is `5`.

#### How to run:

```powershell
cd DeviceRepoAspNetCore
dotnet run --launch-profile http
```

- HTTP profile uses `http://localhost:5027`.
- Set breakpoints and start `DeviceRepoAspNetCore` in your IDE (Rider or Visual Studio).


### Tests

```powershell
dotnet test
```

Optional smoke host:

```powershell
dotnet run --project .\DeviceController.SmokeHost\DeviceController.SmokeHost.csproj --launch-profile http
```

## Used design patterns (excluding framework-provided ones)

- Repository: `Services\IAudioDeviceStorage` and `Services\MongoDbAudioDeviceStorage` abstract and
  encapsulate MongoDB persistence behind an interface.

- DTO (data transfer object): `Models\RestApi\EntireDeviceMessage` and `Models\RestApi\VolumeChangeMessage`
  defineAPI payloads separate from persistence models.

- Adapter/Mapper: `Models\MongoDb\AudioDeviceDocument.ToDeviceMessage()`
  converts MongoDB documents to REST DTOs.

- Specification (via custom validation attribute): `Models\RestApi\AllowedDeviceMessageTypesAttribute`
  constrains allowed `DeviceMessageType` values on models.

## Changelog

- 2026-02-12 Updated `LICENSE` and `README` metadata.
- 2026-01-28 Updated launch settings to localhost URLs.
- 2026-01-23 Added controller tests against a real MongoDB backend.
- 2026-01-22 Moved MongoDB settings and storage to `DeviceControllerLib`.
- 2026-01-15 Extracted shared controller/models/interfaces to `DeviceControllerLib` and added `DeviceController.SmokeHost`.

## License

This project is licensed under the terms of the [MIT License](LICENSE).

## Contact

Eduard Danziger

Email: [edanziger@gmx.de](mailto:edanziger@gmx.de)
