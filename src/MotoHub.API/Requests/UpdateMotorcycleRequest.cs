using System.Text.Json.Serialization;

namespace MotoHub.API.Requests;

public class UpdateMotorcycleRequest
{
    [JsonPropertyName("Placa")]
    public string Plate { get; set; }
}