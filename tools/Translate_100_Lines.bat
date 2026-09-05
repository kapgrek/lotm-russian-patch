@echo off
chcp 65001 > nul
powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0Batch_Translator.ps1" -BatchCount 100
pause
