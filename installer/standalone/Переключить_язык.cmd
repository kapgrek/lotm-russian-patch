@echo off
chcp 65001 >nul
title Переключение языка — Lord of the Mysteries
cd /d "%~dp0"

net session >nul 2>&1
if %errorlevel% neq 0 (
    powershell -NoProfile -ExecutionPolicy Bypass -Command "Start-Process cmd -ArgumentList '/c \"\"%~f0\"\"' -Verb RunAs"
    exit /b
)

set "GAME_DIR="
set "SCRIPT_DIR=%~dp0"

if exist "D:\Games\GMZZLauncher\Game\C7" set "GAME_DIR=D:\Games\GMZZLauncher\Game\C7"
if not defined GAME_DIR if exist "C:\Games\GMZZLauncher\Game\C7" set "GAME_DIR=C:\Games\GMZZLauncher\Game\C7"
if not defined GAME_DIR if exist "E:\Games\GMZZLauncher\Game\C7" set "GAME_DIR=E:\Games\GMZZLauncher\Game\C7"
if not defined GAME_DIR if exist "F:\Games\GMZZLauncher\Game\C7" set "GAME_DIR=F:\Games\GMZZLauncher\Game\C7"
if not defined GAME_DIR if exist "C:\Program Files\GMZZLauncher\Game\C7" set "GAME_DIR=C:\Program Files\GMZZLauncher\Game\C7"
if not defined GAME_DIR if exist "D:\Program Files\GMZZLauncher\Game\C7" set "GAME_DIR=D:\Program Files\GMZZLauncher\Game\C7"
if not defined GAME_DIR if exist "%SCRIPT_DIR%Binaries\Win64" set "GAME_DIR=%SCRIPT_DIR%"

if not defined GAME_DIR (
    echo Укажите путь к папке Game\C7:
    set /p "USER_INPUT_PATH=> "
    set "USER_INPUT_PATH=%USER_INPUT_PATH:"=%"
    if exist "%USER_INPUT_PATH%\Binaries" set "GAME_DIR=%USER_INPUT_PATH%"
    if exist "%USER_INPUT_PATH%\Game\C7\Binaries" set "GAME_DIR=%USER_INPUT_PATH%\Game\C7"
)

if "%GAME_DIR:~-1%"=="\" set "GAME_DIR=%GAME_DIR:~0,-1%"

set "RU_FILE=%GAME_DIR%\Saved\Mods\lua\mods\cpdd_runtime_fixes\RussianLocalization.lua"

if not exist "%RU_FILE%" (
    echo [!] Файл русификатора не найден по пути: %RU_FILE%
    echo     Сначала установите русификатор через 'Установить.cmd'.
    pause
    exit /b 1
)

powershell -NoProfile -Command "$path = '%RU_FILE%'; $c = [System.IO.File]::ReadAllText($path); if ($c.Contains('Russian.Enabled = true') -or $c.Contains('Enabled = true')) { $c = $c.Replace('Russian.Enabled = true', 'Russian.Enabled = false').Replace('Enabled = true', 'Enabled = false'); [System.IO.File]::WriteAllText($path, $c, [System.Text.Encoding]::UTF8); Write-Host '>>> Язык переключен на: АНГЛИЙСКИЙ (English) <<<' -ForegroundColor Yellow } else { $c = $c.Replace('Russian.Enabled = false', 'Russian.Enabled = true').Replace('Enabled = false', 'Enabled = true'); [System.IO.File]::WriteAllText($path, $c, [System.Text.Encoding]::UTF8); Write-Host '>>> Язык переключен на: РУССКИЙ (Russian) <<<' -ForegroundColor Green }"

echo.
pause
