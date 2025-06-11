using MotoHub.Application.DTOs;
using MotoHub.Application.Interfaces.Repositories;
using MotoHub.Application.UseCases.Renting;
using MotoHub.Domain.Common;
using MotoHub.Domain.Entities;

namespace MotoHub.Tests.UseCases.Renting;

[TestFixture]
public class GetRentByIdentifierUseCaseTests
{
    private Mock<IRentRepository> _rentRepositoryMock;
    private GetRentByIdentifierUseCase _useCase;

    [SetUp]
    public void Setup()
    {
        _rentRepositoryMock = new Mock<IRentRepository>();
        _useCase = new GetRentByIdentifierUseCase(_rentRepositoryMock.Object);
    }

    [Test]
    public async Task ExecuteAsync_WithNonExistingRent_ShouldReturnNotFoundError()
    {
        string identifier = "rent-001";

        _rentRepositoryMock.Setup(r => r.GetByIdAsync(identifier, It.IsAny<CancellationToken>()))
                           .ReturnsAsync((Rent?)null);

        Result<RentDto> result = await _useCase.ExecuteAsync(identifier);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorType, Is.EqualTo(ResultErrorType.NotFound));
            Assert.That(result.ErrorMessage, Is.EqualTo("Aluguel não encontrado"));
        });
    }

    [Test]
    public async Task ExecuteAsync_WithExistingRent_ShouldReturnRentData()
    {
        string identifier = "rent-001";

        Rent rent = new()
        {
            Id = identifier,
            MotorcycleIdentifier = "moto-001",
            CourierIdentifier = "courier-001",
            StartDate = DateTime.UtcNow.AddDays(-3),
            EndDate = DateTime.UtcNow.AddDays(-1),
            EstimatedEndDate = DateTime.UtcNow.AddDays(2),
            Status = RentStatus.Active,
            DailyRate = 150.00m
        };

        _rentRepositoryMock.Setup(r => r.GetByIdAsync(identifier, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(rent);

        Result<RentDto> result = await _useCase.ExecuteAsync(identifier);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Data?.Identifier, Is.EqualTo(rent.Id));
            Assert.That(result.Data?.MotorcycleIdentifier, Is.EqualTo(rent.MotorcycleIdentifier));
            Assert.That(result.Data?.CourierIdentifier, Is.EqualTo(rent.CourierIdentifier));
            Assert.That(result.Data?.StartDate, Is.EqualTo(rent.StartDate));
            Assert.That(result.Data?.EndDate, Is.EqualTo(rent.EndDate));
            Assert.That(result.Data?.EstimatedEndDate, Is.EqualTo(rent.EstimatedEndDate));
            Assert.That(result.Data?.Status, Is.EqualTo(rent.Status));
            Assert.That(result.Data?.DailyRate, Is.EqualTo(rent.DailyRate));
        });

        _rentRepositoryMock.Verify(r => r.GetByIdAsync(identifier, It.IsAny<CancellationToken>()), Times.Once);
    }
}