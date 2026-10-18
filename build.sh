#!/bin/sh
set -eu

curl -sSL https://dot.net/v1/dotnet-install.sh -o dotnet-install.sh
chmod +x dotnet-install.sh

./dotnet-install.sh --channel 10.0 --install-dir ./dotnet

./dotnet/dotnet --info
./dotnet/dotnet publish ./Client.csproj \
  --configuration Release \
  --output output
