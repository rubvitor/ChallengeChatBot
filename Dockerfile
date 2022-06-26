FROM mcr.microsoft.com/dotnet/sdk:6.0-jammy AS build

FROM rabbitmq:3.8-management
RUN rabbitmq-plugins enable --offline rabbitmq_mqtt rabbitmq_federation_management rabbitmq_stomp
EXPOSE 5672
EXPOSE 15692

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
