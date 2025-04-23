using System.Text.Json;
using System.Text.Json.Serialization;

namespace MODBD_Common.Text;

public static class DMExtendedJsonSerializerOptions
{
    public static readonly JsonSerializerOptions Instance = new ()
    {
        PropertyNameCaseInsensitive = true, // This will make the property names case-insensitive
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase, // Use camelCase naming convention
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) } // Convert enum strings to camelCase
    };
}
