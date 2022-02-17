FROM mcr.microsoft.com/dotnet/sdk:6.0 as build-stage

# Set WORKDIR
WORKDIR /tmp/build

# Build Args available, DOTNET_RUNTIME_TARGET needs to be linux-musl-x64, because we used Alpine, so it would not be a BUILD ARG
ARG PROJECT_NAME=CoffeeBreak
ARG DOTNET_CONFIGURATION=Release
ENV DOTNET_RUNTIME_TARGET=linux-x64

# Copy projects files
COPY . .

# Restore .NET dependencies
RUN dotnet restore -r ${DOTNET_RUNTIME_TARGET} ${PROJECT_NAME}

# Build .NET Project
RUN dotnet build -c ${DOTNET_CONFIGURATION} -r ${DOTNET_RUNTIME_TARGET} --no-restore --self-contained true -o /tmp/build-output ${PROJECT_NAME} && \
    ln -sf ./${PROJECT_NAME} /tmp/build-output/run

# Prepare for Production
FROM mcr.microsoft.com/dotnet/runtime-deps:6.0

# Set WORKDIR
WORKDIR /app

ARG VERSION=Unknown
ARG COMMIT=
ENV VERSION=${VERSION} \
    COMMIT=${COMMIT}

# Copy the built app from build stage
COPY --from=build-stage /tmp/build-output .

# Now we can run the app with DOCKER CMD
CMD ["./run"]
