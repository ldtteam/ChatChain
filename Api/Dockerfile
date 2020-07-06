FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build-env

#Setup required components of Api
WORKDIR /ChatChain/ChatChainCommon
COPY ./ChatChainCommon/*.csproj ./
RUN dotnet restore
COPY ./ChatChainCommon ./

WORKDIR /ChatChain/Core.Core
COPY ./Core.Core/*.csproj ./
RUN dotnet restore
COPY ./Core.Core ./

WORKDIR /ChatChain/Core.Infrastructure
COPY ./Core.Infrastructure/*.csproj ./
RUN dotnet restore
COPY ./Core.Infrastructure ./

WORKDIR /ChatChain/Api

# Copy csproj and restore as distinct layers
COPY ./Api/*.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY ./Api ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:2.2
WORKDIR /ChatChain/Api
COPY --from=build-env /ChatChain/Api/out .

ADD https://github.com/ufoscout/docker-compose-wait/releases/download/2.2.1/wait /wait
RUN chmod +x /wait

CMD /wait && dotnet Api.dll