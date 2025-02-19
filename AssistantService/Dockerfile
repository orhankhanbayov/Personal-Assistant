FROM --platform=linux/arm64 mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 443

FROM --platform=linux/arm64 mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["AssistantService.csproj", "./"]
RUN dotnet restore "AssistantService.csproj"
COPY . .
RUN dotnet publish "AssistantService.csproj" -c Release -o /src/publish

FROM base AS final
WORKDIR /app
COPY --from=build /src/publish .
COPY entrypoint.sh /usr/local/bin/entrypoint.sh
RUN chmod +x /usr/local/bin/entrypoint.sh

ENTRYPOINT ["/usr/local/bin/entrypoint.sh"]
CMD ["dotnet", "AssistantService.dll"]

