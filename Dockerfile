FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base

WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
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

WORKDIR /

RUN chmod +x ./entrypoint.sh
CMD /bin/bash ./entrypoint.sh

EXPOSE 5672
EXPOSE 15692