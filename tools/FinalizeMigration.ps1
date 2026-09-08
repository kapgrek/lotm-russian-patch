# =====================================================================
# Lord of the Mysteries - Migration to Canonical Directory v2.0.0
# =====================================================================
$ErrorActionPreference = "Stop"

Write-Host "=====================================================================" -ForegroundColor Cyan
Write-Host " Lord of the Mysteries - Migration to Canonical Directory v2.0.0" -ForegroundColor Cyan
Write-Host "=====================================================================" -ForegroundColor Cyan

Set-Location "D:\gameDev"

if (Test-Path "D:\gameDev\translate lotm") {
    Write-Host "[1/2] Backing up legacy 'translate lotm' -> 'translate_lotm_legacy_backup'..." -ForegroundColor Yellow
    Rename-Item -Path "D:\gameDev\translate lotm" -NewName "translate_lotm_legacy_backup"
    Write-Host "      Done." -ForegroundColor Green
} else {
    Write-Host "[1/2] Legacy folder 'translate lotm' not found, skipping." -ForegroundColor Gray
}

if (Test-Path "D:\gameDev\NewBild") {
    Write-Host "[2/2] Renaming clean 'NewBild' (Clean Architecture 2.0) -> 'translate lotm'..." -ForegroundColor Yellow
    Rename-Item -Path "D:\gameDev\NewBild" -NewName "translate lotm"
    Write-Host "      Done." -ForegroundColor Green
} else {
    Write-Host "[ERROR] Folder D:\gameDev\NewBild not found!" -ForegroundColor Red
}

Write-Host "`n=====================================================================" -ForegroundColor Cyan
Write-Host " MIGRATION SUCCESSFUL!" -ForegroundColor Green
Write-Host " Canonical repository: D:\gameDev\translate lotm" -ForegroundColor White
Write-Host " Legacy backup:        D:\gameDev\translate_lotm_legacy_backup" -ForegroundColor White
Write-Host "=====================================================================" -ForegroundColor Cyan