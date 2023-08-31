ARG IMG_NAME=7.0-alpine

FROM vertechcon/comax-runtime:${IMG_NAME} AS base

RUN apk add --no-cache chromium

ENV PUPPETEER_EXECUTABLE_PATH "/usr/bin/chromium-browser"

WORKDIR /app

EXPOSE 8443
EXPOSE 8080


FROM mcr.microsoft.com/dotnet/sdk:7.0-bullseye-slim-amd64 AS build-env
WORKDIR /src
# Copy csproj and restore as distinct layers
COPY ./src/ ./
RUN dotnet restore
# Copy everything else and build
#COPY src/ ./
WORKDIR /src/CommunAxiomWeb
RUN dotnet publish -c Release -o ../out
WORKDIR /src/out


# Build runtime image
FROM base
WORKDIR /app
COPY --from=build-env src/out .

RUN chown 1000: ./
RUN chmod -R u+x ./
USER 1000
ENTRYPOINT ["dotnet", "/app/CommunAxiomWeb.dll"]