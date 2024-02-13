FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Services/UserManagement/UserManagement.Core/UserManagement.Core.csproj", "Services/UserManagement/UserManagement.Core/"]
COPY ["Services/UserManagement/UserManagement.Application/UserManagement.Application.csproj", "Services/UserManagement/UserManagement.Application/"]
COPY ["Services/UserManagement/UserManagement.Persistence/UserManagement.Persistence.csproj", "Services/UserManagement/UserManagement.Persistence/"]
COPY ["Services/UserManagement/UserManagement.WebApi/UserManagement.WebApi.csproj", "Services/UserManagement/UserManagement.WebApi/"]
RUN dotnet restore "Services/UserManagement/UserManagement.WebApi/UserManagement.WebApi.csproj"
COPY . .
WORKDIR "/src/Services/UserManagement/UserManagement.WebApi"
RUN dotnet build "UserManagement.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "UserManagement.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UserManagement.WebApi.dll"]
