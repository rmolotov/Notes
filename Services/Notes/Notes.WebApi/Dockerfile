FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Services/Notes/Notes.Core/Notes.Core.csproj", "Services/Notes/Notes.Core/"]
COPY ["Services/Notes/Notes.Application/Notes.Application.csproj", "Services/Notes/Notes.Application/"]
COPY ["Services/Notes/Notes.Persistence/Notes.Persistence.csproj", "Services/Notes/Notes.Persistence/"]
COPY ["Services/Notes/Notes.WebApi/Notes.WebApi.csproj", "Services/Notes/Notes.WebApi/"]
RUN dotnet restore "Services/Notes/Notes.WebApi/Notes.WebApi.csproj"
COPY . .
WORKDIR "/src/Services/Notes/Notes.WebApi"
RUN dotnet build "Notes.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Notes.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_ENVIRONMENT=Docker
ENTRYPOINT ["dotnet", "Notes.WebApi.dll"]
