using System.Text.Json.Serialization;

namespace MotoHub.API.Requests;

public class RegisterCourierRequest
{
    [JsonPropertyName("identificador")]
    public string Identifier { get; set; }

    [JsonPropertyName("nome")]
    public string Name { get; set; }

    [JsonPropertyName("cnpj")]
    public string TaxNumber { get; set; }

    [JsonPropertyName("data_nascimento")]
    public DateTime BirthDate { get; set; }

    [JsonPropertyName("numero_cnh")]
    public string DriverLicenseNumber { get; set; }

    [JsonPropertyName("tipo_cnh")]
    public string DriverLicenseType { get; set; }

    [JsonPropertyName("imagem_CNH")]
    public string DriverLicenseImageBase64 { get; set; }
}