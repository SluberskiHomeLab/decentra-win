# Decentra Windows Desktop Client

A native Windows desktop application for the Decentra chat platform, built with C# and WPF.

## Features

- 🔐 **Authentication** - Login and sign up with invite code support
- 💬 **Real-time Messaging** - WebSocket-based instant messaging
- 🖥️ **Server Management** - Create and join servers, manage channels
- 📺 **Text Channels** - Organized text chat channels within servers
- 🔊 **Voice Channels** - WebRTC-based voice chat (group and direct calls)
- 👥 **Friends System** - Add friends and start direct messages
- 📱 **Direct Messages** - Private conversations with friends
- 🎨 **Modern UI** - Discord-inspired dark theme interface

## System Requirements

- **Operating System**: Windows 10 or Windows 11
- **.NET Runtime**: .NET 8.0 or higher
- **RAM**: 512 MB minimum (1 GB recommended)
- **Storage**: 50 MB available space
- **Network**: Internet connection for server communication

## Installation

### Option 1: Download Pre-built Release (Recommended)

1. Go to the [Releases](https://github.com/SluberskiHomeLab/decentra-win/releases) page
2. Download the latest `DecentraWin-v{version}.zip` file
3. Extract the ZIP file to your desired location
4. Run `DecentraWin.exe`

### Option 2: Build from Source

#### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or higher
- [Visual Studio 2022](https://visualstudio.microsoft.com/) (optional, for IDE support)

#### Build Steps

```bash
# Clone the repository
git clone https://github.com/SluberskiHomeLab/decentra-win.git
cd decentra-win

# Restore dependencies
dotnet restore

# Build the project
dotnet build -c Release

# Run the application
dotnet run --project DecentraWin
```

Or open `DecentraWin.sln` in Visual Studio and build/run from there.

### Option 3: Build Installer/Package

To create a distributable installer or ZIP package:

#### On Windows (with PowerShell):

```powershell
# Build and create installer + ZIP archive
.\build-installer.ps1

# Specify a version
.\build-installer.ps1 -Version "1.0.1"
```

**Requirements for installer creation:**
- [Inno Setup 6](https://jrsoftware.org/isinfo.php) - Free installer creator

The script will create:
- `installer-output/DecentraWin-Setup-v{version}.exe` - Windows installer
- `installer-output/DecentraWin-v{version}-win-x64.zip` - ZIP archive

#### On Linux/macOS (creates ZIP only):

```bash
# Build and create ZIP archive
./build-installer.sh

# Specify a version
./build-installer.sh Release 1.0.1
```

For detailed installer build instructions, see [installer/README.md](installer/README.md).

## Configuration

### Server Connection

By default, the application connects to:
- **WebSocket**: `ws://localhost:8765/ws`
- **API**: `http://localhost:8765/api`

To connect to a different server:
1. Edit the connection settings in the application (Settings menu)
2. Or modify the service initialization in `App.xaml.cs`

### Auto-Login

The application supports remembering your credentials for automatic login. This feature can be enabled/disabled in the settings.

## Usage

### First Time Setup

1. **Launch the Application**
   - Run `DecentraWin.exe`

2. **Create an Account**
   - Enter a username and password
   - (Optional) Enter an invite code if your server requires one
   - Click "Sign Up"

3. **Login**
   - Enter your username and password
   - Click "Login"

### Main Interface

The main window is divided into three sections:

#### Left Sidebar (Server List)
- Shows all servers you're a member of
- Click a server icon to view its channels
- Click the **+** button to create a new server
- Click the **👥** button to view your friends list

#### Middle Panel (Channels/Friends)
- **When a server is selected**: Shows text and voice channels
- **When Friends is selected**: Shows your friends list
- **When a DM is selected**: Shows the conversation

#### Right Panel (Chat Area)
- Displays messages in the current channel/DM
- Type your message in the input box at the bottom
- Press Enter to send (Shift+Enter for new line)

### Features Guide

#### Servers & Channels

**Create a Server:**
1. Click the **+** button in the left sidebar
2. Enter a server name
3. Click "Create"

**Create a Channel:**
1. Select a server
2. Right-click in the channel list
3. Choose "Create Channel"
4. Select channel type (Text or Voice)
5. Enter channel name

**Join a Server:**
1. Ask a server owner for an invite code
2. Click "Join Server" in the menu
3. Enter the invite code

#### Friends & Direct Messages

**Add a Friend:**
1. Click the **👥** Friends button
2. Click "Add Friend"
3. Enter the friend's username
4. Friend is added instantly (no approval needed)

**Start a Direct Message:**
1. Go to your Friends list
2. Click on a friend
3. Click "Send Message"

#### Voice Chat

**Join a Voice Channel:**
1. Select a server
2. Click on a voice channel
3. Click "Join Voice"

**Voice Controls:**
- **Mute/Unmute**: Click the microphone button
- **Leave**: Click "Leave Voice"

**Direct Voice Call:**
1. Go to Friends or open a DM
2. Click the call button
3. Wait for the friend to accept

## Keyboard Shortcuts

- **Enter** - Send message
- **Shift+Enter** - New line in message
- **Ctrl+K** - Quick channel switcher
- **Ctrl+/** - Show keyboard shortcuts
- **Esc** - Close dialogs

## Troubleshooting

### Cannot Connect to Server

**Problem**: Application shows "Connection error"

**Solutions**:
- Verify the server is running
- Check the server URL in settings
- Ensure firewall allows the connection
- Try using `127.0.0.1` instead of `localhost`

### Authentication Failed

**Problem**: Login fails with "Authentication failed"

**Solutions**:
- Verify your username and password are correct
- Ensure the server is accepting connections
- Check if an invite code is required for new accounts

### Voice Chat Not Working

**Problem**: Cannot hear others or others cannot hear you

**Solutions**:
- Check your microphone permissions in Windows
- Verify your speaker/headphone output
- Click the Mute button to ensure you're unmuted
- Restart the voice connection

### Build Errors

**Problem**: Build fails with missing dependencies

**Solutions**:
```bash
# Restore NuGet packages
dotnet restore

# Clean and rebuild
dotnet clean
dotnet build
```

## Architecture

### Technology Stack

- **Framework**: .NET 8.0
- **UI**: WPF (Windows Presentation Foundation)
- **Pattern**: MVVM (Model-View-ViewModel)
- **WebSocket**: System.Net.WebSockets.ClientWebSocket
- **HTTP**: System.Net.Http.HttpClient
- **JSON**: System.Text.Json
- **WebRTC**: SIPSorcery.WebRTC
- **Audio**: NAudio
- **Crypto**: BCrypt.Net-Next

### Project Structure

```
DecentraWin/
├── Models/              # Data models
├── ViewModels/          # MVVM ViewModels
├── Views/               # XAML Views
├── Services/            # Business logic
│   ├── WebSocketService.cs
│   ├── ApiService.cs
│   ├── AuthService.cs
│   ├── VoiceService.cs
│   └── AudioService.cs
├── Helpers/             # Utility classes
└── Resources/           # Styles and assets
```

## API Compatibility

This client is compatible with the [Decentra server](https://github.com/SluberskiHomeLab/decentra).

For API documentation, see: [API.md](https://github.com/SluberskiHomeLab/decentra/blob/main/API.md)

## Development

### Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

### Code Style

- Follow C# coding conventions
- Use meaningful variable and method names
- Add XML documentation comments for public APIs
- Keep methods focused and under 50 lines when possible
- Use async/await for asynchronous operations

### Testing

```bash
# Run tests (when available)
dotnet test
```

## Security

- Passwords are hashed using bcrypt before transmission
- WebSocket connections can use WSS (secure WebSocket)
- Local credentials can be stored using Windows Credential Manager
- All user input is sanitized to prevent injection attacks

**Security Note**: This is a chat application intended for private networks. For production use, ensure you:
- Use HTTPS/WSS for all connections
- Implement proper authentication and authorization
- Regular security audits and updates

## License

This project is licensed under the Apache License 2.0 - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

- Inspired by Discord's UI/UX design
- Built for the [Decentra](https://github.com/SluberskiHomeLab/decentra) platform
- Uses [SIPSorcery](https://github.com/sipsorcery-org/sipsorcery) for WebRTC
- Uses [NAudio](https://github.com/naudio/NAudio) for audio handling

## Support

For issues, questions, or contributions:
- Open an issue on [GitHub Issues](https://github.com/SluberskiHomeLab/decentra-win/issues)
- Check existing issues for solutions
- Provide detailed information for bug reports

## Roadmap

- [ ] Screen sharing in voice channels
- [ ] File attachments in messages
- [ ] Message reactions and emoji support
- [ ] User profiles and avatars
- [ ] Server roles and permissions
- [ ] Message search functionality
- [ ] Notification system
- [ ] Custom themes and appearance settings
- [ ] Multi-server voice chat optimization
- [ ] Mobile companion app sync

---

**Version**: 1.0.0  
**Last Updated**: December 2025  
**Maintained by**: Sluberski Home Lab

