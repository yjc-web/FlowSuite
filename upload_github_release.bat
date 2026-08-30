@echo off
title [OverlayPic] GitHub Release Auto Uploader
powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0upload_github_release.ps1"
echo.
pause
