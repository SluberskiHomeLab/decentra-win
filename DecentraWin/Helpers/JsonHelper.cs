using System.Text.Json;
using System.Text.Json.Serialization;

namespace DecentraWin.Helpers;

public static class JsonHelper
{
    private static readonly JsonSerializerOptions _options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = false
    };

    public static string Serialize<T>(T obj)
    {
        return JsonSerializer.Serialize(obj, _options);
    }

    public static T? Deserialize<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json, _options);
    }
}
