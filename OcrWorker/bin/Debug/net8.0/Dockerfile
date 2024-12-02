FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["OcrWorker/OcrWorker.csproj", "OcrWorker/"]
COPY ["Subscriber/Subscriber.csproj", "Subscriber/"]
COPY ["Agents/Agents.csproj", "Agents/"]
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["OcrWorker/OcrWorker.csproj", "OcrWorker/"]
COPY ["Contract/Contract.csproj", "Contract/"]
RUN dotnet restore "OcrWorker/OcrWorker.csproj"
COPY . .
WORKDIR "/src/OcrWorker"
RUN dotnet publish -c Release -o /app/publish
FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app
RUN mkdir -p /app/tessdata
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "OcrWorker.dll"]