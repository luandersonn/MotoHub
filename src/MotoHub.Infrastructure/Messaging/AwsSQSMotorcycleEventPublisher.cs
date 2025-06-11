using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MotoHub.Application.Events;
using MotoHub.Application.Interfaces.Messaging;
using MotoHub.Infrastructure.Settings;
using System.Text.Json;

namespace MotoHub.Infrastructure.Messaging;

public class AwsSQSMotorcycleEventPublisher(IAmazonSQS sqsClient, IOptions<AwsSQSSettings> settings, ILogger<AwsSQSMotorcycleEventPublisher> logger) : IMotorcycleEventPublisher
{
    private static readonly JsonSerializerOptions _options = new()
    {

    };

    private readonly string _queueUrl = settings.Value.QueueUrl ?? throw new InvalidOperationException("Queue URL not configured");

    public async Task PublishMotorcycleRegisteredAsync(MotorcycleRegisteredEvent @event, CancellationToken cancellationToken)
    {
        try
        {
            string messageBody = JsonSerializer.Serialize(@event, _options);

            SendMessageRequest request = new()
            {
                QueueUrl = _queueUrl,
                MessageBody = messageBody
            };

            SendMessageResponse response = await sqsClient.SendMessageAsync(request, cancellationToken);            
            
            logger.LogDebug("Evento de registro de moto publicado. Resposta com Status Code = {statusCode}, Sequence Number = {sequenceNumber}, MD5 of body = {md5}",
                            response.HttpStatusCode,
                            response.SequenceNumber,
                            response.MD5OfMessageBody);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao publicar evento");
        }
    }
}
