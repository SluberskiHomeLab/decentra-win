namespace DecentraWin.Models;

public class User
{
    public string Username { get; set; } = string.Empty;
    public string? EmojiAvatar { get; set; }
    public bool IsOnline { get; set; }
    public DateTime? LastSeen { get; set; }
}
