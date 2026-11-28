@echo off
cd /d "%~dp0"

echo [1/4] Verificando Docker...
docker info >nul 2>&1
if %errorlevel% neq 0 (
    echo Iniciando Docker Desktop...
    start "" "C:\Users\mario\AppData\Local\Programs\DockerDesktop\Docker Desktop.exe"
    echo Esperando a que el motor de Docker responda...
    :wait_docker
    timeout /t 3 /nobreak >nul
    docker info >nul 2>&1
    if %errorlevel% neq 0 goto wait_docker
)

echo [2/4] Levantando contenedor de PostgreSQL...
docker compose up -d

echo [3/4] Iniciando Backend (.NET)...
start "Backend - Book API" cmd /k "cd backend && dotnet run"

echo [4/4] Iniciando Frontend (Vue + Vite)...
start "Frontend - Book App" cmd /k "cd frontend && pnpm dev"

echo Esperando servidores...
timeout /t 5 /nobreak >nul

echo Abriendo navegador en la app...
start http://localhost:5173/login
