FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["LogCentralizer/LogCentralizer.csproj", "LogCentralizer/"]
RUN dotnet restore "LogCentralizer/LogCentralizer.csproj"
COPY . .
WORKDIR "/src/LogCentralizer"
RUN dotnet build "LogCentralizer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LogCentralizer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:5000
ENV LogDirectory=/app/logs
VOLUME ["/app/logs"]
ENTRYPOINT ["dotnet", "LogCentralizer.dll"]
