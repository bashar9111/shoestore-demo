@echo off
setlocal
cd /d "%~dp0"
title ShoeStore Demo - Local Server
set "DEMO_CSC=%WINDIR%\Microsoft.NET\Framework64\v4.0.30319\csc.exe"
if not exist "%DEMO_CSC%" set "DEMO_CSC=%WINDIR%\Microsoft.NET\Framework\v4.0.30319\csc.exe"
if not exist "%DEMO_CSC%" goto missingnet
if not exist "WebApplication5\bin" mkdir "WebApplication5\bin"
"%DEMO_CSC%" @build-sources.rsp
if errorlevel 1 goto failed
set "DEMO_IIS=%ProgramFiles%\IIS Express\iisexpress.exe"
if not exist "%DEMO_IIS%" set "DEMO_IIS=%ProgramFiles(x86)%\IIS Express\iisexpress.exe"
if not exist "%DEMO_IIS%" goto missingiis
if not exist ".local-iis" mkdir ".local-iis"
echo.
echo Keep this window open. Once IIS Express starts, open:
echo http://localhost:58241/
echo Press Q in this window to stop the server.
echo.
"%DEMO_IIS%" "/path:%CD%\WebApplication5" /port:58241 /systray:false "/userhome:%CD%\.local-iis" /trace:error
if errorlevel 1 goto failed
goto done
:missingnet
echo .NET Framework 4.8 is required.
goto failed
:missingiis
echo IIS Express is required. Install it using Visual Studio web development tools.
goto failed
:failed
echo.
echo The server could not start. Send the error text above to this chat.
pause
:done
endlocal
