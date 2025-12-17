using DecentraWin.Models;
using DecentraWin.Helpers;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace DecentraWin.Services;

public class WebSocketService
{
    private ClientWebSocket? _webSocket;
    private CancellationTokenSource? _cancellationTokenSource;
    private string _serverUrl = "ws://localhost:8765/ws";
    private bool _isConnected;

    public event EventHandler<WebSocketMessage>? MessageReceived;
    public event EventHandler? Connected;
    public event EventHandler? Disconnected;

    public bool IsConnected => _isConnected;

    public void SetServerUrl(string url)
    {
        _serverUrl = url;
    }

    public async Task ConnectAsync()
    {
        try
        {
            _webSocket = new ClientWebSocket();
            _cancellationTokenSource = new CancellationTokenSource();

            await _webSocket.ConnectAsync(new Uri(_serverUrl), _cancellationTokenSource.Token);
            _isConnected = true;
            Connected?.Invoke(this, EventArgs.Empty);

            _ = Task.Run(async () => await ReceiveMessagesAsync());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"WebSocket connection error: {ex.Message}");
            _isConnected = false;
        }
    }

    public async Task DisconnectAsync()
    {
        if (_webSocket != null && _webSocket.State == WebSocketState.Open)
        {
            _cancellationTokenSource?.Cancel();
            await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client disconnecting", CancellationToken.None);
            _isConnected = false;
            Disconnected?.Invoke(this, EventArgs.Empty);
        }
    }

    public async Task SendMessageAsync(WebSocketMessage message)
    {
        if (_webSocket == null || _webSocket.State != WebSocketState.Open)
        {
            Console.WriteLine("WebSocket is not connected");
            return;
        }

        try
        {
            var json = JsonSerializer.Serialize(message);
            var bytes = Encoding.UTF8.GetBytes(json);
            await _webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending WebSocket message: {ex.Message}");
        }
    }

    private async Task ReceiveMessagesAsync()
    {
        var buffer = new byte[8192];

        try
        {
            while (_webSocket != null && _webSocket.State == WebSocketState.Open && !_cancellationTokenSource!.Token.IsCancellationRequested)
            {
                var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), _cancellationTokenSource.Token);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                    _isConnected = false;
                    Disconnected?.Invoke(this, EventArgs.Empty);
                    break;
                }

                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                var wsMessage = JsonSerializer.Deserialize<WebSocketMessage>(message);

                if (wsMessage != null)
                {
                    MessageReceived?.Invoke(this, wsMessage);
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Expected when cancellation token is triggered
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error receiving WebSocket message: {ex.Message}");
            _isConnected = false;
            Disconnected?.Invoke(this, EventArgs.Empty);
        }
    }
}
