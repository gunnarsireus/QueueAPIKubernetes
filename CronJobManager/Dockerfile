FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["CronJobManager/CronJobManager.csproj", "CronJobManager/"]
COPY ["CronJobManager.Services/CronJobManager.Services.csproj", "CronJobManager.Services/"]
RUN dotnet restore "./CronJobManager/CronJobManager.csproj"
COPY . .
WORKDIR "/src/CronJobManager"
RUN dotnet build "./CronJobManager.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CronJobManager.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CronJobManager.dll"]
