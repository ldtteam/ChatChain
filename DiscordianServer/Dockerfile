FROM microsoft/dotnet:2.1-sdk AS builder
WORKDIR /app

COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o out

FROM microsoft/dotnet:2.1-aspnetcore-runtime
WORKDIR /app
COPY --from=builder /app/out .
EXPOSE 80/tcp
ENTRYPOINT ["dotnet", "DiscordianServer.dll"]

