@echo off
title [OverlayPic] GitHub Upload & Push
powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0github_upload.ps1"
echo.
pause
