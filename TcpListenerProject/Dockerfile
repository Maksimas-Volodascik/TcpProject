# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/runtime:9.0@sha256:de0826f037d10d1ca4dab10c27d460b42a94702da9d4ffbf60fba26d079c6532 AS base
ARG APP_UID=1000 
USER $APP_UID
WORKDIR /app


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0@sha256:24fe7c00719afc72e2b67cace264dea2ee39ae45f5b553bd59493fe3ddfc920f AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TcpListenerProject/TcpListenerProject.csproj", "TcpListenerProject/"]
RUN dotnet restore "./TcpListenerProject/TcpListenerProject.csproj"
COPY . .
WORKDIR "/src/TcpListenerProject"
RUN dotnet build "./TcpListenerProject.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TcpListenerProject.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TcpListenerProject.dll"]