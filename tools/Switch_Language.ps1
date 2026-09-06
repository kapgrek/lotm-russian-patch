[Console]::OutputEncoding = [System.Text.Encoding]::UTF8

$gameMods = "D:\Games\GMZZLauncher\Game\C7\Saved\Mods"
$russianFile = "$gameMods\lua\mods\cpdd_runtime_fixes\RussianLocalization.lua"

Clear-Host
Write-Host "=====================================================" -ForegroundColor Cyan
Write-Host "    Lord of the Mysteries - Переключатель Языка     " -ForegroundColor Yellow
Write-Host "=====================================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "1. Включить РУССКИЙ язык (Русификатор)" -ForegroundColor Green
Write-Host "2. Включить АНГЛИЙСКИЙ язык (English Patch)" -ForegroundColor Yellow
Write-Host "3. Восстановить оригинальный файл Init.lua (Backup)" -ForegroundColor Magenta
Write-Host "4. Выход" -ForegroundColor Gray
Write-Host ""

$choice = Read-Host "Выберите действие (1-4)"

switch ($choice) {
    "1" {
        if (Test-Path $russianFile) {
            $content = [System.IO.File]::ReadAllText($russianFile, [System.Text.Encoding]::UTF8)
            $content = $content -replace "Russian\.Enabled\s*=\s*false", "Russian.Enabled = true"
            $content = $content -replace "Enabled\s*=\s*false", "Enabled = true"
            [System.IO.File]::WriteAllText($russianFile, $content, (New-Object System.Text.UTF8Encoding($false)))

            $gameInit = "$gameMods\lua\mods\cpdd_runtime_fixes\Init.lua"
            $dataInit = "$PSScriptRoot\..\data\Init.lua"
            if (Test-Path $gameInit) {
                $initText = [System.IO.File]::ReadAllText($gameInit, [System.Text.Encoding]::UTF8)
                if (-not $initText.Contains("RussianLocalization") -and (Test-Path $dataInit)) {
                    Copy-Item $dataInit $gameInit -Force
                    Write-Host "[OK] Хуки в Init.lua восстановлены!" -ForegroundColor Cyan
                }
            }
            Write-Host "`n[OK] Русификатор успешно ВКЛЮЧЕН!" -ForegroundColor Green
        } else {
            Write-Host "`n[ОШИБКА] Файл RussianLocalization.lua не найден!" -ForegroundColor Red
        }
    }
    "2" {
        if (Test-Path $russianFile) {
            $content = [System.IO.File]::ReadAllText($russianFile, [System.Text.Encoding]::UTF8)
            $content = $content -replace "Russian\.Enabled\s*=\s*true", "Russian.Enabled = false"
            $content = $content -replace "Enabled\s*=\s*true", "Enabled = false"
            [System.IO.File]::WriteAllText($russianFile, $content, (New-Object System.Text.UTF8Encoding($false)))
            Write-Host "`n[OK] Перевод переключен на АНГЛИЙСКИЙ!" -ForegroundColor Yellow
        } else {
            Write-Host "`n[ОШИБКА] Файл RussianLocalization.lua не найден!" -ForegroundColor Red
        }
    }
    "3" {
        $bak = "$gameMods\lua\mods\cpdd_runtime_fixes\Init.lua.bak_orig"
        $init = "$gameMods\lua\mods\cpdd_runtime_fixes\Init.lua"
        if (Test-Path $bak) {
            Copy-Item $bak $init -Force
            Write-Host "`n[OK] Init.lua успешно восстановлен из резервной копии!" -ForegroundColor Green
        } else {
            Write-Host "`n[ОШИБКА] Файл бэкапа не найден!" -ForegroundColor Red
        }
    }
    Default {
        Write-Host "`nВыход без изменений." -ForegroundColor Gray
    }
}

Write-Host ""
pause
