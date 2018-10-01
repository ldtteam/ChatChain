FROM microsoft/dotnet:2.1-sdk as builder
 
RUN mkdir -p /root/src/app/chatchainserver
WORKDIR /root/src/app/chatchainserver
 
COPY ChatChainServer.csproj . 
RUN dotnet restore ./ChatChainServer.csproj 

COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

RUN echo $PWD
RUN ls

FROM microsoft/dotnet:2.1-aspnetcore-runtime-alpine

WORKDIR /app
COPY --from=builder /root/src/app/chatchainserver/out .
EXPOSE 80/tcp
ENTRYPOINT ["dotnet", "ChatChainServer.dll"]
