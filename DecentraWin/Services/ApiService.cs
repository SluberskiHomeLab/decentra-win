using DecentraWin.Models;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace DecentraWin.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;
    private string _baseUrl = "http://localhost:8765/api";

    public ApiService()
    {
        _httpClient = new HttpClient();
    }

    public void SetBaseUrl(string url)
    {
        _baseUrl = url;
    }

    public async Task<List<Server>?> GetServersAsync(string username)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/servers?username={username}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<Server>>(json);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting servers: {ex.Message}");
        }
        return null;
    }

    public async Task<List<Message>?> GetMessagesAsync(string contextType, string contextId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/messages?context_type={contextType}&context_id={contextId}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<Message>>(json);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting messages: {ex.Message}");
        }
        return null;
    }

    public async Task<List<Friend>?> GetFriendsAsync(string username)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/friends?username={username}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<Friend>>(json);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting friends: {ex.Message}");
        }
        return null;
    }

    public async Task<List<DirectMessage>?> GetDirectMessagesAsync(string username)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/dms?username={username}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<DirectMessage>>(json);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting DMs: {ex.Message}");
        }
        return null;
    }

    public async Task<bool> AuthenticateAsync(string username, string password)
    {
        try
        {
            var content = new StringContent(
                JsonSerializer.Serialize(new { username, password }),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync($"{_baseUrl}/auth", content);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error authenticating: {ex.Message}");
            return false;
        }
    }
}
