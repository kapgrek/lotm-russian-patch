param (
    [string]$Version = "v1.33.1"
)

Add-Type -AssemblyName System.IO.Compression.FileSystem

$projectRoot = $PSScriptRoot
$buildDir = "$projectRoot\build"
if (Test-Path $buildDir) { Remove-Item $buildDir -Recurse -Force }
New-Item -ItemType Directory -Path $buildDir -Force | Out-Null

$staging = "$buildDir\staging"
New-Item -ItemType Directory -Path "$staging\Saved\Mods\lua\mods\cpdd_runtime_fixes" -Force | Out-Null
New-Item -ItemType Directory -Path "$staging\Binaries\Win64\lua\Launch\Base" -Force | Out-Null

$gameDir = "D:\Games\GMZZLauncher\Game\C7"

Write-Host "Copying mod files..."
# Сначала базовый шаблон из mod_base
if (Test-Path "$projectRoot\mod_base\Saved\Mods") {
    Write-Host "Using mod_base templates..." -ForegroundColor Cyan
    Copy-Item "$projectRoot\mod_base\Saved\Mods\*" "$staging\Saved\Mods\" -Recurse -Force
    Copy-Item "$projectRoot\mod_base\Binaries\Win64\lua\Launch\Base\*" "$staging\Binaries\Win64\lua\Launch\Base\" -Recurse -Force
}

# Если есть живая папка игры, дополняем файлы
if (Test-Path "$gameDir\Saved\Mods") {
    if (Test-Path "$gameDir\Saved\Mods\translation-overrides.state.json") {
        Copy-Item "$gameDir\Saved\Mods\translation-overrides.state.json" "$staging\Saved\Mods\" -Force
    }
    if (Test-Path "$gameDir\Saved\Mods\lua\cpdd_translation") {
        Copy-Item "$gameDir\Saved\Mods\lua\cpdd_translation" "$staging\Saved\Mods\lua\" -Recurse -Force
    }
}

# Гарантия: файлы русификатора из data/ имеют абсолютный приоритет
Copy-Item "$projectRoot\data\Init.lua" "$staging\Saved\Mods\lua\mods\cpdd_runtime_fixes\Init.lua" -Force
Copy-Item "$projectRoot\data\RussianLocalization.lua" "$staging\Saved\Mods\lua\mods\cpdd_runtime_fixes\RussianLocalization.lua" -Force
Copy-Item "$projectRoot\data\RuntimeTextRussian.lua" "$staging\Saved\Mods\lua\mods\cpdd_runtime_fixes\RuntimeTextRussian.lua" -Force
if (Test-Path "$projectRoot\data\EnglishToRussian.lua") {
    Copy-Item "$projectRoot\data\EnglishToRussian.lua" "$staging\Saved\Mods\lua\mods\cpdd_runtime_fixes\EnglishToRussian.lua" -Force
}
if (Test-Path "$projectRoot\data\CPDDTranslation.lua") {
    Copy-Item "$projectRoot\data\CPDDTranslation.lua" "$staging\Binaries\Win64\lua\Launch\Base\CPDDTranslation.lua" -Force
}
if (Test-Path "$projectRoot\data\LaunchInstance.native-bridge.padded.oodle") {
    Copy-Item "$projectRoot\data\LaunchInstance.native-bridge.padded.oodle" "$staging\LaunchInstance.native-bridge.padded.oodle" -Force
}
if (Test-Path "$projectRoot\data\shards") {
    Write-Host "Copying Russian translation shards (1,024 shards)..." -ForegroundColor Green
    Copy-Item "$projectRoot\data\shards\RuntimeTextGemini_*.lua" "$staging\Saved\Mods\lua\mods\cpdd_runtime_fixes\" -Force
}

if (Test-Path "$projectRoot\tools\PakHook.cs") {
    Write-Host "Compiling standalone PakHook.exe..." -ForegroundColor Cyan
    & "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe" /nologo /optimize+ /out:"$projectRoot\installer\standalone\PakHook.exe" "$projectRoot\tools\PakHook.cs"
}

if (Test-Path "$projectRoot\installer\standalone") {
    Write-Host "Adding standalone scripts (Install.cmd, Uninstall.cmd, PakHook.exe, Readme)..." -ForegroundColor Cyan
    Copy-Item "$projectRoot\installer\standalone\*" "$staging\" -Force
}

$zipPath = "$buildDir\lom-russian-patch-data.zip"
Write-Host "Creating primary archive $zipPath..."
[System.IO.Compression.ZipFile]::CreateFromDirectory($staging, $zipPath, [System.IO.Compression.CompressionLevel]::Optimal, $false, [System.Text.Encoding]::UTF8)

$standaloneZip = "$buildDir\lom-russian-patch-$Version.zip"
Write-Host "Creating standalone release archive $standaloneZip..."
Copy-Item $zipPath $standaloneZip -Force

Remove-Item $staging -Recurse -Force

$zipHash = (Get-FileHash $zipPath -Algorithm SHA256).Hash.ToLower()
$zipSize = (Get-Item $zipPath).Length

Write-Host "Building clean GUI installer with manifest and metadata..." -ForegroundColor Cyan
& "$projectRoot\installer\build_installer.ps1" -OutDir "$buildDir"

$installerDest = "$buildDir\Lord-of-Mysteries-Russian-Patch.exe"
Copy-Item $installerDest "$projectRoot\Lord-of-Mysteries-Russian-Patch.exe" -Force
$exeHash = (Get-FileHash $installerDest -Algorithm SHA256).Hash.ToLower()
$exeSize = (Get-Item $installerDest).Length

# Generate release.json
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
    standalone = @{
        name = "lom-russian-patch-$Version.zip"
        sha256 = $zipHash
        size = $zipSize
        url = "https://github.com/kapgrek/lotm-russian-patch/releases/download/$Version/lom-russian-patch-$Version.zip"
    }
}

$jsonContent = $releaseInfo | ConvertTo-Json -Depth 5
[System.IO.File]::WriteAllText("$buildDir\release.json", $jsonContent, [System.Text.Encoding]::UTF8)

Write-Host "Release package successfully built in $buildDir!" -ForegroundColor Green
Write-Host "  Data Archive:       $zipPath ($([Math]::Round($zipSize/1MB, 2)) MB)"
Write-Host "  Standalone Archive: $standaloneZip ($([Math]::Round($zipSize/1MB, 2)) MB)"
Write-Host "  GUI Installer:      $installerDest"
Write-Host "  Manifest:           $buildDir\release.json"
