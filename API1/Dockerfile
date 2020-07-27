#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["API1/WeatherServiceApi.csproj", "API1/"]
COPY ["Api.RabbitMq/Api.RabbitMq.csproj", "Api.RabbitMq/"]
COPY ["WeatherServiceApi.Message.Domain/WeatherServiceApi.Message.Domain.csproj", "API1/"]
RUN dotnet restore "API1/WeatherServiceApi.csproj"
COPY . .
WORKDIR "/src/API1"
RUN dotnet build "WeatherServiceApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WeatherServiceApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WeatherServiceApi.dll"]