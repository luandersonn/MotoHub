using MotoHub.Application.DTOs;
using MotoHub.Application.Interfaces.Repositories;
using MotoHub.Application.UseCases.Motorcycles;
using MotoHub.Domain.Common;
using MotoHub.Domain.Entities;

namespace MotoHub.Tests.UseCases.Motorcycles;

[TestFixture]
public class SearchMotorcyclesUseCaseTests
{
    private Mock<IMotorcycleRepository> _repositoryMock;
    private SearchMotorcyclesUseCase _useCase;

    [SetUp]
    public void Setup()
    {
        _repositoryMock = new Mock<IMotorcycleRepository>();
        _useCase = new SearchMotorcyclesUseCase(_repositoryMock.Object);
    }

    [Test]
    public async Task ExecuteAsync_WithNegativeOffset_ShouldReturnValidationError()
    {
        MotorcycleSearchParameters parameters = new()
        {
            Offset = -1,
            Limit = 10
        };

        Result<List<MotorcycleDto>> result = await _useCase.ExecuteAsync(parameters);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorType, Is.EqualTo(ResultErrorType.ValidationError));
            Assert.That(result.ErrorMessage, Is.EqualTo("Offset and limit must be greater than or equal to zero."));
        });
    }

    [Test]
    public async Task ExecuteAsync_WithZeroOrNegativeLimit_ShouldReturnValidationError()
    {
        MotorcycleSearchParameters parameters = new()
        {
            Offset = 0,
            Limit = 0
        };

        Result<List<MotorcycleDto>> result = await _useCase.ExecuteAsync(parameters);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorType, Is.EqualTo(ResultErrorType.ValidationError));
            Assert.That(result.ErrorMessage, Is.EqualTo("Offset and limit must be greater than or equal to zero."));
        });
    }

    [Test]
    public async Task ExecuteAsync_WithLimitGreaterThan100_ShouldReturnValidationError()
    {
        MotorcycleSearchParameters parameters = new MotorcycleSearchParameters
        {
            Offset = 0,
            Limit = 101
        };

        Result<List<MotorcycleDto>> result = await _useCase.ExecuteAsync(parameters);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorType, Is.EqualTo(ResultErrorType.ValidationError));
            Assert.That(result.ErrorMessage, Is.EqualTo("Limit cannot exceed 100."));
        });
    }

    [Test]
    public async Task ExecuteAsync_WithValidParameters_ShouldCallRepositoryAndReturnData()
    {
        MotorcycleSearchParameters parameters = new()
        {
            Offset = 0,
            Limit = 50
        };

        List<Motorcycle> mockMotorcycles =
        [
            new Motorcycle { Id = "1", Year = 2020, Plate = "ABC123", Model = "Sport" },
            new Motorcycle { Id = "2", Year = 2021, Plate = "XYZ789", Model = "Touring" }
        ];

        _repositoryMock.Setup(r => r.SearchAsync(parameters, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(mockMotorcycles);

        Result<List<MotorcycleDto>> result = await _useCase.ExecuteAsync(parameters);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Data?.Count, Is.EqualTo(2));
        });

        _repositoryMock.Verify(r => r.SearchAsync(parameters, It.IsAny<CancellationToken>()), Times.Once);
    }
}