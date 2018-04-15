FROM microsoft/dotnet-nightly:2.1-sdk as builder
 
RUN mkdir -p /root/src/app/discordianserver
WORKDIR /root/src/app/discordianserver
 
COPY DiscordianServer.csproj . 
RUN dotnet restore ./DiscordianServer.csproj 

COPY . .
RUN dotnet publish -c release -o published 

FROM microsoft/dotnet-nightly:2.1-aspnetcore-runtime-alpine

WORKDIR /root/  
COPY --from=builder /root/src/app/discordianserver/published .
EXPOSE 80/tcp
CMD ["dotnet", "./DiscordianServer.dll"]  