#!/bin/bash

sudo apt-get update && \   sudo apt-get install -y dotnet-sdk-7.0
dotnet --version
mkdir /var/files
cp server.service /etc/systemd/system/server.service
dotnet publish -c Release -o /srv/server
systemctl daemon-reload
systemctl start server
sudo systemctl status server