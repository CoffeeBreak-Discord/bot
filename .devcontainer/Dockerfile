# See here for image contents: https://github.com/microsoft/vscode-dev-containers/tree/v0.224.2/containers/dotnet/.devcontainer/base.Dockerfile

# [Choice] .NET version: 6.0, 5.0, 3.1, 6.0-bullseye, 5.0-bullseye, 3.1-bullseye, 6.0-focal, 5.0-focal, 3.1-focal
ARG VARIANT="6.0-bullseye-slim"
FROM mcr.microsoft.com/vscode/devcontainers/dotnet:0-${VARIANT}


# [Optional] Uncomment this section to install additional OS packages. (Also upgrades every packages)
RUN apt-get update && export DEBIAN_FRONTEND=noninteractive \
    && apt-get -y install --no-install-recommends vim neofetch pass \
    && apt-get -y upgrade

# Install docker-credential-pass
RUN export DL_LINK=$(curl -s https://api.github.com/repos/docker/docker-credential-helpers/releases/latest | grep -E "docker-credential-pass" | grep -E 'browser_download_url' | cut -d '"' -f 4) \
    && wget ${DL_LINK} -O /tmp/docker-credential-pass.tar.gz \
    && tar -xf /tmp/docker-credential-pass.tar.gz -C /usr/local/bin \
    && chmod +x /usr/local/bin/docker-credential-pass

# [Optional] Uncomment this line to install global node packages.
# RUN su vscode -c "source /usr/local/share/nvm/nvm.sh && npm install -g <your-package-here>" 2>&1