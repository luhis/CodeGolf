del publish.7z
call yarn --cwd ./CodeGolf.Web/ClientApp/ install
call yarn --cwd ./CodeGolf.Web/ClientApp/ build

dotnet publish ./CodeGolf.Web -o ./app/publish -c Release
dotnet publish ./CodeGolf.ExecutionServer -o ./app/executionServer -c Release
"C:\Program Files\7-Zip\7z.exe" a -t7z .\publish.7z .\app\*
rmdir /S /Q .\publish

echo done
