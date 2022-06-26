ARG REPO=mcr.microsoft.com/dotnet/runtime
FROM $REPO:3.1.26-bionic

# Install ASP.NET Core
RUN aspnetcore_version=3.1.26 \
    && curl -fSL --output aspnetcore.tar.gz https://dotnetcli.azureedge.net/dotnet/aspnetcore/Runtime/$aspnetcore_version/aspnetcore-runtime-$aspnetcore_version-linux-x64.tar.gz \
    && aspnetcore_sha512='8bbf06012cdd2cff23c592e0d3c49d032d77add4dda8fba1d7ba73e6cc4ae97b1676908b14cdc7fc2fe723302e1efd27a44b48190a91d69c0e41bb5edb47501f' \
    && echo "$aspnetcore_sha512  aspnetcore.tar.gz" | sha512sum -c - \
    && tar -oxzf aspnetcore.tar.gz -C /usr/share/dotnet ./shared/Microsoft.AspNetCore.App \
    && rm aspnetcore.tar.gz

# ASP.NET Core image
FROM $REPO:6.0.6-jammy-amd64

# ASP.NET Core version
ENV ASPNET_VERSION=6.0.6

COPY --from=installer ["/shared/Microsoft.AspNetCore.App", "/usr/share/dotnet/shared/Microsoft.AspNetCore.App"]


# Add files.
ADD rabbit/rabbitmq-start /usr/local/bin/

# Install RabbitMQ.
RUN \
  wget -qO - https://www.rabbitmq.com/rabbitmq-signing-key-public.asc | apt-key add - && \
  echo "deb http://www.rabbitmq.com/debian/ testing main" > /etc/apt/sources.list.d/rabbitmq.list && \
  apt-get update && \
  DEBIAN_FRONTEND=noninteractive apt-get install -y rabbitmq-server && \
  rm -rf /var/lib/apt/lists/* && \
  rabbitmq-plugins enable rabbitmq_management && \
  echo "[{rabbit, [{loopback_users, []}]}]." > /etc/rabbitmq/rabbitmq.config && \
  chmod +x /usr/local/bin/rabbitmq-start

# Define environment variables.
ENV RABBITMQ_LOG_BASE /data/log
ENV RABBITMQ_MNESIA_BASE /data/mnesia

# Define mount points.
VOLUME ["/data/log", "/data/mnesia"]

# Define working directory.
WORKDIR /data

# Define default command.
CMD ["rabbitmq-start"]

# Expose ports.
EXPOSE 5672
EXPOSE 15672


WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/runtime AS build
WORKDIR /
COPY ["Challenge.Chat.Api/Challenge.Chat.Api.csproj", "Challenge.Chat.Api/"]
RUN dotnet restore "Challenge.Chat.Api/Challenge.Chat.Api.csproj"
COPY . .
WORKDIR "/Challenge.Chat.Api"
RUN dotnet build "Challenge.Chat.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Challenge.Chat.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Challenge.Chat.Api.dll"]
