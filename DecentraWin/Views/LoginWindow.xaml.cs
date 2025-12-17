using System.Windows;
using DecentraWin.ViewModels;
using DecentraWin.Services;

namespace DecentraWin.Views;

public partial class LoginWindow : Window
{
    private readonly LoginViewModel _viewModel;

    public LoginWindow(WebSocketService webSocketService, AuthService authService)
    {
        InitializeComponent();
        
        _viewModel = new LoginViewModel(webSocketService, authService);
        DataContext = _viewModel;

        // Bind events
        LoginButton.Click += async (s, e) => await HandleLoginAsync();
        SignUpButton.Click += async (s, e) => await HandleSignUpAsync();
        
        _viewModel.LoginSuccessful += OnLoginSuccessful;
        _viewModel.PropertyChanged += ViewModel_PropertyChanged;

        UsernameTextBox.Focus();
    }

    private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_viewModel.StatusMessage))
        {
            StatusTextBlock.Text = _viewModel.StatusMessage;
        }
        else if (e.PropertyName == nameof(_viewModel.IsLoading))
        {
            LoginButton.IsEnabled = !_viewModel.IsLoading;
            SignUpButton.IsEnabled = !_viewModel.IsLoading;
        }
    }

    private async Task HandleLoginAsync()
    {
        _viewModel.Username = UsernameTextBox.Text;
        _viewModel.Password = PasswordBox.Password;
        
        if (_viewModel.LoginCommand.CanExecute(null))
        {
            _viewModel.LoginCommand.Execute(null);
        }
    }

    private async Task HandleSignUpAsync()
    {
        _viewModel.Username = UsernameTextBox.Text;
        _viewModel.Password = PasswordBox.Password;
        _viewModel.InviteCode = InviteCodeTextBox.Text;
        
        if (_viewModel.SignUpCommand.CanExecute(null))
        {
            _viewModel.SignUpCommand.Execute(null);
        }
    }

    private void OnLoginSuccessful(object? sender, EventArgs e)
    {
        DialogResult = true;
        Close();
    }

    public LoginViewModel ViewModel => _viewModel;
}
