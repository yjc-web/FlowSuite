@echo off
title [OverlayPic] Build All Packages
powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0build_all.ps1"
echo.
pause
