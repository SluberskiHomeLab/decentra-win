using System.Windows;
using DecentraWin.ViewModels;
using DecentraWin.Services;

namespace DecentraWin.Views;

public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel;

    public MainWindow(WebSocketService webSocketService, ApiService apiService, AuthService authService, VoiceService voiceService)
    {
        InitializeComponent();
        
        _viewModel = new MainViewModel(webSocketService, apiService, authService, voiceService);
        DataContext = _viewModel;

        Loaded += async (s, e) => await _viewModel.InitializeAsync();

        // Auto-scroll to bottom when new messages arrive
        _viewModel.Messages.CollectionChanged += (s, e) =>
        {
            Dispatcher.Invoke(() =>
            {
                MessagesScrollViewer.ScrollToBottom();
            });
        };

        // Enter key to send message
        MessageInputBox.KeyDown += (s, e) =>
        {
            if (e.Key == System.Windows.Input.Key.Enter && 
                !System.Windows.Input.Keyboard.Modifiers.HasFlag(System.Windows.Input.ModifierKeys.Shift))
            {
                e.Handled = true;
                if (_viewModel.SendMessageCommand.CanExecute(null))
                {
                    _viewModel.SendMessageCommand.Execute(null);
                }
            }
        };
    }
}
