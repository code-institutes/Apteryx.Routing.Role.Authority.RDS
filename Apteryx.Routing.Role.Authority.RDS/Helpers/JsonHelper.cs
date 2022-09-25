using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace Apteryx.Routing.Role.Authority.RDS
{
    public static class JsonHelper
    {
        public static string ToJson<T>(this T obj)
        {
            JsonSerializerOptions jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
            };
            return JsonSerializer.Serialize<T>(obj, jsonOptions);
        }

        public static T? FromJson<T>(this string s)
        {
            JsonSerializerOptions jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
            };
            return JsonSerializer.Deserialize<T>(s,jsonOptions);
        }
    }
}
