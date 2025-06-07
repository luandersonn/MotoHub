using System.Text.Json.Serialization;

namespace MotoHub.API.Requests;

public class RegisterMotorcycleRequest
{
    [JsonPropertyName("identificador")]
    public string Identifier { get; set; }

    [JsonPropertyName("ano")]
    public int Year { get; set; }

    [JsonPropertyName("modelo")]
    public string Model { get; set; }

    [JsonPropertyName("placa")]
    public string Plate { get; set; }
}