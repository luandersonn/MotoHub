namespace MotoHub.Application.Interfaces.Repositories;

public interface IImageRepository
{
    Task<string> UploadImageAsBase64Async(string base64Image, CancellationToken cancellationToken = default);
    Task<string?> GetImageAsBase64Async(string fileIdentifier, CancellationToken cancellationToken = default);
    Task RemoveAsync(string fileIdentifier, CancellationToken cancellationToken = default);
}