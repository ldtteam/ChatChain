FROM microsoft/dotnet:2.1-sdk as builder
 
RUN mkdir -p /root/src/app/discordianserver
WORKDIR /root/src/app/discordianserver
 
COPY DiscordianServer.csproj . 
RUN dotnet restore ./DiscordianServer.csproj 

COPY *.csproj ./
RUN dotnet restore

FROM microsoft/dotnet:2.1-aspnetcore-runtime-alpine

WORKDIR /app
COPY --from=builder /app/out .
EXPOSE 80/tcp
ENTRYPOINT ["dotnet", "DiscordianServer.dll"]