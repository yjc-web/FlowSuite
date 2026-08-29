@echo off
title [OverlayPic] GitHub Upload
powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0github_upload.ps1"
echo.
pause
