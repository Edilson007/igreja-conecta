FROM node:20-alpine AS frontend-build
WORKDIR /src/frontend
COPY frontend/package*.json ./
RUN npm install --no-audit --no-fund
COPY frontend/ ./
RUN npm run build

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS backend-build
WORKDIR /src
COPY IgrejaConecta.Domain/IgrejaConecta.Domain.csproj IgrejaConecta.Domain/
COPY IgrejaConecta.Application/IgrejaConecta.Application.csproj IgrejaConecta.Application/
COPY IgrejaConecta.Infrastructure/IgrejaConecta.Infrastructure.csproj IgrejaConecta.Infrastructure/
COPY IgrejaConecta.Api/IgrejaConecta.Api.csproj IgrejaConecta.Api/
RUN dotnet restore IgrejaConecta.Api/IgrejaConecta.Api.csproj
COPY . ./
RUN dotnet publish IgrejaConecta.Api/IgrejaConecta.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=backend-build /app/publish ./
COPY --from=frontend-build /src/frontend/dist/igreja-conecta-web ./wwwroot
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "IgrejaConecta.Api.dll"]
