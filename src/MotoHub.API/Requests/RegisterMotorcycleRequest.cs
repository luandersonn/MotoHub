using System.Text.Json.Serialization;

namespace MotoHub.API.Requests;

public class RegisterMotorcycleRequest
{
    [JsonPropertyName("Identificador")]
    public string Identifier { get; set; }

    [JsonPropertyName("Ano")]
    public int Year { get; set; }

    [JsonPropertyName("Modelo")]
    public string Model { get; set; }

    [JsonPropertyName("Placa")]
    public string Plate { get; set; }
}