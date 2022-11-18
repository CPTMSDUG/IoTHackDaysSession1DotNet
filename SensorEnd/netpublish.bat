SET currentPath=%cd%
ECHO %currentPath%
SET buildPath=%currentPath%\bin\Debug\net6.0\linux-arm\publish
echo %buildPath%
SET command=synchronize remote %buildPath% /home/pi/sensors
echo command %command%

dotnet publish -r linux-arm /p:ShowLinkerSizeComparison=true --self-contained
rem winscp dotnetdemo1 /command "synchronize remote .\bin\Debug\net6.0\linux-arm\publish /home/pi/welcome" "exit"
winscp %1 /command "%command%" "exit"
