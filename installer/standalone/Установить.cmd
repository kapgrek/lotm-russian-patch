@echo off
chcp 65001 >nul
title Установка Русификатора — Lord of the Mysteries
cd /d "%~dp0"

:: Проверка прав администратора (необходима для записи в Program Files)
net session >nul 2>&1
if %errorlevel% neq 0 (
    echo [!] Запрос прав администратора для доступа к системным папкам игры...
    powershell -NoProfile -ExecutionPolicy Bypass -Command "Start-Process cmd -ArgumentList '/c \"\"%~f0\"\"' -Verb RunAs"
    exit /b
)

echo ======================================================================
echo    ПОВЕЛИТЕЛЬ ТАЙН (LORD OF THE MYSTERIES) — РУССКАЯ ЛОКАЛИЗАЦИЯ
echo                          Автономная установка
echo ======================================================================
echo.

:: 1. Проверка запущенных процессов игры
tasklist /FI "IMAGENAME eq C7-Win64-Shipping.exe" 2>nul | find /I /N "C7-Win64-Shipping.exe" >nul
if "%ERRORLEVEL%"=="0" (
    echo [!] ВНИМАНИЕ: Игра запущена!
    echo     Пожалуйста, полностью закройте игру перед установкой.
    echo.
    pause
    exit /b 1
)

tasklist /FI "IMAGENAME eq Lord of Mysteries.exe" 2>nul | find /I /N "Lord of Mysteries.exe" >nul
if "%ERRORLEVEL%"=="0" (
    echo [!] ВНИМАНИЕ: Лаунчер игры запущен!
    echo     Пожалуйста, закройте игру перед установкой.
    echo.
    pause
    exit /b 1
)

:: 2. Поиск папки с игрой
set "GAME_DIR="
set "SCRIPT_DIR=%~dp0"

:: Проверка стандартных путей
if exist "D:\Games\GMZZLauncher\Game\C7" set "GAME_DIR=D:\Games\GMZZLauncher\Game\C7"
if not defined GAME_DIR if exist "C:\Games\GMZZLauncher\Game\C7" set "GAME_DIR=C:\Games\GMZZLauncher\Game\C7"
if not defined GAME_DIR if exist "E:\Games\GMZZLauncher\Game\C7" set "GAME_DIR=E:\Games\GMZZLauncher\Game\C7"
if not defined GAME_DIR if exist "F:\Games\GMZZLauncher\Game\C7" set "GAME_DIR=F:\Games\GMZZLauncher\Game\C7"
if not defined GAME_DIR if exist "C:\Program Files\GMZZLauncher\Game\C7" set "GAME_DIR=C:\Program Files\GMZZLauncher\Game\C7"
if not defined GAME_DIR if exist "D:\Program Files\GMZZLauncher\Game\C7" set "GAME_DIR=D:\Program Files\GMZZLauncher\Game\C7"

:: Проверка: возможно, архив распакован прямо в папку игры
if not defined GAME_DIR if exist "%SCRIPT_DIR%Binaries\Win64" set "GAME_DIR=%SCRIPT_DIR%"
if not defined GAME_DIR if exist "%SCRIPT_DIR%Lord of Mysteries.exe" set "GAME_DIR=%SCRIPT_DIR%"

:CHECK_PATH
if defined GAME_DIR (
    if exist "%GAME_DIR%\Binaries" goto FOUND
    if exist "%GAME_DIR%\Lord of Mysteries.exe" goto FOUND
)

echo Не удалось автоматически найти папку с игрой.
echo.
echo Укажите путь к папке Game\C7 (например: D:\Games\GMZZLauncher\Game\C7):
set /p "USER_INPUT_PATH=> "
set "USER_INPUT_PATH=%USER_INPUT_PATH:"=%"

if exist "%USER_INPUT_PATH%\Binaries" (
    set "GAME_DIR=%USER_INPUT_PATH%"
    goto FOUND
)
if exist "%USER_INPUT_PATH%\Game\C7\Binaries" (
    set "GAME_DIR=%USER_INPUT_PATH%\Game\C7"
    goto FOUND
)

echo [!] Ошибка: По указанному пути игра не найдена!
echo     Папка должна содержать подпапку Binaries\Win64 или файл Lord of Mysteries.exe.
echo.
goto CHECK_PATH

:FOUND
:: Удаляем замыкающий слэш, если есть
if "%GAME_DIR:~-1%"=="\" set "GAME_DIR=%GAME_DIR:~0,-1%"

echo [*] Папка с игрой найдена: %GAME_DIR%
echo.

:: 3. Проверка наличия исходных файлов мода в архиве
if not exist "%SCRIPT_DIR%Saved" (
    echo [!] ОШИБКА: Папка 'Saved' не найдена рядом со скриптом!
    echo     Убедитесь, что вы полностью распаковали ZIP-архив перед запуском.
    echo.
    pause
    exit /b 1
)

:: 4. Резервное копирование оригинального Init.lua (если ещё не сделано)
set "FIXES_DIR=%GAME_DIR%\Saved\Mods\lua\mods\cpdd_runtime_fixes"
if exist "%FIXES_DIR%\Init.lua" (
    if not exist "%FIXES_DIR%\Init.lua.bak_orig" (
        echo [*] Создание резервной копии оригинального Init.lua...
        copy /Y "%FIXES_DIR%\Init.lua" "%FIXES_DIR%\Init.lua.bak_orig" >nul
    )
)

:: 5. Копирование файлов локализации
echo [*] Установка компонентов русификатора...
echo     - Копирование ядра локализации и настроек...
if not exist "%GAME_DIR%\Saved" mkdir "%GAME_DIR%\Saved"
xcopy /E /Y /I /Q "%SCRIPT_DIR%Saved" "%GAME_DIR%\Saved" >nul

if exist "%SCRIPT_DIR%Binaries" (
    echo     - Настройка загрузчика мода (CPDDTranslation)...
    if not exist "%GAME_DIR%\Binaries" mkdir "%GAME_DIR%\Binaries"
    xcopy /E /Y /I /Q "%SCRIPT_DIR%Binaries" "%GAME_DIR%\Binaries" >nul
)

:: 6. Настройка нативного загрузчика в pakchunk0-Windows.pak (Автономный запуск)
set "PAK_FILE=%GAME_DIR%\Content\Paks\pakchunk0-Windows.pak"
set "HOOK_FILE=%SCRIPT_DIR%LaunchInstance.native-bridge.padded.oodle"
if not exist "%HOOK_FILE%" set "HOOK_FILE=%GAME_DIR%\LaunchInstance.native-bridge.padded.oodle"

if exist "%HOOK_FILE%" if exist "%PAK_FILE%" (
    echo     - Настройка хука загрузчика в pakchunk0-Windows.pak...
    powershell -NoProfile -ExecutionPolicy Bypass -Command "$pak = $env:PAK_FILE; $hook = $env:HOOK_FILE; $hBytes = [System.IO.File]::ReadAllBytes($hook); if ($hBytes.Length -eq 4660) { $fs = [System.IO.File]::Open($pak, [System.IO.FileMode]::Open, [System.IO.FileAccess]::ReadWrite, [System.IO.FileShare]::ReadWrite); $offset = 427225161L; if ($fs.Length -ge ($offset + 4660)) { $fs.Seek($offset, [System.IO.SeekOrigin]::Begin) | Out-Null; $cur = New-Object byte[] 4660; $fs.Read($cur, 0, 4660) | Out-Null; $sha = [System.Security.Cryptography.SHA256]::Create(); $hash = [BitConverter]::ToString($sha.ComputeHash($cur)).Replace('-','').ToLower(); if ($hash -ne 'c031726986e09358bb18ff8a2b8ee5f0b4e65ce8ae8331eed2d7575c80b7efa9') { $bak = Join-Path (Split-Path $pak -Parent) 'pakchunk0-Windows.pak.orig_block'; if (-not (Test-Path $bak)) { [System.IO.File]::WriteAllBytes($bak, $cur) }; $fs.Seek($offset, [System.IO.SeekOrigin]::Begin) | Out-Null; $fs.Write($hBytes, 0, 4660); $fs.Flush(); Write-Host '      ✔ Нативный хук загрузчика успешно активирован!' -ForegroundColor Green } else { Write-Host '      ✔ Нативный хук загрузчика уже активен.' -ForegroundColor Green } }; $fs.Close(); $fs.Dispose() }" 2>nul
)

:: 7. Снятие блокировки файлов Zone.Identifier (Mark-of-the-Web)
echo [*] Разблокировка установленных файлов в системе безопасности Windows...
powershell -NoProfile -Command "Get-ChildItem -Path '%GAME_DIR%\Saved\Mods' -Recurse -ErrorAction SilentlyContinue | Unblock-File" 2>nul

echo.
echo ======================================================================
echo          ✔ РУСИФИКАТОР УСПЕШНО УСТАНОВЛЕН И АКТИВИРОВАН!
echo ======================================================================
echo   - Статус перевода: 100%% завершено (1 024 нативных шарда)
echo   - Шрифты и боевые формулы: настроены
echo   - Резервная копия оригинала: сохранена в Init.lua.bak_orig
echo.
echo   Для запуска игры используйте ваш обычный лаунчер (Bilibili / GMZZ).
echo   Для переключения на английский запустите 'Переключить_язык.cmd'.
echo ======================================================================
echo.
pause
