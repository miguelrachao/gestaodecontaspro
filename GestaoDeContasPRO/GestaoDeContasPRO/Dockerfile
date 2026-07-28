# ==========================================
# Runtime
# ==========================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

USER root

ENV TZ=Europe/Lisbon

RUN apt-get update \
    && apt-get install -y --no-install-recommends tzdata \
    && ln -snf /usr/share/zoneinfo/$TZ /etc/localtime \
    && echo $TZ > /etc/timezone \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app

ENV ASPNETCORE_HTTP_PORTS=80

EXPOSE 80

# Executar como utilizador não privilegiado
USER $APP_UID

# ==========================================
# Build
# ==========================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

ARG BUILD_CONFIGURATION=Release

WORKDIR /src

# Copia apenas o ficheiro do projeto para aproveitar a cache
COPY ["GestaoDeContasPRO.csproj", "./"]

# Restaura dependências
RUN dotnet restore "GestaoDeContasPRO.csproj"

# Copia o restante código
COPY . .

# Publica a aplicação (já inclui a compilação)
RUN dotnet publish "GestaoDeContasPRO.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false

# ==========================================
# Final
# ==========================================
FROM base AS final

WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "GestaoDeContasPRO.dll"]