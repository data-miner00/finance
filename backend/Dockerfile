# Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

COPY ["Directory.Packages.props", "./"]
COPY ["Core/*.csproj", "Core/"]
COPY ["WebApi/*.csproj", "WebApi/"]
RUN dotnet restore WebApi

COPY . .

RUN dotnet build "WebApi" -c Release -o /app/build --no-restore

FROM build AS publish
RUN dotnet publish "WebApi" -c Release -o /app/publish /p:UseAppHost=false

# Final stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "WebApi.dll"]

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production