namespace DecentraWin.Models;

public class Message
{
    public string MessageId { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string MessageType { get; set; } = "message"; // "message", "system"
    public string? ContextType { get; set; }
    public string? ContextId { get; set; }
}
