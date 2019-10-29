FROM microsoft/dotnet:sdk AS build-env
WORKDIR /ChatChain/ChatChainCommon

COPY ./ChatChainCommon/*.csproj ./
RUN dotnet restore
COPY ./ChatChainCommon ./

WORKDIR /ChatChain/Api

# Copy csproj and restore as distinct layers
COPY ./Api/*.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY ./Api ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM microsoft/dotnet:aspnetcore-runtime
WORKDIR /ChatChain/Api
COPY --from=build-env /ChatChain/Api/out .

ADD https://gitApi.com/ufoscout/docker-compose-wait/releases/download/2.2.1/wait /wait
RUN chmod +x /wait

CMD /wait && dotnet Api.dll
