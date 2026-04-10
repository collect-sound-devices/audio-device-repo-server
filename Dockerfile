FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src

RUN apk add --no-cache git \
    && dotnet tool install -g Microsoft.Web.LibraryManager.Cli

ENV PATH="${PATH}:/root/.dotnet/tools"

COPY . .

RUN dotnet publish ./DeviceRepoAspNetCore/DeviceRepoAspNetCore.csproj \
    -c Release \
    -p:LatestCommitTimestampScript='git log -1 --format=%cI' \
    -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "DeviceRepoAspNetCore.dll"]


