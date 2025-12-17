# Contributing to DecentraWin

Thank you for your interest in contributing to DecentraWin! This document provides guidelines and instructions for contributing to the project.

## Table of Contents

- [Development Setup](#development-setup)
- [Building the Application](#building-the-application)
- [Creating Installers](#creating-installers)
- [Code Style](#code-style)
- [Testing](#testing)
- [Submitting Changes](#submitting-changes)

## Development Setup

### Prerequisites

- **Windows 10 or 11** - Required for WPF development
- **.NET 8.0 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Visual Studio 2022** (recommended) or Visual Studio Code
- **Git** - For version control

### Getting Started

1. **Fork the repository**
   ```bash
   # Click the "Fork" button on GitHub
   ```

2. **Clone your fork**
   ```bash
   git clone https://github.com/YOUR-USERNAME/decentra-win.git
   cd decentra-win
   ```

3. **Add upstream remote**
   ```bash
   git remote add upstream https://github.com/SluberskiHomeLab/decentra-win.git
   ```

4. **Install dependencies**
   ```bash
   dotnet restore
   ```

5. **Build the project**
   ```bash
   dotnet build
   ```

6. **Run the application**
   ```bash
   dotnet run --project DecentraWin
   ```

## Building the Application

### Debug Build

For development and debugging:

```bash
dotnet build -c Debug
dotnet run --project DecentraWin
```

### Release Build

For testing performance and optimizations:

```bash
dotnet build -c Release
```

### Published Build

To create a standalone executable:

```bash
dotnet publish DecentraWin/DecentraWin.csproj -c Release -r win-x64 --self-contained true
```

The output will be in `DecentraWin/bin/Release/net8.0-windows/win-x64/publish/`

## Creating Installers

### Using the Build Scripts

**On Windows (PowerShell):**
```powershell
.\build-installer.ps1
```

**On Linux/macOS (Bash):**
```bash
./build-installer.sh
```

### Manual Installer Creation

1. **Install Inno Setup**
   - Download from [https://jrsoftware.org/isinfo.php](https://jrsoftware.org/isinfo.php)
   - Install using default options

2. **Publish the application**
   ```bash
   dotnet publish DecentraWin/DecentraWin.csproj -c Release -r win-x64 --self-contained true
   ```

3. **Compile the installer**
   - Open Inno Setup
   - Load `installer/DecentraWin.iss`
   - Click "Compile" (or press Ctrl+F9)

The installer will be created in `installer-output/DecentraWin-Setup-v1.0.0.exe`

## Code Style

### C# Coding Conventions

- Follow [Microsoft's C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- Use meaningful variable and method names
- Keep methods focused and under 50 lines when possible
- Use async/await for asynchronous operations
- Add XML documentation comments for public APIs

### XAML Guidelines

- Use data binding instead of code-behind when possible
- Follow MVVM pattern
- Keep view code-behind minimal (only UI-specific logic)
- Use styles and resources for consistent UI

### Example

```csharp
/// <summary>
/// Sends a message to the specified channel.
/// </summary>
/// <param name="channelId">The ID of the channel.</param>
/// <param name="message">The message to send.</param>
/// <returns>True if the message was sent successfully.</returns>
public async Task<bool> SendMessageAsync(string channelId, string message)
{
    if (string.IsNullOrWhiteSpace(message))
        return false;
        
    try
    {
        await _webSocketService.SendAsync(new MessageData
        {
            ChannelId = channelId,
            Content = message
        });
        return true;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Failed to send message");
        return false;
    }
}
```

## Testing

### Running Tests

```bash
dotnet test
```

### Writing Tests

- Add tests for new features
- Ensure tests are independent and can run in any order
- Use meaningful test names that describe the scenario
- Follow the Arrange-Act-Assert pattern

Example:
```csharp
[Fact]
public async Task SendMessageAsync_WithValidMessage_ReturnsTrue()
{
    // Arrange
    var service = new MessageService();
    var message = "Hello, world!";
    
    // Act
    var result = await service.SendMessageAsync("channel-123", message);
    
    // Assert
    Assert.True(result);
}
```

## Submitting Changes

### Creating a Feature Branch

```bash
# Update your fork
git checkout main
git pull upstream main

# Create a new branch
git checkout -b feature/your-feature-name
```

### Making Changes

1. Make your changes in your feature branch
2. Test your changes thoroughly
3. Commit your changes with clear messages

```bash
git add .
git commit -m "Add feature: description of your changes"
```

### Commit Message Guidelines

- Use the present tense ("Add feature" not "Added feature")
- Use the imperative mood ("Move cursor to..." not "Moves cursor to...")
- Limit the first line to 72 characters or less
- Reference issues and pull requests liberally after the first line

Examples:
```
Add voice chat feature for direct messages

- Implement WebRTC connection for 1-on-1 calls
- Add UI controls for call management
- Update documentation

Fixes #123
```

### Pushing Changes

```bash
git push origin feature/your-feature-name
```

### Creating a Pull Request

1. Go to your fork on GitHub
2. Click "New Pull Request"
3. Select your feature branch
4. Fill in the PR template with:
   - Description of changes
   - Related issues
   - Testing performed
   - Screenshots (for UI changes)
5. Submit the pull request

### PR Review Process

- Maintainers will review your PR
- Address any feedback or requested changes
- Once approved, your PR will be merged

## Getting Help

- **Questions?** Open a [GitHub Discussion](https://github.com/SluberskiHomeLab/decentra-win/discussions)
- **Bug Reports** Open a [GitHub Issue](https://github.com/SluberskiHomeLab/decentra-win/issues)
- **Feature Requests** Open a [GitHub Issue](https://github.com/SluberskiHomeLab/decentra-win/issues) with the "enhancement" label

## License

By contributing to DecentraWin, you agree that your contributions will be licensed under the Apache License 2.0.

Thank you for contributing to DecentraWin! 🎉
