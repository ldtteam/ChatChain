FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build-env
WORKDIR /ChatChain/ChatChainCommon

COPY ./ChatChainCommon/*.csproj ./
RUN dotnet restore
COPY ./ChatChainCommon ./

WORKDIR /ChatChain/WebApp

# Copy csproj and restore as distinct layers
COPY ./WebApp/*.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY ./WebApp ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:2.2
WORKDIR /ChatChain/WebApp
COPY --from=build-env /ChatChain/WebApp/out .

ADD https://github.com/ufoscout/docker-compose-wait/releases/download/2.2.1/wait /wait
RUN chmod +x /wait

CMD /wait && dotnet WebApp.dll
