# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY . ./
RUN dotnet restore
RUN dotnet build -c Release --no-incremental --no-restore
RUN dotnet publish -c Release --no-build --no-restore -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "TransportInfo.Gateway.dll"]