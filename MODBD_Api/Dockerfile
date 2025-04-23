# Use the ASP.NET runtime image for the base stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Use the .NET SDK image for the build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy the solution file and restore dependencies
COPY ["MODBD_Api.sln", "./"]
COPY ["MODBD_Common/MODBD_Common.csproj", "MODBD_Common/"]
COPY ["MODBD_Core/MODBD_Core.csproj", "MODBD_Core/"]
COPY ["MODBD_Api/MODBD_Api.csproj", "MODBD_Api/"]
COPY ["MODBD_Analiza/MODBD_Analiza.csproj", "MODBD_Analiza/"]
RUN dotnet restore "./MODBD_Api.sln"

# Copy all project files and build the solution
COPY . .
WORKDIR "/src/MODBD_Api"
RUN dotnet build "./MODBD_Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish the application
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MODBD_Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Use the runtime image for the final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MODBD_Api.dll"]