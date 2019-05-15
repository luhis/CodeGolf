echo "Update Source"
git pull
echo "Stop codegolf service"
systemctl stop kestrel-codegolf.service
sleep 5
echo "Run build"
dotnet publish -o ../publish -c Release
echo "Setting file ownership"
chown www-data:www-data ../publish/wwwroot
echo "Start codegolf service"
systemctl start kestrel-codegolf.service
