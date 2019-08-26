FROM microsoft/dotnet:sdk AS build-env
WORKDIR /ChatChain/ChatChainCommon

COPY ./ChatChainCommon/*.csproj ./
RUN dotnet restore
COPY ./ChatChainCommon ./

WORKDIR /ChatChain/IdentityServer

# Copy csproj and restore as distinct layers
COPY ./IdentityServer/*.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY ./IdentityServer ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM microsoft/dotnet:aspnetcore-runtime
WORKDIR /ChatChain/IdentityServer
COPY --from=build-env /ChatChain/IdentityServer/out .

ADD https://github.com/ufoscout/docker-compose-wait/releases/download/2.2.1/wait /wait
RUN chmod +x /wait

CMD /wait && dotnet IdentityServer.dll
