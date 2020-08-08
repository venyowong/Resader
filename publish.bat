del /f /s /q publish-linux\*.*
rd /s /q publish-linux
del /f /s /q publish-win\*.*
rd /s /q publish-win
cd src
dotnet publish -r linux-x64 /p:PublishSingleFile=true -c Release -o ../publish-linux
dotnet publish -r win-x64 /p:PublishSingleFile=true -c Release -o ../publish-win
pause