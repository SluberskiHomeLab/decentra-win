namespace DecentraWin.Models;

public class Channel
{
    public string ChannelId { get; set; } = string.Empty;
    public string ServerId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string ChannelType { get; set; } = "text"; // "text" or "voice"
    public List<string> ActiveUsers { get; set; } = new();
}
