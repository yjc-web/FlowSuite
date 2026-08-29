[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $scriptDir

Write-Host "========================================================" -ForegroundColor Cyan
Write-Host "  [OverlayPic] GitHub 원클릭 업로드 & 푸시" -ForegroundColor Cyan
Write-Host "========================================================`n" -ForegroundColor Cyan

# 1. Check Git Remote
$existingRemotes = git remote
$remoteUrl = $null

if ($existingRemotes -contains "origin") {
    $remoteUrl = git remote get-url origin
    Write-Host "원격 저장소(origin): $remoteUrl`n" -ForegroundColor Gray
} else {
    Write-Host "[안내] 원격 저장소(origin)가 설정되어 있지 않습니다." -ForegroundColor Yellow
    $defaultUrl = "https://github.com/yjc-web/FlowSuite.git"
    $inputUrl = Read-Host "GitHub 저장소 URL을 입력하세요 (엔터 시 기본값: $defaultUrl)"
    if ([string]::IsNullOrWhiteSpace($inputUrl)) {
        $remoteUrl = $defaultUrl
    } else {
        $remoteUrl = $inputUrl.Trim()
    }
    git remote add origin $remoteUrl
    Write-Host "원격 저장소 등록 완료: $remoteUrl`n" -ForegroundColor Green
}

# 2. Commit Message Input
$defaultCommitMsg = "Release OverlayPic v1.0.1 - Inno Setup installer and MSIX update"
$commitMsg = Read-Host "커밋 메시지를 입력하세요 (엔터 시 기본값: '$defaultCommitMsg')"
if ([string]::IsNullOrWhiteSpace($commitMsg)) {
    $commitMsg = $defaultCommitMsg
}

# 3. Git Staging & Commit
Write-Host "`n1. 변경 사항 스테이징 (git add .)..." -ForegroundColor Yellow
git add .

$status = git status --porcelain
if ($status) {
    Write-Host "2. 커밋 생성 중..." -ForegroundColor Yellow
    git commit -m "$commitMsg"
} else {
    Write-Host "커밋할 새로운 변경 사항이 없습니다." -ForegroundColor Gray
}

# 4. Push to Origin
Write-Host "`n3. GitHub로 푸시 중 (git push -u origin main)..." -ForegroundColor Yellow
git branch -M main
git push -u origin main
if ($LASTEXITCODE -ne 0) {
    Write-Host "`n[경고] 푸시 실패! 네트워크 연결 또는 GitHub 로그인/인증 권한을 확인하세요." -ForegroundColor Red
} else {
    Write-Host "`n[성공] 소스 코드가 GitHub에 성공적으로 푸시되었습니다!" -ForegroundColor Green
}

# 5. Tag & Release Option
$createTag = Read-Host "`nv1.0.1 릴리즈 태그도 함께 푸시하시겠습니까? (Y/n)"
if ($createTag -ne "n" -and $createTag -ne "N") {
    Write-Host "태그 생성 및 푸시 중 (v1.0.1)..." -ForegroundColor Yellow
    git tag -f "v1.0.1"
    git push origin "v1.0.1" --force
    Write-Host "태그 푸시 완료!" -ForegroundColor Green
}

# 6. Open Releases Page & dist folder for asset upload
$releaseUrl = "https://github.com/yjc-web/FlowSuite/releases/tag/OverlayPic"
$distDir = Join-Path $scriptDir "dist"

Write-Host "`n========================================================" -ForegroundColor Cyan
Write-Host "  [완료] GitHub 업로드 작업이 마무리되었습니다." -ForegroundColor Green
Write-Host "  * 배포 파일 위치: dist\" -ForegroundColor White
Write-Host "  * FlowSuite 릴리즈 URL: $releaseUrl" -ForegroundColor Cyan
Write-Host "========================================================`n" -ForegroundColor Cyan

$openBrowser = Read-Host "FlowSuite 릴리즈 페이지와 dist 폴더를 여시겠습니까? (Y/n)"
if ($openBrowser -ne "n" -and $openBrowser -ne "N") {
    if (Test-Path $distDir) { Start-Process $distDir }
    Start-Process $releaseUrl
}