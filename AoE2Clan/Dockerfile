#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim-arm32v7 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["AoE2Clan/AoE2Clan.csproj", "AoE2Clan/"]
RUN dotnet restore "AoE2Clan/AoE2Clan.csproj"
COPY . .
WORKDIR "/src/AoE2Clan"
RUN dotnet build "AoE2Clan.csproj" -c Release -o /app/build

FROM build AS publish
 RUN dotnet publish "AoE2Clan.csproj" -c Release -r debian-arm -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AoE2Clan.dll"]