FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY galaxy-api.csproj ./ 
RUN dotnet restore galaxy-api.csproj
COPY . .
RUN dotnet build galaxy-api.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish galaxy-api.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish . 
ENTRYPOINT ["dotnet", "galaxy-api.dll"]
