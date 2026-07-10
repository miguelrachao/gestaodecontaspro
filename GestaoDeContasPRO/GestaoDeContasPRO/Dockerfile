# Fase base (runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

USER root

# Configuração do fuso horário
ENV TZ=Europe/Lisbon

RUN apt-get update \
    && apt-get install -y --no-install-recommends tzdata \
    && ln -snf /usr/share/zoneinfo/$TZ /etc/localtime \
    && echo $TZ > /etc/timezone \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app

ENV ASPNETCORE_HTTP_PORTS=80
EXPOSE 80

# Volta a utilizar o utilizador da aplicação
USER $APP_UID

# Fase de compilação
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

ARG BUILD_CONFIGURATION=Release

WORKDIR /src

COPY ["GestaoDeContasPRO.csproj", "./"]

RUN dotnet restore "GestaoDeContasPRO.csproj"

COPY . .

RUN dotnet build "GestaoDeContasPRO.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/build

# Fase de publicação
FROM build AS publish

ARG BUILD_CONFIGURATION=Release

RUN dotnet publish "GestaoDeContasPRO.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false

# Fase final
FROM base AS final

WORKDIR /app

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "GestaoDeContasPRO.dll"]