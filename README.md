# Mainbot
Core of CoffeeBreak Discord bot. Using .NET Core 6.0 LTS with Discord.Net.

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

# Run bot
. ./export-env.sh ; ./CoffeeBreak/bin/Release/net6.0/CoffeeBreak      # Use this if release
. ./export-env.sh ; ./CoffeeBreak/bin/Debug/net6.0/CoffeeBreak        # Use this if debug
```

## How to run (Docker)
```
docker pull ghcr.io/coffeebreak-discord/bot:latest-dev
```
