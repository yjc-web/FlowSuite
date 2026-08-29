@echo off
title [OverlayPic] Build Store MSIX
powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0build_store_msix.ps1"
echo.
pause
