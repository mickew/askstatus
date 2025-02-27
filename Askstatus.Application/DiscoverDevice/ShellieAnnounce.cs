using System.Text.Json.Serialization;

namespace Askstatus.Application.DiscoverDevice;

public record ShellieAnnounce(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("model")] string Model,
    [property: JsonPropertyName("mac")] string Mac,
    [property: JsonPropertyName("ip")] string Ip,
    [property: JsonPropertyName("gen")] int Gen
 );
