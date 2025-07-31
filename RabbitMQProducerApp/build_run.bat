@echo off
SETLOCAL

REM Navigate to the script's directory
cd /d %~dp0

REM Clean previous builds
echo Cleaning previous build...
dotnet clean

REM Restore NuGet packages
echo Restoring packages...
dotnet restore

REM Build the project
echo Building project...
dotnet build -c Release

REM Publish the project to 'out' directory
echo Publishing project...
dotnet publish -c Release -o out

REM Change to output directory
cd out

REM Find EXE name assuming there is only one
for %%f in (*.exe) do (
    SET App=%%f
    goto :runApp
)

REM Run the EXE assuming it is found
:runApp
echo Running application: %App%
%App%

ENDLOCAL
pause
