namespace MotoHub.Infrastructure.Messaging;

public class AwsSQSSettings
{
    public required string QueueUrl { get; set; }
    public required string Secret { get; set; }
    public required string Key { get; set; }
}
