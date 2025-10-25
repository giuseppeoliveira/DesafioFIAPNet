# Stage 1: Build frontend (Vite)
FROM node:20-alpine AS vite-builder
WORKDIR /frontend
COPY frontend/package*.json ./
# Install dependencies (use ci when lockfile exists, otherwise install)
RUN if [ -f package-lock.json ]; then npm ci --no-audit --no-fund; else npm install --no-audit --no-fund; fi
COPY frontend/ .
RUN npm run build

# Stage 2: Build .NET app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# copy csproj and restore as distinct layers
COPY ["SchoolAPI/SchoolAPI.csproj", "SchoolAPI/"]
RUN dotnet restore "SchoolAPI/SchoolAPI.csproj"

# copy everything and build
COPY . .
WORKDIR /src/SchoolAPI

# Copy Vite output into the API's www folder before publish
COPY --from=vite-builder /frontend/dist/ /src/SchoolAPI/www/

RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
ENTRYPOINT ["dotnet", "SchoolAPI.dll"]