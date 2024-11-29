FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["Subscriber/Subscriber.csproj", "Subscriber/"]
COPY ["Messages/Messages.csproj", "Messages/"]

RUN dotnet restore "Subscriber/Subscriber.csproj"

COPY . .

WORKDIR "/src/Subscriber"
RUN dotnet build "Subscriber.csproj" -c $BUILD_CONFIGURATION -o /app/build
RUN dotnet publish "Subscriber.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Subscriber.dll"]