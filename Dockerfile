# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy solution and projects files
COPY *.sln ./

COPY Order_status.API/Order_status.API.csproj Order_status.API/
COPY Order_status.Domain/Order_status.Domain.csproj Order_status.Domain/
COPY Order_status.Infrastructure/Order_status.Infrastructure.csproj Order_status.Infrastructure/
COPY Order_status.UnitTests/Domain.UnitTests.csproj Order_status.UnitTests/
COPY API.Tests/API.Tests.csproj API.Tests/
COPY Infrastructure.Tests/Infrastructure.Tests.csproj Infrastructure.Tests/

# Restore dependencies
RUN dotnet restore 

# Copy the remaining files and build the application
COPY . ./
RUN dotnet publish Order_status.API/Order_status.API.csproj -c Release -o /app/out

# Use the official .NET runtime image to run the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/out .

EXPOSE 80

# Set the entry point for the container
ENTRYPOINT ["dotnet", "Order_status.API.dll"]