# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copiar os arquivos do projeto
COPY *.sln .
COPY src/API/API.csproj src/API/
COPY src/Application/Application.csproj src/Application/
COPY src/Domain/Domain.csproj src/Domain/
COPY src/Infrastructure/Infrastructure.csproj src/Infrastructure/
COPY Tests/API.Tests/API.Tests.csproj tests/API.Tests/

# Restaurar os pacotes NuGet
RUN dotnet restore

# Copiar o restante do código e compilar
COPY . .
WORKDIR /app/src/API
RUN dotnet publish -c Release -o /app/out

# Etapa 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

# Instalar o curl
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/out .

# Expor a porta da aplicação
EXPOSE 5000
ENTRYPOINT ["dotnet", "API.dll"]