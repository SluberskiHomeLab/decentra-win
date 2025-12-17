using DecentraWin.Helpers;
using DecentraWin.Models;
using DecentraWin.Services;
using System.Collections.ObjectModel;
using System.Windows;

namespace DecentraWin.ViewModels;

public class MainViewModel : ObservableObject
{
    private readonly WebSocketService _webSocketService;
    private readonly ApiService _apiService;
    private readonly AuthService _authService;
    private readonly VoiceService _voiceService;

    private Server? _selectedServer;
    private Channel? _selectedChannel;
    private DirectMessage? _selectedDm;
    private string _currentView = "servers"; // servers, friends, dm
    private string _messageInput = string.Empty;
    private bool _isInVoice;

    public ObservableCollection<Server> Servers { get; } = new();
    public ObservableCollection<Channel> Channels { get; } = new();
    public ObservableCollection<Friend> Friends { get; } = new();
    public ObservableCollection<DirectMessage> DirectMessages { get; } = new();
    public ObservableCollection<Message> Messages { get; } = new();

    public Server? SelectedServer
    {
        get => _selectedServer;
        set
        {
            if (SetProperty(ref _selectedServer, value) && value != null)
            {
                _ = SwitchServerAsync(value.ServerId);
            }
        }
    }

    public Channel? SelectedChannel
    {
        get => _selectedChannel;
        set
        {
            if (SetProperty(ref _selectedChannel, value) && value != null)
            {
                _ = SwitchChannelAsync(value.ChannelId);
            }
        }
    }

    public DirectMessage? SelectedDm
    {
        get => _selectedDm;
        set
        {
            if (SetProperty(ref _selectedDm, value) && value != null)
            {
                _ = SwitchDmAsync(value.DmId);
            }
        }
    }

    public string CurrentView
    {
        get => _currentView;
        set => SetProperty(ref _currentView, value);
    }

    public string MessageInput
    {
        get => _messageInput;
        set => SetProperty(ref _messageInput, value);
    }

    public bool IsInVoice
    {
        get => _isInVoice;
        set => SetProperty(ref _isInVoice, value);
    }

    public RelayCommand SendMessageCommand { get; }
    public RelayCommand ShowServersCommand { get; }
    public RelayCommand ShowFriendsCommand { get; }
    public RelayCommand CreateServerCommand { get; }
    public RelayCommand JoinVoiceCommand { get; }
    public RelayCommand LeaveVoiceCommand { get; }
    public RelayCommand ToggleMuteCommand { get; }

    public MainViewModel(WebSocketService webSocketService, ApiService apiService, AuthService authService, VoiceService voiceService)
    {
        _webSocketService = webSocketService;
        _apiService = apiService;
        _authService = authService;
        _voiceService = voiceService;

        SendMessageCommand = new RelayCommand(async () => await SendMessageAsync(), () => !string.IsNullOrWhiteSpace(MessageInput));
        ShowServersCommand = new RelayCommand(() => CurrentView = "servers");
        ShowFriendsCommand = new RelayCommand(() => CurrentView = "friends");
        CreateServerCommand = new RelayCommand(async () => await CreateServerAsync());
        JoinVoiceCommand = new RelayCommand(async () => await JoinVoiceAsync(), () => SelectedChannel?.ChannelType == "voice");
        LeaveVoiceCommand = new RelayCommand(async () => await LeaveVoiceAsync(), () => IsInVoice);
        ToggleMuteCommand = new RelayCommand(() => _voiceService.SetMuted(!_voiceService.InVoiceChannel));

        _webSocketService.MessageReceived += OnWebSocketMessageReceived;
    }

    public async Task InitializeAsync()
    {
        if (_authService.CurrentUsername != null)
        {
            var servers = await _apiService.GetServersAsync(_authService.CurrentUsername);
            if (servers != null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Servers.Clear();
                    foreach (var server in servers)
                    {
                        Servers.Add(server);
                    }
                });
            }

            var friends = await _apiService.GetFriendsAsync(_authService.CurrentUsername);
            if (friends != null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Friends.Clear();
                    foreach (var friend in friends)
                    {
                        Friends.Add(friend);
                    }
                });
            }

            var dms = await _apiService.GetDirectMessagesAsync(_authService.CurrentUsername);
            if (dms != null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    DirectMessages.Clear();
                    foreach (var dm in dms)
                    {
                        DirectMessages.Add(dm);
                    }
                });
            }
        }
    }

    private async Task SendMessageAsync()
    {
        if (string.IsNullOrWhiteSpace(MessageInput))
            return;

        await _webSocketService.SendMessageAsync(new WebSocketMessage
        {
            Type = "message",
            Content = MessageInput
        });

        MessageInput = string.Empty;
    }

    private async Task SwitchServerAsync(string serverId)
    {
        await _webSocketService.SendMessageAsync(new WebSocketMessage
        {
            Type = "switch_server",
            ServerId = serverId
        });
    }

    private async Task SwitchChannelAsync(string channelId)
    {
        await _webSocketService.SendMessageAsync(new WebSocketMessage
        {
            Type = "switch_channel",
            ChannelId = channelId
        });
    }

    private async Task SwitchDmAsync(string dmId)
    {
        await _webSocketService.SendMessageAsync(new WebSocketMessage
        {
            Type = "switch_dm",
            DmId = dmId
        });
    }

    private async Task CreateServerAsync()
    {
        // This would show a dialog to get server name
        await _webSocketService.SendMessageAsync(new WebSocketMessage
        {
            Type = "create_server",
            Name = "New Server"
        });
    }

    private async Task JoinVoiceAsync()
    {
        if (SelectedChannel != null && SelectedServer != null)
        {
            await _voiceService.JoinVoiceChannelAsync(SelectedServer.ServerId, SelectedChannel.ChannelId);
            IsInVoice = true;
        }
    }

    private async Task LeaveVoiceAsync()
    {
        await _voiceService.LeaveVoiceChannelAsync();
        IsInVoice = false;
    }

    private void OnWebSocketMessageReceived(object? sender, WebSocketMessage message)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            switch (message.Type)
            {
                case "message":
                    if (!string.IsNullOrEmpty(message.Content) && !string.IsNullOrEmpty(message.Username))
                    {
                        Messages.Add(new Message
                        {
                            Content = message.Content,
                            Username = message.Username,
                            Timestamp = DateTime.TryParse(message.Timestamp, out var ts) ? ts : DateTime.Now,
                            MessageType = "message"
                        });
                    }
                    break;

                case "history":
                    Messages.Clear();
                    if (message.History != null)
                    {
                        foreach (var msg in message.History)
                        {
                            Messages.Add(msg);
                        }
                    }
                    break;

                case "server_list":
                    Servers.Clear();
                    if (message.Servers != null)
                    {
                        foreach (var server in message.Servers)
                        {
                            Servers.Add(server);
                        }
                    }
                    break;

                case "channel_list":
                    Channels.Clear();
                    if (message.Channels != null)
                    {
                        foreach (var channel in message.Channels)
                        {
                            Channels.Add(channel);
                        }
                    }
                    break;

                case "friends_list":
                    Friends.Clear();
                    if (message.Friends != null)
                    {
                        foreach (var friend in message.Friends)
                        {
                            Friends.Add(friend);
                        }
                    }
                    break;

                case "dm_list":
                    DirectMessages.Clear();
                    if (message.Dms != null)
                    {
                        foreach (var dm in message.Dms)
                        {
                            DirectMessages.Add(dm);
                        }
                    }
                    break;

                case "system":
                    if (!string.IsNullOrEmpty(message.Content))
                    {
                        Messages.Add(new Message
                        {
                            Content = message.Content,
                            Username = "System",
                            Timestamp = DateTime.Now,
                            MessageType = "system"
                        });
                    }
                    break;
            }
        });
    }
}
