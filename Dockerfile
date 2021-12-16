FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal AS base
WORKDIR /app
EXPOSE 7008

ENV ASPNETCORE_URLS=http://+:7008

# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build
WORKDIR /src
COPY ["scabackend.csproj", "./"]
RUN dotnet restore "scabackend.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "scabackend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "scabackend.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "scabackend.dll"]
