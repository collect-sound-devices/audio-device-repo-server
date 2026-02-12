# Audio Device Repository Server (ASP.NET Core with REST API)

- The backend of Audio Device Repository Server as a ASP.Net Core Server with REST API (C# / MongoDB).

- The GUI client (Next.js / React / TypeScript) deployed on Vercel: [https://list-audio-react-app.vercel.app](https://list-audio-react-app.vercel.app)

- For Audio Device REST API description see [rest-api-documentation.md](DeviceRepoAspNetCore/rest-api-documentation.md).

- Client Apps start the Audio Device Repository Server on-demand, if it doesn't run already.

- The main GUI client's repository is [list-audio-react-app](https://github.com/collect-sound-devices/list-audio-react-app/)

## Development environment

- The ASP.NET Core server can be started via Terminal using the following command:

```powershell or bash
cd DeviceRepoAspNetCore
dotnet run --launch-profile http
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

## License

This project is licensed under the terms of the [MIT License](LICENSE).

## Contact

Eduard Danziger

Email: [edanziger@gmx.de](mailto:edanziger@gmx.de)
