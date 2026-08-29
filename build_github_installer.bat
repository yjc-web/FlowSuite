@echo off
title [OverlayPic] Build GitHub Installer
powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0build_github_installer.ps1"
echo.
pause
