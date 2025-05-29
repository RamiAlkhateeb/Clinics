# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY WebApi.csproj ./
RUN dotnet restore 

# Copy everything else
COPY . ./

# Build the solution (Release mode)
RUN dotnet build WebApi.csproj -c Release -o /app/build

# Publish the Web API project
RUN dotnet publish WebApi.csproj -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Expose ports (5000 HTTP, 5001 HTTPS)
EXPOSE 4000
EXPOSE 4001

# Set environment variable for ASPNETCORE_URLS
ENV ASPNETCORE_URLS=http://+:4000

ENTRYPOINT ["dotnet", "WebApi.dll"]