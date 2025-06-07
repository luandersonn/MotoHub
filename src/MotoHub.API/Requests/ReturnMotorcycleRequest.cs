using System.Text.Json.Serialization;

namespace MotoHub.API.Requests;

public class ReturnMotorcycleRequest
{
    [JsonPropertyName("data_devolucao")]
    public required DateTime ReturnDate { get; set; }
}