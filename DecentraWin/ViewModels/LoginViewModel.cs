using DecentraWin.Helpers;
using DecentraWin.Services;
using DecentraWin.Models;
using System.Windows;

namespace DecentraWin.ViewModels;

public class LoginViewModel : ObservableObject
{
    private readonly WebSocketService _webSocketService;
    private readonly AuthService _authService;
    private string _username = string.Empty;
    private string _password = string.Empty;
    private string _inviteCode = string.Empty;
    private string _statusMessage = string.Empty;
    private bool _isLoading;

    public string Username
    {
        get => _username;
        set => SetProperty(ref _username, value);
    }

    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    public string InviteCode
    {
        get => _inviteCode;
        set => SetProperty(ref _inviteCode, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public RelayCommand LoginCommand { get; }
    public RelayCommand SignUpCommand { get; }

    public event EventHandler? LoginSuccessful;

    public LoginViewModel(WebSocketService webSocketService, AuthService authService)
    {
        _webSocketService = webSocketService;
        _authService = authService;

        LoginCommand = new RelayCommand(async () => await LoginAsync(), () => !IsLoading && !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password));
        SignUpCommand = new RelayCommand(async () => await SignUpAsync(), () => !IsLoading && !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password));
    }

    private async Task LoginAsync()
    {
        IsLoading = true;
        StatusMessage = "Connecting...";

        try
        {
            if (!_webSocketService.IsConnected)
            {
                await _webSocketService.ConnectAsync();
            }

            await _webSocketService.SendMessageAsync(new WebSocketMessage
            {
                Type = "auth",
                Username = Username,
                Password = Password
            });

            // Wait for auth response (handled in MainViewModel)
        }
        catch (Exception ex)
        {
            StatusMessage = $"Connection error: {ex.Message}";
            IsLoading = false;
        }
    }

    private async Task SignUpAsync()
    {
        IsLoading = true;
        StatusMessage = "Creating account...";

        try
        {
            if (!_webSocketService.IsConnected)
            {
                await _webSocketService.ConnectAsync();
            }

            await _webSocketService.SendMessageAsync(new WebSocketMessage
            {
                Type = "signup",
                Username = Username,
                Password = Password,
                InviteCode = InviteCode
            });

            // Wait for auth response (handled in MainViewModel)
        }
        catch (Exception ex)
        {
            StatusMessage = $"Sign up error: {ex.Message}";
            IsLoading = false;
        }
    }

    public void OnAuthSuccess()
    {
        _authService.SetAuthenticated(Username);
        IsLoading = false;
        StatusMessage = "Login successful!";
        LoginSuccessful?.Invoke(this, EventArgs.Empty);
    }

    public void OnAuthFailed(string message)
    {
        IsLoading = false;
        StatusMessage = message;
    }
}
