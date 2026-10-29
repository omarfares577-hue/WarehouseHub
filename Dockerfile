FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "WarehouseHub.Api/WarehouseHub.Api.csproj"
RUN dotnet publish "WarehouseHub.Api/WarehouseHub.Api.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "WarehouseHub.Api.dll"]