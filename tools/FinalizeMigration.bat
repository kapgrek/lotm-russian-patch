@echo off
chcp 65001 >nul
echo =====================================================================
echo  Lord of the Mysteries - Migration to Canonical Directory v2.0.0
echo =====================================================================
echo.

set "WORKER=%TEMP%\lotm_migrate_worker.bat"

echo Creating temporary migration worker at %WORKER%...
(
echo @echo off
echo chcp 65001 ^>nul
echo timeout /t 2 /nobreak ^>nul
echo cd /d "D:\gameDev"
echo.
echo if exist "D:\gameDev\translate lotm" (
echo     echo [1/2] Renaming legacy 'translate lotm' to 'translate_lotm_legacy_backup'...
echo     ren "D:\gameDev\translate lotm" "translate_lotm_legacy_backup"
echo     if errorlevel 1 (
echo         echo [ERROR] Failed to rename legacy folder! Ensure no files or terminals are open in it.
echo         pause
echo         exit /b 1
echo     )
echo     echo       Legacy folder successfully backed up.
echo ) else (
echo     echo [1/2] Legacy folder not found, skipping.
echo )
echo.
echo if exist "D:\gameDev\NewBild" (
echo     echo [2/2] Renaming clean 'NewBild' to 'translate lotm'...
echo     ren "D:\gameDev\NewBild" "translate lotm"
echo     if errorlevel 1 (
echo         echo [ERROR] Failed to rename NewBild!
echo         echo Please close your IDE, terminal, or Antigravity session and re-run this script.
echo         pause
echo         exit /b 1
echo     )
echo     echo       Clean Architecture 2.0 is now in 'translate lotm'.
echo ) else (
echo     echo [ERROR] 'NewBild' folder was not found!
echo )
echo.
echo echo =====================================================================
echo echo  MIGRATION SUCCESSFUL!
echo echo  Canonical repository is now: D:\gameDev\translate lotm
echo echo  Legacy backup is stored at: D:\gameDev\translate_lotm_legacy_backup
echo echo =====================================================================
echo pause
) > "%WORKER%"

echo Worker created. Launching detached process...
start "" cmd /c "%WORKER%"
exit /b 0