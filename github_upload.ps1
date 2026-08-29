$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $scriptDir

Write-Host "========================================================" -ForegroundColor Cyan
Write-Host "  [OverlayPic] GitHub Commit & Push" -ForegroundColor Cyan
Write-Host "========================================================`n" -ForegroundColor Cyan

# 1. Check Git Remote
$remotes = git remote
$targetUrl = "https://github.com/yjc-web/OverlayPic.git"

if ($remotes -contains "origin") {
    $currentUrl = git remote get-url origin
    Write-Host "Current remote origin: $currentUrl" -ForegroundColor Gray
} else {
    Write-Host "Adding remote origin: $targetUrl" -ForegroundColor Yellow
    git remote add origin $targetUrl
}

# 2. Input Commit Message
$defaultMsg = "Release v1.0.1 - Add Inno Setup installer and MSIX update"
$userMsg = Read-Host "Enter commit message (Press Enter for default: '$defaultMsg')"
if ([string]::IsNullOrWhiteSpace($userMsg)) {
    $commitMsg = $defaultMsg
} else {
    $commitMsg = $userMsg.Trim()
}

# 3. Stage & Commit
Write-Host "`n[1/3] Staging files (git add .)..." -ForegroundColor Yellow
git add .

Write-Host "[2/3] Committing changes..." -ForegroundColor Yellow
git commit -m "$commitMsg"

# 4. Push
Write-Host "[3/3] Pushing to GitHub (git push -u origin main)..." -ForegroundColor Yellow
git branch -M main
git push -u origin main

if ($LASTEXITCODE -eq 0) {
    Write-Host "`n[SUCCESS] Code pushed to GitHub successfully!" -ForegroundColor Green
} else {
    Write-Host "`n[WARNING] Git push encountered an issue. Check network/credentials." -ForegroundColor Red
}

# 5. Tag
$tagChoice = Read-Host "`nPush version tag v1.0.1? (Y/n)"
if ($tagChoice -ne "n" -and $tagChoice -ne "N") {
    git tag -f "v1.0.1"
    git push origin "v1.0.1" --force
    Write-Host "Tag v1.0.1 pushed." -ForegroundColor Green
}

# 6. Open Releases URL & dist directory
$releaseUrl = "https://github.com/yjc-web/FlowSuite/releases/tag/OverlayPic"
$distDir = Join-Path $scriptDir "dist"

Write-Host "`n========================================================" -ForegroundColor Cyan
Write-Host "  [DONE] Completed!" -ForegroundColor Green
Write-Host "  * Output Directory : $distDir" -ForegroundColor White
Write-Host "  * Release Page URL : $releaseUrl" -ForegroundColor Cyan
Write-Host "========================================================`n" -ForegroundColor Cyan

$openChoice = Read-Host "Open Release Page and dist folder in Explorer? (Y/n)"
if ($openChoice -ne "n" -and $openChoice -ne "N") {
    if (Test-Path $distDir) { Start-Process $distDir }
    Start-Process $releaseUrl
}