# =============================================================================
# Stage 1: Build & Publish
# =============================================================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar archivos de solución y proyectos para restaurar dependencias
COPY ["Asisya.sln", "./"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["Api/Api.csproj", "Api/"]
COPY ["tests/Backend.UnitTests/Backend.UnitTests.csproj", "tests/Backend.UnitTests/"]
COPY ["tests/Backend.IntegrationTests/Backend.IntegrationTests.csproj", "tests/Backend.IntegrationTests/"]

RUN dotnet restore "Asisya.sln"

# Copiar todo el código fuente y compilar
COPY . .
WORKDIR "/src/Api"
RUN dotnet publish "Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# =============================================================================
# Stage 2: Runtime Image
# =============================================================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:5000

EXPOSE 5000

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Asisya.Api.dll"]
