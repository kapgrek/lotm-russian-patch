param (
    [string]$Version = "v1.1.0"
)

Add-Type -AssemblyName System.IO.Compression.FileSystem

$buildDir = "C:\Users\yapug\.gemini\antigravity\scratch\lotm-russian-patch\build"
if (Test-Path $buildDir) { Remove-Item $buildDir -Recurse -Force }
New-Item -ItemType Directory -Path $buildDir -Force | Out-Null

$staging = "$buildDir\staging"
New-Item -ItemType Directory -Path "$staging\Saved\Mods\lua\mods\cpdd_runtime_fixes" -Force | Out-Null
New-Item -ItemType Directory -Path "$staging\Binaries\Win64\lua\Launch\Base" -Force | Out-Null

$gameDir = "D:\Games\GMZZLauncher\Game\C7"

Write-Host "Копирование файлов мода..."
Copy-Item "$gameDir\Saved\Mods\bootstrap.lua" "$staging\Saved\Mods\" -Force
Copy-Item "$gameDir\Saved\Mods\manifest.lua" "$staging\Saved\Mods\" -Force
Copy-Item "$gameDir\Saved\Mods\translation-overrides.lua" "$staging\Saved\Mods\" -Force
Copy-Item "$gameDir\Saved\Mods\lua\mods\cpdd_runtime_fixes\*" "$staging\Saved\Mods\lua\mods\cpdd_runtime_fixes\" -Recurse -Force
Copy-Item "$gameDir\Binaries\Win64\lua\Launch\Base\CPDDTranslation.lua" "$staging\Binaries\Win64\lua\Launch\Base\" -Force

$zipPath = "$buildDir\lom-russian-patch-data.zip"
Write-Host "Создание архива $zipPath..."
[System.IO.Compression.ZipFile]::CreateFromDirectory($staging, $zipPath, [System.IO.Compression.CompressionLevel]::Optimal, $false)

# Удаляем временную папку staging
Remove-Item $staging -Recurse -Force

# Хэш архива
$zipHash = (Get-FileHash $zipPath -Algorithm SHA256).Hash.ToLower()
$zipSize = (Get-Item $zipPath).Length

# Копируем установщик в папку build
$installerSrc = "C:\Users\yapug\Downloads\lotm translate\Lord-of-Mysteries-Russian-Patch.exe"
$installerDest = "$buildDir\Lord-of-Mysteries-Russian-Patch.exe"
Copy-Item $installerSrc $installerDest -Force
$exeHash = (Get-FileHash $installerDest -Algorithm SHA256).Hash.ToLower()
$exeSize = (Get-Item $installerDest).Length

# Генерируем release.json
$releaseInfo = @{
    release_version = $Version
    release_tag = $Version
    format_version = 2
    patcher_asset = @{
        name = "Lord-of-Mysteries-Russian-Patch.exe"
        sha256 = $exeHash
        size = $exeSize
    }
    payload = @{
        name = "lom-russian-patch-data.zip"
        sha256 = $zipHash
        size = $zipSize
        url = "https://github.com/kapgrek/lotm-russian-patch/releases/download/$Version/lom-russian-patch-data.zip"
    }
}

$jsonContent = $releaseInfo | ConvertTo-Json -Depth 5
[System.IO.File]::WriteAllText("$buildDir\release.json", $jsonContent, [System.Text.Encoding]::UTF8)

Write-Host "Релизный пакет успешно собран в $buildDir!" -ForegroundColor Green
Write-Host "  Архив данных: $zipPath ($([Math]::Round($zipSize/1MB, 2)) MB)"
Write-Host "  Установщик: $installerDest"
Write-Host "  Манифест: $buildDir\release.json"