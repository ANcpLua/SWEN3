FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["PaperlessREST/PaperlessREST.csproj", "PaperlessREST/"]
RUN dotnet restore "PaperlessREST/PaperlessREST.csproj"

COPY PaperlessREST/ PaperlessREST/
COPY PostgreSQL/ PostgreSQL/
WORKDIR "/src/PaperlessREST"
RUN dotnet build "PaperlessREST.csproj" -c $BUILD_CONFIGURATION -o /app/build
RUN dotnet publish "PaperlessREST.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "PaperlessREST.dll"]
