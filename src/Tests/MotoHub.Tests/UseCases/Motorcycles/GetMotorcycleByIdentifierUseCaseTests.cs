using MotoHub.Application.DTOs;
using MotoHub.Application.Interfaces.Repositories;
using MotoHub.Application.UseCases.Motorcycles;
using MotoHub.Domain.Common;
using MotoHub.Domain.Entities;

namespace MotoHub.Tests.UseCases.Motorcycles;

[TestFixture]
public class GetMotorcycleByIdentifierUseCaseTests
{
    private Mock<IMotorcycleRepository> _repositoryMock;
    private GetMotorcycleByIdentifierUseCase _useCase;

    [SetUp]
    public void Setup()
    {
        _repositoryMock = new Mock<IMotorcycleRepository>();
        _useCase = new GetMotorcycleByIdentifierUseCase(_repositoryMock.Object);
    }

    [Test]
    public async Task ExecuteAsync_WithNonExistingMotorcycle_ShouldReturnNotFoundError()
    {
        string identifier = "123";

        _repositoryMock.Setup(r => r.GetByIdAsync(identifier, It.IsAny<CancellationToken>()))
                       .ReturnsAsync((Motorcycle?)null);

        Result<MotorcycleDto> result = await _useCase.ExecuteAsync(identifier);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorType, Is.EqualTo(ResultErrorType.NotFound));
            Assert.That(result.ErrorMessage, Is.EqualTo("Moto não encontrada"));
        });
    }

    [Test]
    public async Task ExecuteAsync_WithExistingMotorcycle_ShouldReturnMotorcycleData()
    {
        string identifier = "123";

        Motorcycle motorcycle = new()
        {
            Id = identifier,
            Plate = "ABC123",
            Year = 2020,
            Model = "Sport"
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(identifier, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(motorcycle);

        Result<MotorcycleDto> result = await _useCase.ExecuteAsync(identifier);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Data?.Identifier, Is.EqualTo(motorcycle.Id));
            Assert.That(result.Data?.Plate, Is.EqualTo(motorcycle.Plate));
            Assert.That(result.Data?.Year, Is.EqualTo(motorcycle.Year));
            Assert.That(result.Data?.Model, Is.EqualTo(motorcycle.Model));
        });

        _repositoryMock.Verify(r => r.GetByIdAsync(identifier, It.IsAny<CancellationToken>()), Times.Once);
    }
}