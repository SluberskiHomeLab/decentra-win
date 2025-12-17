namespace DecentraWin.Models;

public class DirectMessage
{
    public string DmId { get; set; } = string.Empty;
    public List<string> Participants { get; set; } = new();
    public string OtherUser { get; set; } = string.Empty;
    public Message? LastMessage { get; set; }
    public int UnreadCount { get; set; }
}
