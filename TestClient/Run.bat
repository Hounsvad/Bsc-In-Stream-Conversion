@echo off
:beginning
start "TestClientInstance" TestClient.exe
pause
timeout 60
goto beginning