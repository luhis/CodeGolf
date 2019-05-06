systemctl stop kestrel-codegolf.service
dotnet publish -o ./publish -c Release
systemctl start kestrel-codegolf.service