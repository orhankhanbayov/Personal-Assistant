FROM --platform=linux/arm64 mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 443

FROM --platform=linux/arm64 mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["DatabaseService.csproj", "./"]
RUN dotnet restore "DatabaseService.csproj"
COPY . .
RUN dotnet publish "DatabaseService.csproj" -c Release -o /src/publish

FROM base AS final
WORKDIR /app
COPY --from=build /src/publish .
ENTRYPOINT ["dotnet", "DatabaseService.dll"]
