# build the project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and restore dependencies
COPY . .
RUN dotnet restore

# Build and publish the project
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Run the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Expose port
EXPOSE 5001

# Environment for ASP.NET Core
ENV ASPNETCORE_URLS=http://+:5001

ENTRYPOINT ["dotnet", "Identity.Api.dll"]
