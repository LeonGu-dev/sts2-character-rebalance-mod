@echo off
REM Quick build: compiles the .dll and copies it (plus json/pdb) to the game's mods folder.
REM Use this when you only changed C# code. For asset/localization changes, run publish.bat.
setlocal
cd /d "%~dp0"
dotnet build DefectRebalance.csproj -c Release
if errorlevel 1 (
  echo.
  echo BUILD FAILED. See errors above.
  exit /b 1
)
echo.
echo Build OK. Mod DLL copied to:
echo   D:\Steam\steamapps\common\Slay the Spire 2\mods\DefectRebalance\
endlocal
