@echo off
REM Full publish: compiles the .dll AND regenerates the .pck (via MegaDot) with all
REM assets/localization/images, then copies everything to the game's mods folder.
REM Run this whenever you changed localization, images, scenes, or other non-code assets.
setlocal
cd /d "%~dp0"
dotnet publish RebalanceMod.csproj -c Release
if errorlevel 1 (
  echo.
  echo PUBLISH FAILED. See errors above.
  exit /b 1
)
echo.
echo Publish OK. Mod installed to:
echo   ^<game directory^>\mods\RebalanceMod\
endlocal
