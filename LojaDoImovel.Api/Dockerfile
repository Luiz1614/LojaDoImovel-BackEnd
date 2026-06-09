# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["LojaDoImovel.Api/LojaDoImovel.Api.csproj", "LojaDoImovel.Api/"]
COPY ["LojaDoImovel.Application/LojaDoImovel.Application.csproj", "LojaDoImovel.Application/"]
COPY ["LojaDoImovel.Contracts/LojaDoImovel.Contracts.csproj", "LojaDoImovel.Contracts/"]
COPY ["LojaDoImovel.Infrastructure/LojaDoImovel.Infrastructure.csproj", "LojaDoImovel.Infrastructure/"]
COPY ["LojaDoImovel.Domain/LojaDoImovel.Domain.csproj", "LojaDoImovel.Domain/"]
COPY ["LojaDoImovel.IoC/LojaDoImovel.IoC.csproj", "LojaDoImovel.IoC/"]
COPY ["LojaDoImovel.Transform/LojaDoImovel.Transform.csproj", "LojaDoImovel.Transform/"]
RUN dotnet restore "./LojaDoImovel.Api/LojaDoImovel.Api.csproj"
COPY . .
WORKDIR "/src/LojaDoImovel.Api"
RUN dotnet build "./LojaDoImovel.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./LojaDoImovel.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
USER root
WORKDIR /app
COPY --from=publish /app/publish .
RUN mkdir -p /app/wwwroot/uploads && chown -R $APP_UID /app/wwwroot
USER $APP_UID
ENTRYPOINT ["dotnet", "LojaDoImovel.Api.dll"]