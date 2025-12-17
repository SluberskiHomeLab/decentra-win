using System.Windows;
using DecentraWin.Services;
using DecentraWin.Views;

namespace DecentraWin;

public partial class App : Application
{
    private WebSocketService? _webSocketService;
    private ApiService? _apiService;
    private AuthService? _authService;
    private AudioService? _audioService;
    private VoiceService? _voiceService;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Initialize services
        _webSocketService = new WebSocketService();
        _apiService = new ApiService();
        _authService = new AuthService();
        _audioService = new AudioService();
        _voiceService = new VoiceService(_webSocketService, _audioService);

        // Show login window
        var loginWindow = new LoginWindow(_webSocketService, _authService);
        
        // Set up message handler for auth responses
        _webSocketService.MessageReceived += (sender, message) =>
        {
            if (message.Type == "auth_success")
            {
                Dispatcher.Invoke(() => loginWindow.ViewModel.OnAuthSuccess());
            }
            else if (message.Type == "auth_failed")
            {
                Dispatcher.Invoke(() => loginWindow.ViewModel.OnAuthFailed(message.Message ?? "Authentication failed"));
            }
        };

        if (loginWindow.ShowDialog() == true)
        {
            // Login successful, show main window
            var mainWindow = new MainWindow(_webSocketService, _apiService, _authService, _voiceService);
            mainWindow.Show();
        }
        else
        {
            // Login cancelled, exit application
            Shutdown();
        }
    }

    protected override void OnExit(ExitEventArgs e)
    {
        // Clean up
        _webSocketService?.DisconnectAsync().Wait();
        _audioService?.StopCapture();
        _audioService?.StopPlayback();
        
        base.OnExit(e);
    }
}

