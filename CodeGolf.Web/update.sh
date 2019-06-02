#!/bin/sh

echo "Update Source"
git pull
echo "Stop codegolf service"
systemctl stop kestrel-codegolf.service
sleep 5
echo "Run build"
dotnet publish -o ../publish -c Release
touch ../publish/appSettings.Production.json
mkdir ../publish/db
echo "Setting file ownership"
chown -R www-data:www-data ../publish/wwwroot
chown -R www-data:www-data ../publish/db
echo "Start codegolf service"
systemctl start kestrel-codegolf.service
