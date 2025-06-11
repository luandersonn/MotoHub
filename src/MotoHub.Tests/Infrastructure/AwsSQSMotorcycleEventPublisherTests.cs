using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MotoHub.Application.Events;
using MotoHub.Infrastructure.Messaging;
using MotoHub.Infrastructure.Settings;

namespace MotoHub.Tests.Infrastructure;

[TestFixture]
public class AwsSQSMotorcycleEventPublisherTests
{
    private Mock<IAmazonSQS> _sqsClientMock;
    private Mock<IOptions<AwsSQSSettings>> _settingsMock;
    private Mock<ILogger<AwsSQSMotorcycleEventPublisher>> _loggerMock;
    private AwsSQSMotorcycleEventPublisher _publisher;

    private const string QueueUrl = "http://queueUrl";

    [SetUp]
    public void Setup()
    {
        _sqsClientMock = new Mock<IAmazonSQS>();
        _settingsMock = new Mock<IOptions<AwsSQSSettings>>();
        _loggerMock = new Mock<ILogger<AwsSQSMotorcycleEventPublisher>>();

        _settingsMock.Setup(s => s.Value).Returns(new AwsSQSSettings { QueueUrl = QueueUrl, Key = "", Secret = "", });

        _publisher = new AwsSQSMotorcycleEventPublisher(_sqsClientMock.Object, _settingsMock.Object, _loggerMock.Object);
    }

    [Test]
    public void Constructor_WithMissingQueueUrl_ShouldThrowException()
    {
        _settingsMock.Setup(s => s.Value).Returns(new AwsSQSSettings { QueueUrl = null!, Key = "", Secret = "", });

        Assert.Throws<InvalidOperationException>(() =>
            new AwsSQSMotorcycleEventPublisher(_sqsClientMock.Object, _settingsMock.Object, _loggerMock.Object));
    }

    [Test]
    public async Task PublishMotorcycleRegisteredAsync_ShouldSendMessageToSQS()
    {
        MotorcycleRegisteredEvent motorcycleEvent = new("moto-001", "ABC123", 2023, "Sport");

        _sqsClientMock.Setup(s => s.SendMessageAsync(It.IsAny<SendMessageRequest>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(new SendMessageResponse
                      {
                          HttpStatusCode = System.Net.HttpStatusCode.OK,
                          SequenceNumber = "12345",
                          MD5OfMessageBody = "abcde12345"
                      });

        await _publisher.PublishMotorcycleRegisteredAsync(motorcycleEvent, CancellationToken.None);

        _sqsClientMock.Verify(s => s.SendMessageAsync(It.Is<SendMessageRequest>(req =>
            req.QueueUrl == QueueUrl &&
            req.MessageBody.Contains("\"Identifier\":\"moto-001\"") &&
            req.MessageBody.Contains("\"Plate\":\"ABC123\"")), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task PublishMotorcycleRegisteredAsync_WithException_ShouldLogError()
    {
        MotorcycleRegisteredEvent motorcycleEvent = new("moto-001", "ABC123", 2023, "Sport");

        _sqsClientMock.Setup(s => s.SendMessageAsync(It.IsAny<SendMessageRequest>(), It.IsAny<CancellationToken>()))
                      .ThrowsAsync(new Exception("SQS error"));

        await _publisher.PublishMotorcycleRegisteredAsync(motorcycleEvent, CancellationToken.None);

        _loggerMock.Verify(l => l.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((o, _) => o.ToString()!.Contains("Erro ao publicar evento")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }
}
