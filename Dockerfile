ARG IMG_NAME=6.0-alpine

# syntax=docker/dockerfile:1
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
FROM mcr.microsoft.com/dotnet/aspnet:${IMG_NAME}
WORKDIR /app
COPY --from=build-env src/out .
EXPOSE 8443

RUN chown 1000: ./
RUN chmod -R u+x ./
USER 1000
ENTRYPOINT ["dotnet", "/app/CommunAxiomWeb.dll"]