using System.Text.Json;

namespace Payme.Types;

public class PaymeRequest
{
    public string Method { get; set; } = null!;
    public JsonElement Params { get; set; }
    public int Id { get; set; }

    public T GetParams<T>()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        return Params.Deserialize<T>(options) ?? throw new ArgumentException("Unknown params type");
    }
}