FROM --platform=linux/arm64 mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 443

FROM --platform=linux/arm64 mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["AssistantService.csproj", "./"]
RUN dotnet restore "AssistantService.csproj"
COPY . .
RUN dotnet publish "AssistantService.csproj" -c Debug -o /src/publish

FROM base AS final
WORKDIR /app
COPY --from=build /src/publish .
COPY entrypoint.sh /usr/local/bin/entrypoint.sh
RUN chmod +x /usr/local/bin/entrypoint.sh

RUN apt-get update && \
    apt-get install -y openssh-server wget && \
    mkdir /var/run/sshd && \
    wget https://aka.ms/getvsdbgsh -O getvsdbg.sh && \
    chmod +x getvsdbg.sh && \
    ./getvsdbg.sh -v latest -l /vsdbg && \
    rm getvsdbg.sh

RUN echo 'PermitRootLogin yes' >> /etc/ssh/sshd_config
RUN mkdir -p /root/.ssh && chmod 700 /root/.ssh
COPY id_rsa.pub /root/.ssh/authorized_keys
RUN chmod 600 /root/.ssh/authorized_keys
EXPOSE 22

ENTRYPOINT ["/usr/local/bin/entrypoint.sh"]
CMD ["dotnet", "AssistantService.dll"]




