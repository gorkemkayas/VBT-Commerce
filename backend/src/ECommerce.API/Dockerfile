# Build context is backend/src (see docker-compose.yml), so this Dockerfile sees the whole
# solution tree — needed because ECommerce.API references every module's Domain/Application/
# Infrastructure/Contracts projects via relative ProjectReference paths.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY . .
RUN dotnet restore "ECommerce.API/ECommerce.API.csproj"
RUN dotnet publish "ECommerce.API/ECommerce.API.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ECommerce.API.dll"]
