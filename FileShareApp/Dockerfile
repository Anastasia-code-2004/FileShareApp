# Этап 1: Сборка
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY FileShareApp.Backend.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish FileShareApp.Backend.csproj -c Release -o /out

# Этап 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /out .

ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

ENTRYPOINT ["dotnet", "FileShareApp.Backend.dll"]