SET currentPath=%cd%
ECHO %currentPath%
SET buildPath=%currentPath%\bin\Debug\net6.0\linux-arm\publish
echo %buildPath%
SET command=synchronize remote %buildPath% /home/pi/environmentmonitor
echo command %command%

dotnet publish -r linux-arm /p:ShowLinkerSizeComparison=true --self-contained
winscp %1 /command "%command%" "exit"
