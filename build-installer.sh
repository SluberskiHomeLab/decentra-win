#!/bin/bash
# Build script for DecentraWin
# This script builds the application and creates distribution packages

set -e

# Configuration
CONFIGURATION="${1:-Release}"
VERSION="${2:-1.0.0}"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

info() {
    echo -e "${CYAN}$1${NC}"
}

success() {
    echo -e "${GREEN}$1${NC}"
}

error() {
    echo -e "${RED}$1${NC}"
}

# Get script directory
SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
ROOT_DIR="$SCRIPT_DIR"
PROJECT_DIR="$ROOT_DIR/DecentraWin"
OUTPUT_DIR="$ROOT_DIR/installer-output"
PUBLISH_DIR="$PROJECT_DIR/bin/$CONFIGURATION/net8.0-windows/win-x64/publish"

info "========================================="
info "  DecentraWin Build Script"
info "========================================="
info ""
info "Configuration: $CONFIGURATION"
info "Version: $VERSION"
info "Root Directory: $ROOT_DIR"
info ""

# Step 1: Clean previous builds
info "Step 1: Cleaning previous builds..."
if [ -d "$OUTPUT_DIR" ]; then
    rm -rf "$OUTPUT_DIR"
    success "  Cleaned output directory"
fi

dotnet clean "$PROJECT_DIR/DecentraWin.csproj" -c "$CONFIGURATION"
success "  Cleaned project"

# Step 2: Restore dependencies
info "Step 2: Restoring dependencies..."
dotnet restore "$PROJECT_DIR/DecentraWin.csproj"
success "  Dependencies restored"

# Step 3: Build the application
info "Step 3: Building application..."
dotnet build "$PROJECT_DIR/DecentraWin.csproj" -c "$CONFIGURATION"
success "  Build completed"

# Step 4: Publish as self-contained
info "Step 4: Publishing self-contained application..."
dotnet publish "$PROJECT_DIR/DecentraWin.csproj" \
    -c "$CONFIGURATION" \
    -r win-x64 \
    --self-contained true \
    -p:PublishSingleFile=false \
    -p:PublishReadyToRun=true \
    -p:IncludeNativeLibrariesForSelfExtract=true

success "  Application published to: $PUBLISH_DIR"

# Step 5: Verify publish output
if [ -f "$PUBLISH_DIR/DecentraWin.exe" ]; then
    FILE_SIZE=$(du -h "$PUBLISH_DIR/DecentraWin.exe" | cut -f1)
    success "  Executable found: DecentraWin.exe ($FILE_SIZE)"
else
    error "DecentraWin.exe not found in publish directory"
    exit 1
fi

# Step 6: Create ZIP archive
info "Step 6: Creating ZIP archive..."
ZIP_FILENAME="DecentraWin-v$VERSION-win-x64.zip"
ZIP_PATH="$OUTPUT_DIR/$ZIP_FILENAME"

# Create output directory if it doesn't exist
mkdir -p "$OUTPUT_DIR"

# Remove existing ZIP if it exists
if [ -f "$ZIP_PATH" ]; then
    rm -f "$ZIP_PATH"
fi

# Create ZIP archive
cd "$PUBLISH_DIR"
zip -r "$ZIP_PATH" ./*
cd "$ROOT_DIR"

FILE_SIZE=$(du -h "$ZIP_PATH" | cut -f1)
success "  ZIP archive created: $ZIP_FILENAME ($FILE_SIZE)"

info ""
success "========================================="
success "  Build completed successfully!"
success "========================================="
info ""
info "Output directory: $OUTPUT_DIR"
info ""
info "The ZIP archive is ready for distribution."
info ""
info "Note: To create a Windows installer (.exe), run build-installer.ps1"
info "      on a Windows machine with Inno Setup installed."
info ""
