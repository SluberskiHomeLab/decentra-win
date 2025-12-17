using System.Text.Json.Serialization;

namespace DecentraWin.Models;

public class WebSocketMessage
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("username")]
    public string? Username { get; set; }

    [JsonPropertyName("password")]
    public string? Password { get; set; }

    [JsonPropertyName("invite_code")]
    public string? InviteCode { get; set; }

    [JsonPropertyName("content")]
    public string? Content { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("server_id")]
    public string? ServerId { get; set; }

    [JsonPropertyName("channel_id")]
    public string? ChannelId { get; set; }

    [JsonPropertyName("channel_type")]
    public string? ChannelType { get; set; }

    [JsonPropertyName("friend_username")]
    public string? FriendUsername { get; set; }

    [JsonPropertyName("dm_id")]
    public string? DmId { get; set; }

    [JsonPropertyName("target_username")]
    public string? TargetUsername { get; set; }

    [JsonPropertyName("target")]
    public string? Target { get; set; }

    [JsonPropertyName("muted")]
    public bool? Muted { get; set; }

    [JsonPropertyName("offer")]
    public object? Offer { get; set; }

    [JsonPropertyName("answer")]
    public object? Answer { get; set; }

    [JsonPropertyName("candidate")]
    public object? Candidate { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("timestamp")]
    public string? Timestamp { get; set; }

    [JsonPropertyName("servers")]
    public List<Server>? Servers { get; set; }

    [JsonPropertyName("channels")]
    public List<Channel>? Channels { get; set; }

    [JsonPropertyName("friends")]
    public List<Friend>? Friends { get; set; }

    [JsonPropertyName("dms")]
    public List<DirectMessage>? Dms { get; set; }

    [JsonPropertyName("history")]
    public List<Message>? History { get; set; }

    [JsonPropertyName("code")]
    public string? Code { get; set; }
}
