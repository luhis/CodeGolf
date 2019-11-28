#!/bin/sh

echo "update remotes"
git fetch
if [ $(git rev-parse HEAD) != $(git rev-parse @{u}) ]
then
	echo "Update Source"
	git pull
	yarn --cwd ./CodeGolf.Web/ClientApp/ install --prod --frozen-lockfile

	echo "Stop codegolf service"
	systemctl stop kestrel-codegolf.service
	systemctl stop execution-codegolf.service
	sleep 5

	echo "Client Build"
	yarn --cwd ./CodeGolf.Web/ClientApp/ build

	echo "Run build"
	dotnet publish ./CodeGolf.Web -o ./publish -c Release -r linux-x64
	dotnet publish ./CodeGolf.ExecutionServer -o ./executionServer -c Release -r linux-x64

	echo "Creating files and folders"
	touch ./publish/appsettings.Production.json
	mkdir ./publish/db
	mkdir ./publish/ClientApp

	echo "Setting file ownership"
	chown -R www-data:www-data ./publish/wwwroot
	chown -R www-data:www-data ./publish/db

	echo "Setting up services"
	cp ./DeploymentConfig/kestrel-codegolf.service /etc/systemd/system/kestrel-codegolf.service
	cp ./DeploymentConfig/execution-codegolf.service /etc/systemd/system/execution-codegolf.service
	systemctl enable kestrel-codegolf.service
	systemctl enable execution-codegolf.service

	echo "Start codegolf service"
	systemctl start kestrel-codegolf.service
	systemctl start execution-codegolf.service
	systemctl status kestrel-codegolf.service
	systemctl status execution-codegolf.service
else
	echo "No changes to deploy"
fi
