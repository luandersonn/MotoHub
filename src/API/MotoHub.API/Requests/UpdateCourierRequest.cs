using System.Text.Json.Serialization;

namespace MotoHub.API.Requests;

public class UpdateCourierRequest
{
    [JsonPropertyName("imagem_cnh")]
    public string DriverLicenseImageBase64 { get; set; }
}