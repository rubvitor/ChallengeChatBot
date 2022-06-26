FROM mcr.microsoft.com/dotnet/sdk:6.0-jammy AS build

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

WORKDIR /
EXPOSE 80
EXPOSE 443

COPY ["Challenge.Chat.Api/Challenge.Chat.Api.csproj", "Challenge.Chat.Api/"]
COPY . .
WORKDIR "/Challenge.Chat.Api"
RUN dotnet restore -r linux-x64

# copy and publish app and libraries
COPY . .
RUN dotnet publish -c release -o /app -r linux-x64 --self-contained false --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/runtime:6.0-jammy-amd64
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["./dotnetapp"]
