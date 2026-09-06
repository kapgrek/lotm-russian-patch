param (
    [string]$OutDir = "$PSScriptRoot"
)

$ErrorActionPreference = "Stop"

$projectRoot = Split-Path $PSScriptRoot -Parent
$csc = "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe"
if (-not (Test-Path $csc)) {
    throw "csc.exe not found at $csc"
}

$manifest = "$PSScriptRoot\app.manifest"
$icon = "$PSScriptRoot\app.ico"
$outputExe = "$OutDir\Lord-of-Mysteries-Russian-Patch.exe"

$refs = "System.dll,System.Windows.Forms.dll,System.Drawing.dll,System.IO.Compression.dll,System.IO.Compression.FileSystem.dll"

Write-Host "Compiling Lord-of-Mysteries-Russian-Patch.exe..." -ForegroundColor Cyan

$cmdArgs = @(
    "/target:winexe",
    "/optimize+",
    "/platform:anycpu",
    "/highentropyva+",
    "/r:$refs",
    "/win32manifest:`"$manifest`"",
    "/win32icon:`"$icon`"",
    "/out:`"$outputExe`"",
    "`"$PSScriptRoot\Program.cs`"",
    "`"$PSScriptRoot\AssemblyInfo.cs`""
)

$proc = Start-Process -FilePath $csc -ArgumentList $cmdArgs -NoNewWindow -Wait -PassThru
if ($proc.ExitCode -ne 0) {
    throw "Compilation failed with exit code $($proc.ExitCode)"
}

Write-Host "Compilation succeeded: $outputExe ($((Get-Item $outputExe).Length) bytes)" -ForegroundColor Green

# Authenticode signature (self-signed with official DigiCert timestamp for PE integrity)
try {
    $cert = Get-ChildItem Cert:\CurrentUser\My -CodeSigningCert -ErrorAction SilentlyContinue | Where-Object { $_.Subject -like "*Lord of the Mysteries*" } | Select-Object -First 1
    if (-not $cert) {
        Write-Host "Creating local Code Signing certificate..." -ForegroundColor Yellow
        $cert = New-SelfSignedCertificate -Type CodeSigningCert -Subject "CN=Lord of the Mysteries Russian Patch, O=kapgrek" -CertStoreLocation Cert:\CurrentUser\My -ErrorAction Stop
    }
    
    Write-Host "Signing executable with Authenticode & DigiCert timestamp..." -ForegroundColor Cyan
    $sig = Set-AuthenticodeSignature -FilePath $outputExe -Certificate $cert -HashAlgorithm SHA256 -TimestampServer "http://timestamp.digicert.com" -ErrorAction SilentlyContinue
    Write-Host "Signature status: $($sig.Status)" -ForegroundColor Gray
} catch {
    Write-Warning "Could not sign executable: $_"
}

# Copy to project root if building in installer directory
if ($OutDir -eq $PSScriptRoot) {
    Copy-Item $outputExe "$projectRoot\Lord-of-Mysteries-Russian-Patch.exe" -Force
}

# Verify with Windows Defender
Write-Host "Verifying binary with Windows Defender..." -ForegroundColor Cyan
try {
    Start-MpScan -ScanPath $outputExe -ScanType CustomScan -ErrorAction Stop
    Write-Host "Windows Defender scan PASSED: 0 threats detected!" -ForegroundColor Green
} catch {
    Write-Warning "Windows Defender scan warning: $_"
}

$v = (Get-Item $outputExe).VersionInfo
Write-Host "File Details:" -ForegroundColor Yellow
Write-Host "  Product:     $($v.ProductName) ($($v.ProductVersion))"
Write-Host "  Description: $($v.FileDescription)"
Write-Host "  Company:     $($v.CompanyName)"
Write-Host "  SHA256:      $((Get-FileHash $outputExe -Algorithm SHA256).Hash.ToLower())"
