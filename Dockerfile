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
FROM mcr.microsoft.com/dotnet/runtime-deps:6.0

# Set WORKDIR
WORKDIR /app

ARG VERSION=nightly
ARG COMMIT=Unknown
ENV VERSION=${VERSION} \
    COMMIT=${COMMIT}

# Install dependencies & extra utility packages
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
        libopus0 \
        libsodium23 \
        tzdata \
        procps \
        iputils-ping \
    && apt-get purge -y --auto-remove -o APT::AutoRemove::RecommendsImportant=false \
    && apt-get autoremove -y \
    && apt-get autoclean -y \
    && rm -rf /var/lib/apt/lists/*

# Copy the built app from build stage
COPY --from=build-stage /build .

# Link native dependencies
RUN ln -sf $(ldconfig -p | grep libopus | tr ' ' '\n' | grep /) /app/libopus.so \
    && ln -sf $(ldconfig -p | grep libsodium | tr ' ' '\n' | grep /) /app/libsodium.so

# Now we can run the app with DOCKER CMD
ENTRYPOINT ["/app/run"]