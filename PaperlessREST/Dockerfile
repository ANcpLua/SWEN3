FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["PaperlessREST/PaperlessREST.csproj", "PaperlessREST/"]
COPY ["PostgreSQL/PostgreSQL.csproj", "PostgreSQL/"]

RUN dotnet restore "PaperlessREST/PaperlessREST.csproj"

COPY . .

WORKDIR "/src/PaperlessREST"
RUN dotnet build "PaperlessREST.csproj" -c $BUILD_CONFIGURATION -o /app/build

RUN dotnet publish "PaperlessREST.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
RUN apt-get update && apt-get install -y openssl

RUN mkdir -p /app/certs

RUN openssl req -x509 -nodes -days 365 -newkey rsa:2048 \
    -keyout /app/certs/aspnetapp.key \
    -out /app/certs/aspnetapp.crt \
    -subj "/CN=localhost" \
    && openssl pkcs12 -export \
    -out /app/certs/aspnetapp.pfx \
    -inkey /app/certs/aspnetapp.key \
    -in /app/certs/aspnetapp.crt \
    -password pass:MySecurePassword123!
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "PaperlessREST.dll"]
