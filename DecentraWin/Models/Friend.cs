namespace DecentraWin.Models;

public class Friend
{
    public string Username { get; set; } = string.Empty;
    public string? EmojiAvatar { get; set; }
    public bool IsOnline { get; set; }
    public string Status { get; set; } = "offline";
}
