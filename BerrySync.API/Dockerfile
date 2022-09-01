#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine AS base
WORKDIR /app
EXPOSE 80
ENV GOOGLE_APPLICATION_CREDENTIALS=/data/key.json

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["BerrySync.API/BerrySync.API.csproj", "BerrySync.API/"]
COPY ["BerrySync.Data/BerrySync.Data.csproj", "BerrySync.Data/"]
COPY ["BerrySync.Updater/BerrySync.Updater.csproj", "BerrySync.Updater/"]
RUN dotnet restore "BerrySync.API/BerrySync.API.csproj"
COPY . .
WORKDIR "/src/BerrySync.API"
RUN dotnet build "BerrySync.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BerrySync.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BerrySync.API.dll"]