#Requires -Version 7.0
<#
.SYNOPSIS
    Development environment setup script for Windows.

.DESCRIPTION
    Sets up the development environment for McjCoderOrg.ClaudeAutoResume.
    Checks prerequisites, restores dependencies, configures git hooks, and verifies the build.

.EXAMPLE
    ./scripts/setup.ps1
#>

[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'

function Write-Status {
    param([string]$Message)
    Write-Host "[OK] $Message" -ForegroundColor Green
}

function Write-Checking {
    param([string]$Message)
    Write-Host "[..] $Message" -ForegroundColor Yellow -NoNewline
}

function Write-Error {
    param([string]$Message)
    Write-Host "[FAIL] $Message" -ForegroundColor Red
}

function Test-Command {
    param([string]$Command)
    $null -ne (Get-Command $Command -ErrorAction SilentlyContinue)
}

Write-Host ""
Write-Host "McjCoderOrg.ClaudeAutoResume - Development Setup" -ForegroundColor Cyan
Write-Host "=================================================" -ForegroundColor Cyan
Write-Host ""

# Check prerequisites
Write-Host "Checking prerequisites..." -ForegroundColor White
Write-Host ""

# Check .NET SDK
Write-Checking ".NET SDK 10.0+: "
if (Test-Command 'dotnet') {
    $dotnetVersion = dotnet --version 2>$null
    if ($dotnetVersion -match '^10\.') {
        Write-Host ""
        Write-Status ".NET SDK $dotnetVersion"
    } else {
        Write-Host ""
        Write-Error ".NET SDK 10.0+ required, found $dotnetVersion"
        Write-Host "  Install from: https://dotnet.microsoft.com/download/dotnet/10.0" -ForegroundColor Gray
        exit 1
    }
} else {
    Write-Host ""
    Write-Error ".NET SDK not found"
    Write-Host "  Install from: https://dotnet.microsoft.com/download/dotnet/10.0" -ForegroundColor Gray
    exit 1
}

# Check Node.js
Write-Checking "Node.js 22+: "
if (Test-Command 'node') {
    $nodeVersion = node --version 2>$null
    $nodeMajor = [int]($nodeVersion -replace 'v(\d+)\..*', '$1')
    if ($nodeMajor -ge 22) {
        Write-Host ""
        Write-Status "Node.js $nodeVersion"
    } else {
        Write-Host ""
        Write-Error "Node.js 22+ required, found $nodeVersion"
        Write-Host "  Install from: https://nodejs.org/" -ForegroundColor Gray
        exit 1
    }
} else {
    Write-Host ""
    Write-Error "Node.js not found"
    Write-Host "  Install from: https://nodejs.org/" -ForegroundColor Gray
    exit 1
}

# Check Git
Write-Checking "Git: "
if (Test-Command 'git') {
    $gitVersion = git --version 2>$null
    Write-Host ""
    Write-Status $gitVersion
} else {
    Write-Host ""
    Write-Error "Git not found"
    Write-Host "  Install from: https://git-scm.com/" -ForegroundColor Gray
    exit 1
}

# Check GitHub CLI (optional)
Write-Checking "GitHub CLI: "
if (Test-Command 'gh') {
    $ghVersion = gh --version 2>$null | Select-Object -First 1
    Write-Host ""
    Write-Status $ghVersion
} else {
    Write-Host ""
    Write-Host "[SKIP] GitHub CLI not found (optional)" -ForegroundColor DarkGray
    Write-Host "  Install from: https://cli.github.com/" -ForegroundColor Gray
}

Write-Host ""
Write-Host "Installing dependencies..." -ForegroundColor White
Write-Host ""

# Install npm dependencies
Write-Checking "npm install: "
npm install --silent 2>$null
if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Status "npm packages installed"
} else {
    Write-Host ""
    Write-Error "npm install failed"
    exit 1
}

# Restore .NET dependencies
Write-Checking "dotnet restore: "
dotnet restore --verbosity quiet 2>$null
if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Status ".NET packages restored"
} else {
    Write-Host ""
    Write-Error "dotnet restore failed"
    exit 1
}

# Restore .NET tools
Write-Checking "dotnet tool restore: "
dotnet tool restore --verbosity quiet 2>$null
if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Status ".NET tools restored"
} else {
    Write-Host ""
    Write-Error "dotnet tool restore failed"
    exit 1
}

Write-Host ""
Write-Host "Configuring git hooks..." -ForegroundColor White
Write-Host ""

# Configure git hooks via Husky
Write-Checking "Husky hooks: "
npx husky install 2>$null
if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Status "Git hooks configured"
} else {
    Write-Host ""
    Write-Host "[SKIP] Husky install skipped (may already be configured)" -ForegroundColor DarkGray
}

Write-Host ""
Write-Host "Verifying build..." -ForegroundColor White
Write-Host ""

# Build the solution
Write-Checking "dotnet build: "
dotnet build --verbosity quiet 2>$null
if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Status "Build successful"
} else {
    Write-Host ""
    Write-Error "Build failed"
    exit 1
}

# Run tests
Write-Checking "dotnet test: "
$testOutput = dotnet test --verbosity quiet --no-build 2>&1
if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Status "All tests passed"
} else {
    Write-Host ""
    Write-Error "Tests failed"
    Write-Host $testOutput -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "=================================================" -ForegroundColor Cyan
Write-Host "Setup complete!" -ForegroundColor Green
Write-Host ""
Write-Host "Next steps:" -ForegroundColor White
Write-Host "  1. Open the project in VS Code or Visual Studio" -ForegroundColor Gray
Write-Host "  2. Read CONTRIBUTING.md for contribution guidelines" -ForegroundColor Gray
Write-Host "  3. Check docs/ for project documentation" -ForegroundColor Gray
Write-Host ""
