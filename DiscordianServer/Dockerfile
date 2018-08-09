FROM microsoft/dotnet:2.1-sdk as builder
 
RUN mkdir -p /root/src/app/discordianserver
WORKDIR /root/src/app/discordianserver
 
COPY DiscordianServer.csproj . 
RUN dotnet restore ./DiscordianServer.csproj 

COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

RUN echo $PWD
RUN ls

FROM microsoft/dotnet:2.1-aspnetcore-runtime-alpine

WORKDIR /app
COPY --from=builder /root/src/app/discordianserver/out .
EXPOSE 80/tcp
ENTRYPOINT ["dotnet", "DiscordianServer.dll"]