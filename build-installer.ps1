# Build script for DecentraWin installer
# This script builds the application and creates the installer package

param(
    [string]$Configuration = "Release",
    [string]$Version = "1.0.0",
    [switch]$SkipBuild = $false,
    [switch]$SkipInstaller = $false
)

$ErrorActionPreference = "Stop"

# Setup logging
$LogTimestamp = Get-Date -Format "yyyyMMdd-HHmmss"
$LogDir = Join-Path $PSScriptRoot "logs"
$LogFile = Join-Path $LogDir "build-installer-$LogTimestamp.log"

# Create logs directory if it doesn't exist
if (-not (Test-Path $LogDir)) {
    New-Item -ItemType Directory -Path $LogDir -Force | Out-Null
}

# Start transcript to capture all output
Start-Transcript -Path $LogFile

Write-Host "Logging to: $LogFile" -ForegroundColor Cyan

# Colors for output
function Write-Info { Write-Host $args -ForegroundColor Cyan }
function Write-Success { Write-Host $args -ForegroundColor Green }
function Write-Error { Write-Host $args -ForegroundColor Red }

# Get script directory
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$RootDir = Split-Path -Parent $ScriptDir
$ProjectDir = Join-Path $RootDir "DecentraWin"
$OutputDir = Join-Path $RootDir "installer-output"
$PublishDir = Join-Path $ProjectDir "bin\$Configuration\net8.0-windows\win-x64\publish"

try {
    Write-Info "========================================="
    Write-Info "  DecentraWin Installer Build Script"
    Write-Info "========================================="
    Write-Info ""
    Write-Info "Configuration: $Configuration"
    Write-Info "Version: $Version"
    Write-Info "Root Directory: $RootDir"
    Write-Info ""

# Step 1: Clean previous builds
if (-not $SkipBuild) {
    Write-Info "Step 1: Cleaning previous builds..."
    if (Test-Path $OutputDir) {
        Remove-Item -Path $OutputDir -Recurse -Force
        Write-Success "  Cleaned output directory"
    }
    
    dotnet clean "$ProjectDir\DecentraWin.csproj" -c $Configuration
    Write-Success "  Cleaned project"
}

# Step 2: Restore dependencies
if (-not $SkipBuild) {
    Write-Info "Step 2: Restoring dependencies..."
    dotnet restore "$ProjectDir\DecentraWin.csproj"
    if ($LASTEXITCODE -ne 0) {
        throw "Failed to restore dependencies"
    }
    Write-Success "  Dependencies restored"
}

# Step 3: Build the application
if (-not $SkipBuild) {
    Write-Info "Step 3: Building application..."
    dotnet build "$ProjectDir\DecentraWin.csproj" -c $Configuration
    if ($LASTEXITCODE -ne 0) {
        throw "Build failed"
    }
    Write-Success "  Build completed"
}

# Step 4: Publish as self-contained
if (-not $SkipBuild) {
    Write-Info "Step 4: Publishing self-contained application..."
    dotnet publish "$ProjectDir\DecentraWin.csproj" `
        -c $Configuration `
        -r win-x64 `
        --self-contained true `
        -p:PublishSingleFile=false `
        -p:PublishReadyToRun=true `
        -p:IncludeNativeLibrariesForSelfExtract=true
    
    if ($LASTEXITCODE -ne 0) {
        throw "Publish failed"
    }
    Write-Success "  Application published to: $PublishDir"
}

# Step 5: Verify publish output
if (Test-Path $PublishDir) {
    $ExePath = Join-Path $PublishDir "DecentraWin.exe"
    if (Test-Path $ExePath) {
        $FileInfo = Get-Item $ExePath
        Write-Success "  Executable found: DecentraWin.exe ($([math]::Round($FileInfo.Length / 1MB, 2)) MB)"
    } else {
        throw "DecentraWin.exe not found in publish directory"
    }
} else {
    throw "Publish directory not found: $PublishDir"
}

# Step 6: Create installer using Inno Setup (if available)
if (-not $SkipInstaller) {
    Write-Info "Step 5: Creating installer..."
    
    # Check for Inno Setup
    $InnoSetupPaths = @(
        "${env:ProgramFiles(x86)}\Inno Setup 6\ISCC.exe",
        "${env:ProgramFiles}\Inno Setup 6\ISCC.exe",
        "${env:ProgramFiles(x86)}\Inno Setup 5\ISCC.exe",
        "${env:ProgramFiles}\Inno Setup 5\ISCC.exe"
    )
    
    $InnoSetup = $null
    foreach ($path in $InnoSetupPaths) {
        if (Test-Path $path) {
            $InnoSetup = $path
            break
        }
    }
    
    if ($InnoSetup) {
        Write-Info "  Found Inno Setup: $InnoSetup"
        $IssFile = Join-Path $RootDir "installer\DecentraWin.iss"
        
        & $InnoSetup $IssFile
        if ($LASTEXITCODE -ne 0) {
            throw "Installer creation failed"
        }
        
        Write-Success "  Installer created successfully!"
        
        # List created installer
        $InstallerFiles = Get-ChildItem -Path $OutputDir -Filter "*.exe"
        foreach ($file in $InstallerFiles) {
            Write-Success "  Created: $($file.Name) ($([math]::Round($file.Length / 1MB, 2)) MB)"
        }
    } else {
        Write-Info "  Inno Setup not found. Skipping installer creation."
        Write-Info "  To create an installer, download Inno Setup from: https://jrsoftware.org/isinfo.php"
        Write-Info "  The published files are available at: $PublishDir"
    }
} else {
    Write-Info "Step 5: Skipping installer creation (--SkipInstaller specified)"
}

# Step 7: Create ZIP archive as alternative
Write-Info "Step 6: Creating ZIP archive..."
$ZipFileName = "DecentraWin-v$Version-win-x64.zip"
$ZipPath = Join-Path $OutputDir $ZipFileName

# Create output directory if it doesn't exist
if (-not (Test-Path $OutputDir)) {
    New-Item -ItemType Directory -Path $OutputDir | Out-Null
}

# Remove existing ZIP if it exists
if (Test-Path $ZipPath) {
    Remove-Item -Path $ZipPath -Force
}

# Create ZIP archive
Compress-Archive -Path "$PublishDir\*" -DestinationPath $ZipPath -CompressionLevel Optimal
Write-Success "  ZIP archive created: $ZipFileName ($([math]::Round((Get-Item $ZipPath).Length / 1MB, 2)) MB)"

Write-Info ""
Write-Success "========================================="
Write-Success "  Build completed successfully!"
Write-Success "========================================="
Write-Info ""
Write-Info "Output directory: $OutputDir"
Write-Info ""
if (-not $SkipInstaller -and $InnoSetup) {
    Write-Info "The installer and ZIP archive are ready for distribution."
} else {
    Write-Info "The ZIP archive is ready for distribution."
    Write-Info "To create an installer, install Inno Setup and run this script again."
}
Write-Info ""

} catch {
    Write-Host ""
    Write-Host "=========================================" -ForegroundColor Red
    Write-Host "  BUILD FAILED!" -ForegroundColor Red
    Write-Host "=========================================" -ForegroundColor Red
    Write-Host ""
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
    Write-Host "Stack Trace:" -ForegroundColor Yellow
    Write-Host $_.ScriptStackTrace -ForegroundColor Yellow
    Write-Host ""
    
    # Stop transcript
    Stop-Transcript
    
    Write-Host "=========================================" -ForegroundColor Red
    Write-Host "  Log file saved to:" -ForegroundColor Red
    Write-Host "  $LogFile" -ForegroundColor Cyan
    Write-Host "=========================================" -ForegroundColor Red
    Write-Host ""
    
    # Pause to prevent window from closing
    if ($Host.Name -eq "ConsoleHost") {
        Write-Host "Press any key to exit..." -ForegroundColor Yellow
        $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
    }
    
    exit 1
}

# Stop transcript and display log location
Stop-Transcript
Write-Host ""
Write-Host "=========================================" -ForegroundColor Green
Write-Host "  Log file saved to:" -ForegroundColor Green
Write-Host "  $LogFile" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Green
Write-Host ""

# Pause to prevent window from closing if script was double-clicked
if ($Host.Name -eq "ConsoleHost") {
    Write-Host "Press any key to exit..." -ForegroundColor Yellow
    $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
}
