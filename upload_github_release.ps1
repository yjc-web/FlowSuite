# ==============================================================================
# OverlayPic - Direct GitHub Release Binary Uploader
# Target: yjc-web/FlowSuite -> Tag: OverlayPic, Title: OverlayPic
# ==============================================================================
param(
    [string]$Owner = "yjc-web",
    [string]$Repo = "FlowSuite",
    [string]$Tag = "OverlayPic",
    [string]$ReleaseTitle = "OverlayPic"
)

$ErrorActionPreference = "Stop"
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $ScriptDir

Write-Host "============================================================" -ForegroundColor Cyan
Write-Host " [OverlayPic] GitHub Release Uploader" -ForegroundColor Cyan
Write-Host " Target: https://github.com/$Owner/$Repo/releases/tag/$Tag" -ForegroundColor Cyan
Write-Host "============================================================" -ForegroundColor Cyan

# 1. Prepare dist\OverlayPic_v1.0.3.exe
$distDir = Join-Path $ScriptDir "dist"
if (-not (Test-Path $distDir)) {
    New-Item -ItemType Directory -Path $distDir | Out-Null
}

$targetExe = Join-Path $distDir "OverlayPic_v1.0.3.exe"
if (-not (Test-Path $targetExe)) {
    $srcExe = "bin\Release\OverlayPic.exe"
    if (Test-Path $srcExe) {
        Copy-Item $srcExe $targetExe -Force
    } else {
        throw "Binary not found at '$srcExe'. Please build the project first!"
    }
}

Write-Host " Release Tag : $Tag" -ForegroundColor Green
Write-Host " Title       : $ReleaseTitle" -ForegroundColor Green
Write-Host " Asset File  : $targetExe ($([Math]::Round((Get-Item $targetExe).Length / 1KB, 2)) KB)" -ForegroundColor Gray

# 2. Get GitHub Token
$tokenFile = Join-Path $ScriptDir "Packaging\github_token.secret"
$token = $null
if (Test-Path $tokenFile) {
    $token = (Get-Content $tokenFile -Raw).Trim()
}
if ([string]::IsNullOrWhiteSpace($token) -and $env:GITHUB_TOKEN) {
    $token = $env:GITHUB_TOKEN.Trim()
}
if ([string]::IsNullOrWhiteSpace($token)) {
    $altTokenFile = Join-Path (Split-Path -Parent $ScriptDir) "Packaging\github_token.secret"
    if (Test-Path $altTokenFile) {
        $token = (Get-Content $altTokenFile -Raw).Trim()
    }
}

if ([string]::IsNullOrWhiteSpace($token)) {
    throw "GitHub Token is missing. Please check Packaging\github_token.secret"
}

$headers = @{
    "Authorization" = "Bearer $token"
    "Accept"        = "application/vnd.github+json"
    "User-Agent"    = "OverlayPic-Release-Uploader"
}

# 3. Connect to GitHub Repo
Write-Host "`n[1/3] Connecting to https://github.com/$Owner/$Repo..." -ForegroundColor Yellow
$repoCheckUrl = "https://api.github.com/repos/$Owner/$Repo"
try {
    $repoInfo = Invoke-RestMethod -Uri $repoCheckUrl -Method Get -Headers $headers
    Write-Host " -> Connected: $($repoInfo.full_name)" -ForegroundColor Green
} catch {
    throw "Cannot access repository '$Owner/$Repo'. Please verify token permissions."
}

# 4. Fetch Existing Release (Do NOT modify release body)
Write-Host "`n[2/3] Fetching Release '$Tag'..." -ForegroundColor Yellow
$release = $null

try {
    $existingUrl = "https://api.github.com/repos/$Owner/$Repo/releases/tags/$Tag"
    $release = Invoke-RestMethod -Uri $existingUrl -Method Get -Headers $headers
    Write-Host " -> Found existing release: '$($release.name)' (ID: $($release.id))" -ForegroundColor Green
    Write-Host " -> Keeping existing Release notes untouched." -ForegroundColor Gray
} catch {
    # If release doesn't exist yet, create it with title OverlayPic
    $releaseBody = @{
        tag_name   = $Tag
        name       = $ReleaseTitle
        draft      = $false
        prerelease = $false
    } | ConvertTo-Json

    $releaseUrl = "https://api.github.com/repos/$Owner/$Repo/releases"
    $release = Invoke-RestMethod -Uri $releaseUrl -Method Post -Headers $headers -Body $releaseBody -ContentType "application/json; charset=utf-8"
    Write-Host " -> Created release for '$Tag' (ID: $($release.id))" -ForegroundColor Green
}

# 5. Upload OverlayPic_v1.0.3.exe
Write-Host "`n[3/3] Uploading Asset 'OverlayPic_v1.0.3.exe'..." -ForegroundColor Yellow

Add-Type -AssemblyName System.Net.Http
$httpClient = New-Object System.Net.Http.HttpClient
$httpClient.Timeout = [TimeSpan]::FromMinutes(10)
$httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer $token")
$httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.github+json")
$httpClient.DefaultRequestHeaders.Add("User-Agent", "OverlayPic-Release-Uploader")

$fileName = "OverlayPic_v1.0.3.exe"
$assetsUrl = "https://api.github.com/repos/$Owner/$Repo/releases/$($release.id)/assets"

# Remove existing asset with the same name if present
try {
    $existingAssets = Invoke-RestMethod -Uri $assetsUrl -Method Get -Headers $headers
    foreach ($asset in $existingAssets) {
        if ($asset.name -eq $fileName) {
            Write-Host " -> Replacing existing asset '$fileName'..." -ForegroundColor Gray
            Invoke-RestMethod -Uri $asset.url -Method Delete -Headers $headers | Out-Null
        }
    }
} catch { }

# Upload new binary
$uploadUri = "https://uploads.github.com/repos/$Owner/$Repo/releases/$($release.id)/assets?name=$fileName"
$fileStream = [System.IO.File]::OpenRead($targetExe)
$content = New-Object System.Net.Http.StreamContent($fileStream)
$content.Headers.ContentType = New-Object System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream")

try {
    $response = $httpClient.PostAsync($uploadUri, $content).GetAwaiter().GetResult()
    $respBody = $response.Content.ReadAsStringAsync().GetAwaiter().GetResult()
    
    if ($response.IsSuccessStatusCode) {
        Write-Host " -> [OK] '$fileName' Uploaded Successfully!" -ForegroundColor Green
    } else {
        throw "Upload failed with status $($response.StatusCode): $respBody"
    }
} finally {
    $fileStream.Dispose()
    $content.Dispose()
    $httpClient.Dispose()
}

Write-Host "`n============================================================" -ForegroundColor Cyan
Write-Host " [SUCCESS] Asset uploaded to Tag '$Tag'!" -ForegroundColor Green
Write-Host " View Release: https://github.com/$Owner/$Repo/releases/tag/$Tag" -ForegroundColor Yellow
Write-Host "============================================================" -ForegroundColor Cyan