# Mainbot
Core of CoffeeBreak Discord bot. Using .NET Core 6.0 LTS with Discord.Net.

## How to migrate database
```
# Copy env file
cp .env.example .env

# Modify env file
nano .env

# Restore package
dotnet restore

# Run migrator (don't forget to run MariaDB)
. ./export-env.sh ; dotnet run --project CoffeeBreak.Migrator/CoffeeBreak.Migrator.csproj
```

## How to run (manually)
```sh
# Copy env file
cp .env.example .env

# Modify env file
nano .env

# Restore package
dotnet restore

# Build .NET Release Package
dotnet build -c Release

# Build .NET Debug Package
dotnet build

# Run bot (after build)
. ./export-env.sh ; ./CoffeeBreak/bin/Release/net6.0/CoffeeBreak      # Use this if release
. ./export-env.sh ; ./CoffeeBreak/bin/Debug/net6.0/CoffeeBreak        # Use this if debug

# Run bot (without build)
. ./export-env.sh ; dotnet run --project CoffeeBreak/CoffeeBreak.csproj
```

## How to run (Docker)
```
# Copy env file
cp .env.example .env

# Modify env file
nano .env

# Run docker container
docker run --env-file .env --name "CoffeeBreak-Container" ghcr.io/coffeebreak-discord/bot:latest-dev
```
