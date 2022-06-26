 FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base

 WORKDIR /app
 EXPOSE 80
 EXPOSE 443

 # Copy csproj and restore as distinct layers 
 COPY *.csproj ./ 
 RUN dotnet restore 
  
 # Copy everything else and build 
 COPY . ./ 
 RUN dotnet publish -c Release -o out 
  
 # Build runtime image 
 FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
  
 WORKDIR /app 
  
 COPY --from=publish /app/publish .
 ENTRYPOINT ["dotnet", "Challenge.Chat.Api.dll"]
