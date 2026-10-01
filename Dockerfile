# --- Build stage ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /app

# --- Runtime stage ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app .

# Render expects your service to listen on port 10000 by default
ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

# Change SmartGearWeb.dll to match your actual project name if different
ENTRYPOINT ["dotnet", "SmartGearWeb.dll"]
