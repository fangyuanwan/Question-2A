#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine AS base
WORKDIR /app
EXPOSE 80
ENV RABBITMQ_HOST localhost
ENV RABBITMQ_PORT 5672 
ENV ASPNETCORE_URLS=http://+:5000

# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine AS build
WORKDIR /src
COPY ["Question2A.csproj", "./"]
RUN dotnet restore "Question2A.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "Question2A.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Question2A.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Question2A.dll"]
