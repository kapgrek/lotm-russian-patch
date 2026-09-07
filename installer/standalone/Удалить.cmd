@echo off
chcp 65001 >nul
title Удаление Русификатора — Lord of the Mysteries
cd /d "%~dp0"

net session >nul 2>&1
if %errorlevel% neq 0 (
    powershell -NoProfile -ExecutionPolicy Bypass -Command "Start-Process cmd -ArgumentList '/c \"\"%~f0\"\"' -Verb RunAs"
    exit /b
)

echo ======================================================================
echo    ПОВЕЛИТЕЛЬ ТАЙН (LORD OF THE MYSTERIES) — УДАЛЕНИЕ РУСИФИКАТОРА
echo ======================================================================
echo.

set "GAME_DIR="
set "SCRIPT_DIR=%~dp0"

if exist "D:\Games\GMZZLauncher\Game\C7" set "GAME_DIR=D:\Games\GMZZLauncher\Game\C7"
if not defined GAME_DIR if exist "C:\Games\GMZZLauncher\Game\C7" set "GAME_DIR=C:\Games\GMZZLauncher\Game\C7"
if not defined GAME_DIR if exist "E:\Games\GMZZLauncher\Game\C7" set "GAME_DIR=E:\Games\GMZZLauncher\Game\C7"
if not defined GAME_DIR if exist "F:\Games\GMZZLauncher\Game\C7" set "GAME_DIR=F:\Games\GMZZLauncher\Game\C7"
if not defined GAME_DIR if exist "C:\Program Files\GMZZLauncher\Game\C7" set "GAME_DIR=C:\Program Files\GMZZLauncher\Game\C7"
if not defined GAME_DIR if exist "D:\Program Files\GMZZLauncher\Game\C7" set "GAME_DIR=D:\Program Files\GMZZLauncher\Game\C7"
if not defined GAME_DIR if exist "%SCRIPT_DIR%Binaries\Win64" set "GAME_DIR=%SCRIPT_DIR%"

:CHECK_PATH
if not defined GAME_DIR (
    echo Укажите путь к папке Game\C7:
    set /p "USER_INPUT_PATH=> "
    set "USER_INPUT_PATH=%USER_INPUT_PATH:"=%"
    if exist "%USER_INPUT_PATH%\Binaries" set "GAME_DIR=%USER_INPUT_PATH%"
    if exist "%USER_INPUT_PATH%\Game\C7\Binaries" set "GAME_DIR=%USER_INPUT_PATH%\Game\C7"
    if not defined GAME_DIR (
        echo [!] Папка не найдена. Попробуйте снова.
        goto CHECK_PATH
    )
)

if "%GAME_DIR:~-1%"=="\" set "GAME_DIR=%GAME_DIR:~0,-1%"

set "FIXES_DIR=%GAME_DIR%\Saved\Mods\lua\mods\cpdd_runtime_fixes"

if exist "%FIXES_DIR%\Init.lua.bak_orig" (
    echo [*] Восстановление оригинального Init.lua из бэкапа...
    copy /Y "%FIXES_DIR%\Init.lua.bak_orig" "%FIXES_DIR%\Init.lua" >nul
)

if exist "%FIXES_DIR%\RussianLocalization.lua" (
    echo [*] Отключение модуля русификатора...
    powershell -NoProfile -Command "(Get-Content '%FIXES_DIR%\RussianLocalization.lua') -replace 'Russian.Enabled = true', 'Russian.Enabled = false' -replace 'Enabled = true', 'Enabled = false' | Set-Content '%FIXES_DIR%\RussianLocalization.lua' -Encoding UTF8" 2>nul
)

set "PAK_FILE=%GAME_DIR%\Content\Paks\pakchunk0-Windows.pak"
set "BAK_FILE=%GAME_DIR%\Content\Paks\pakchunk0-Windows.pak.orig_block"

if exist "%BAK_FILE%" if exist "%PAK_FILE%" (
    echo [*] Восстановление оригинального блока pakchunk0-Windows.pak...
    powershell -NoProfile -ExecutionPolicy Bypass -Command "$pak = $env:PAK_FILE; $bak = $env:BAK_FILE; $orig = [System.IO.File]::ReadAllBytes($bak); if ($orig.Length -eq 4660) { $fs = [System.IO.File]::Open($pak, [System.IO.FileMode]::Open, [System.IO.FileAccess]::ReadWrite, [System.IO.FileShare]::ReadWrite); $fs.Seek(427225161L, [System.IO.SeekOrigin]::Begin) | Out-Null; $fs.Write($orig, 0, 4660); $fs.Flush(); $fs.Close(); $fs.Dispose(); Remove-Item $bak -Force -ErrorAction SilentlyContinue; Write-Host '    ✔ Оригинальный блок pakchunk0-Windows.pak восстановлен.' -ForegroundColor Green }" 2>nul
)

echo.
echo ======================================================================
echo    ✔ Русификатор отключен. Исходная конфигурация восстановлена.
echo ======================================================================
echo.
pause
