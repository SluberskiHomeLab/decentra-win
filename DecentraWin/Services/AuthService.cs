using BCrypt.Net;

namespace DecentraWin.Services;

public class AuthService
{
    private string? _currentUsername;
    private string? _sessionToken;

    public string? CurrentUsername => _currentUsername;
    public bool IsAuthenticated => !string.IsNullOrEmpty(_currentUsername);

    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool VerifyPassword(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }

    public void SetAuthenticated(string username, string? token = null)
    {
        _currentUsername = username;
        _sessionToken = token;
    }

    public void Logout()
    {
        _currentUsername = null;
        _sessionToken = null;
    }
}
