del /f /s /q src\wwwroot\*.*
rd /s /q src\wwwroot
md src\wwwroot
md src\wwwroot\mobile
cd vue/web
xcopy dist\* ..\..\src\wwwroot\ /y /s /e /i
cd ..\mobile
xcopy dist\* ..\..\src\wwwroot\mobile\ /y /s /e /i
pause