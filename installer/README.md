# DecentraWin Installer

This directory contains the Inno Setup script for creating a Windows installer for DecentraWin.

## Prerequisites

To build the installer, you need:

1. **Windows OS** - The installer can only be built on Windows
2. **Inno Setup 6** - Download from [https://jrsoftware.org/isinfo.php](https://jrsoftware.org/isinfo.php)
3. **.NET 8.0 SDK** - For building the application

## Building the Installer

### Option 1: Using the PowerShell Script (Recommended)

Run the build script from the repository root:

```powershell
.\build-installer.ps1
```

This will:
1. Clean previous builds
2. Restore dependencies
3. Build the application in Release mode
4. Publish as a self-contained Windows x64 application
5. Create an installer using Inno Setup (if available)
6. Create a ZIP archive for manual distribution

#### Script Options

```powershell
# Specify version
.\build-installer.ps1 -Version "1.0.1"

# Skip the build process (use existing publish folder)
.\build-installer.ps1 -SkipBuild

# Skip installer creation (only build and create ZIP)
.\build-installer.ps1 -SkipInstaller
```

### Option 2: Using the Bash Script (Linux/macOS)

You can build the application and create a ZIP archive on Linux/macOS:

```bash
./build-installer.sh
```

This creates a ZIP archive but not the Windows installer (requires Inno Setup on Windows).

### Option 3: Manual Build

1. **Publish the application:**
   ```bash
   dotnet publish DecentraWin/DecentraWin.csproj -c Release -r win-x64 --self-contained true
   ```

2. **Create the installer:**
   - Open Inno Setup
   - Load `installer/DecentraWin.iss`
   - Click "Compile" or press Ctrl+F9

3. **Find the installer:**
   - The installer will be created in `installer-output/`
   - Filename: `DecentraWin-Setup-v1.0.0.exe`

## Installer Features

The generated installer includes:

- **Self-contained deployment** - No .NET runtime installation required
- **Start Menu shortcut** - Automatically created
- **Desktop shortcut** - Optional (unchecked by default)
- **Uninstaller** - Standard Windows uninstallation
- **License agreement** - Shows Apache 2.0 license
- **Modern wizard UI** - Professional installation experience

## Customizing the Installer

Edit `DecentraWin.iss` to customize:

- **Version number** - Update `#define MyAppVersion`
- **Application name** - Update `#define MyAppName`
- **Company info** - Update `#define MyAppPublisher`
- **Icon** - Set `SetupIconFile` to a custom .ico file
- **Default install location** - Modify `DefaultDirName`
- **Desktop shortcut default** - Change `Flags` on the desktopicon task

## Output Files

After a successful build, you'll find in `installer-output/`:

1. **DecentraWin-Setup-v{version}.exe** - Windows installer (if Inno Setup is available)
2. **DecentraWin-v{version}-win-x64.zip** - ZIP archive for manual installation

## Distribution

### Installer (.exe)
Best for end users who want a traditional Windows installation experience.

**Advantages:**
- Professional installation wizard
- Automatic Start Menu shortcuts
- Easy uninstallation via Windows Settings
- No manual extraction required

### ZIP Archive (.zip)
Best for portable installations or when installer privileges are not available.

**Advantages:**
- No installation required
- Portable - can run from any folder
- No admin privileges needed
- Easy to move or backup

## Troubleshooting

### "Inno Setup not found"
- Download and install Inno Setup 6 from [https://jrsoftware.org/isinfo.php](https://jrsoftware.org/isinfo.php)
- Ensure it's installed in the default location or update the script paths

### "Publish directory not found"
- Ensure the application builds successfully first
- Check that .NET 8.0 SDK is installed
- Try running with `-SkipBuild` flag removed

### "Access denied" errors
- Run PowerShell as Administrator
- Check antivirus isn't blocking the build process
- Ensure you have write permissions to the repository folder

## Signing the Installer (Optional)

For production releases, you should sign the installer with a code signing certificate:

1. Obtain a code signing certificate
2. Update the Inno Setup script to include signing:
   ```ini
   [Setup]
   SignTool=signtool
   ```
3. Configure SignTool in Inno Setup's Tools menu

## Version Management

When releasing a new version:

1. Update version in `DecentraWin/DecentraWin.csproj`:
   ```xml
   <Version>1.0.1</Version>
   <AssemblyVersion>1.0.1.0</AssemblyVersion>
   <FileVersion>1.0.1.0</FileVersion>
   ```

2. Update version in `installer/DecentraWin.iss`:
   ```ini
   #define MyAppVersion "1.0.1"
   ```

3. Run the build script:
   ```powershell
   .\build-installer.ps1 -Version "1.0.1"
   ```

Or let the build script read from the project file (future enhancement).
