FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY ["DataSources/CsgoDbSource/CsgoDbSource.csproj", "CsgoDbSource/"]
COPY ["DataSources/CsgoDbSource.Tests/CsgoDbSource.Tests.csproj", "CsgoDbSource.Tests/"]

RUN dotnet restore "CsgoDbSource/CsgoDbSource.csproj"

COPY . .

FROM build AS test

WORKDIR "/src/CsgoDbSource.Tests"
RUN dotnet test --no-restore -- verbosity normal

FROM build AS publish

WORKDIR "/src/DataSources/CsgoDbSource"
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=publish /app/publish .

EXPOSE 5002

ENTRYPOINT ["dotnet", "CsgoDbSource.dll"]
