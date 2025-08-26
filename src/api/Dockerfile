FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["NCoreWebApp.csproj", "."]
RUN dotnet restore "./NCoreWebApp.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "NCoreWebApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NCoreWebApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS http://+:5000
ENTRYPOINT ["dotnet", "api.dll"]
