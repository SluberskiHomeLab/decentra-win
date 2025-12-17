namespace DecentraWin.Models;

public class Server
{
    public string ServerId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Owner { get; set; } = string.Empty;
    public List<string> Members { get; set; } = new();
    public List<Channel> Channels { get; set; } = new();
    public Dictionary<string, List<string>> Permissions { get; set; } = new();
}
