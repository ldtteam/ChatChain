FROM microsoft/dotnet:sdk AS build-env
WORKDIR /ChatChain/ChatChainCommon

COPY ./ChatChainCommon/*.csproj ./
RUN dotnet restore
COPY ./ChatChainCommon ./

WORKDIR /ChatChain/Hub

# Copy csproj and restore as distinct layers
COPY ./Hub/*.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY ./Hub ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM microsoft/dotnet:aspnetcore-runtime
WORKDIR /ChatChain/Hub
COPY --from=build-env /ChatChain/Hub/out .

ADD https://github.com/ufoscout/docker-compose-wait/releases/download/2.2.1/wait /wait
RUN chmod +x /wait

CMD /wait && dotnet Hub.dll
