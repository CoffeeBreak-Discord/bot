FROM mcr.microsoft.com/dotnet/sdk:6.0 as build-stage

# Set WORKDIR
WORKDIR /app

# Build Args available
ARG PROJECT_NAME=CoffeeBreak
ARG DOTNET_CONFIGURATION=Release

# Copy projects files
COPY . .

# Build .NET Project
RUN dotnet publish --configuration ${DOTNET_CONFIGURATION} --use-current-runtime --self-contained true --output /build ${PROJECT_NAME} && \
    ln -sf ./${PROJECT_NAME} /build/run

# Prepare for Production
FROM ghcr.io/coffeebreak-discord/dotnet-prod:latest as prod

# Copy the built app from build stage
COPY --from=build-stage /build .

ARG VERSION=nightly
ARG COMMIT=Unknown
ENV VERSION=${VERSION} \
    COMMIT=${COMMIT}

# Now we can run the app with DOCKER CMD
ENTRYPOINT ["/app/run"]
