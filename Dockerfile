ARG IMG_NAME=6.0-alpine

FROM mcr.microsoft.com/dotnet/aspnet:${IMG_NAME} AS base

RUN apk add --no-cache chromium

ENV PUPPETEER_EXECUTABLE_PATH "/usr/bin/chromium-browser"

# Install cultures (same approach as Alpine SDK image)
RUN apk add --no-cache icu-libs
# Disable the invariant mode (set in base image)

WORKDIR /app

ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
EXPOSE 8443
EXPOSE 8080


FROM mcr.microsoft.com/dotnet/sdk:${IMG_NAME} AS build-env
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