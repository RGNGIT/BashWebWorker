FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["BashWebWorker.csproj", "BashWebWorker/"]
COPY ["Libs/Newtonsoft.Json.dll", "BashWebWorker/Libs/"]
RUN dotnet restore "./BashWebWorker/BashWebWorker.csproj"

COPY . .
WORKDIR "/src/BashWebWorker"

RUN dotnet build "./BashWebWorker.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BashWebWorker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=publish /src/BashWebWorker/Libs/Newtonsoft.Json.dll ./Libs/
ENTRYPOINT ["dotnet", "BashWebWorker.dll"]