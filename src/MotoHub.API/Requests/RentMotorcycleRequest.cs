using System.Text.Json.Serialization;

namespace MotoHub.API.Requests;

public class RentMotorcycleRequest
{
    [JsonPropertyName("entregador_id")]
    public string TenantIdentifier { get; set; }

    [JsonPropertyName("moto_id")]
    public string MotorcycleIdentifier { get; set; }

    [JsonPropertyName("data_inicio")]
    public DateTime StartDate { get; set; }

    [JsonPropertyName("data_termino")]
    public DateTime EndDate { get; set; }

    [JsonPropertyName("data_previsao_termino")]
    public DateTime EstimatedEndDate { get; set; }

    [JsonPropertyName("plano")]
    public required int Plan { get; set; }
}