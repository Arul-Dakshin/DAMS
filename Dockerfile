# ---- Build stage ----
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restore (copy csproj first for layer caching)
COPY DAMS.Core/DAMS.Core.csproj DAMS.Core/
COPY DAMS.API/DAMS.API.csproj DAMS.API/
RUN dotnet restore DAMS.API/DAMS.API.csproj

# Build & publish
COPY DAMS.Core/ DAMS.Core/
COPY DAMS.API/ DAMS.API/
RUN dotnet publish DAMS.API/DAMS.API.csproj -c Release -o /app /p:UseAppHost=false

# ---- Runtime stage ----
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .

ENV ASPNETCORE_ENVIRONMENT=Production
# Render injects PORT; the app reads it in Program.cs. 8080 is the local-container default.
EXPOSE 8080

ENTRYPOINT ["dotnet", "DAMS.API.dll"]
