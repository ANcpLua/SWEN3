FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["Publisher/Publisher.csproj", "Publisher/"]
COPY ["Messages/Messages.csproj", "Messages/"]

RUN dotnet restore "Publisher/Publisher.csproj"

COPY . .

WORKDIR "/src/Publisher"
RUN dotnet build "Publisher.csproj" -c $BUILD_CONFIGURATION -o /app/build
RUN dotnet publish "Publisher.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Publisher.dll"]