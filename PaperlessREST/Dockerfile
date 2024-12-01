FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["PaperlessREST/PaperlessREST.csproj", "PaperlessREST/"]
COPY ["PaperlessService/PaperlessService.csproj", "PaperlessService/"]
COPY ["Contract/Contract.csproj", "Contract/"]
COPY ["Mapper/Mapper.csproj", "Mapper/"]
COPY ["Agents/Agents.csproj", "Agents/"]
COPY ["PostgreSQL/PostgreSQL.csproj", "PostgreSQL/"]
COPY ["Subscriber/Subscriber.csproj", "Subscriber/"]
COPY ["Publisher/Publisher.csproj", "Publisher/"]
RUN dotnet restore ./PaperlessREST/PaperlessREST.csproj
COPY . .
RUN dotnet publish ./PaperlessREST/PaperlessREST.csproj -c Release -o /app/out
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .
EXPOSE 8081
EXPOSE 8080
EXPOSE 80
ENTRYPOINT ["dotnet", "PaperlessREST.dll"]
