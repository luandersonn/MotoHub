using MotoHub.Application.DTOs;
using MotoHub.Application.Interfaces.Repositories;
using MotoHub.Application.UseCases.Motorcycles;
using MotoHub.Domain.Common;
using MotoHub.Domain.Entities;

namespace MotoHub.Tests.UseCases.Motorcycles;

[TestFixture]
public class UpdateMotorcycleUseCaseTests
{
    private Mock<IMotorcycleRepository> _repositoryMock;
    private UpdateMotorcycleUseCase _useCase;

    [SetUp]
    public void Setup()
    {
        _repositoryMock = new Mock<IMotorcycleRepository>();
        _useCase = new UpdateMotorcycleUseCase(_repositoryMock.Object);
    }

    [Test]
    public async Task ExecuteAsync_WithNonExistingMotorcycle_ShouldReturnNotFoundError()
    {
        string identifier = "123";
        UpdateMotorcycleDto dto = new()
        {
            Plate = "XYZ789",
            Year = 2023,
            Model = "Sport"
        };

        _repositoryMock.Setup(r => r.GetByIdentifierAsync(identifier, It.IsAny<CancellationToken>()))
                       .ReturnsAsync((Motorcycle?)null);

        Result<MotorcycleDto> result = await _useCase.ExecuteAsync(identifier, dto);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorType, Is.EqualTo(ResultErrorType.NotFound));
            Assert.That(result.ErrorMessage, Is.EqualTo("Moto não encontrada"));
        });
    }

    [Test]
    public async Task ExecuteAsync_WithDuplicatePlate_ShouldReturnBusinessError()
    {
        string identifier = "123";
        UpdateMotorcycleDto dto = new()
        {
            Plate = "ABC123"
        };

        Motorcycle motorcycle = new()
        {
            Identifier = identifier,
            Plate = "OLD123"
        };

        Motorcycle existingMotorcycle = new()
        {
            Identifier = "456",
            Plate = "ABC123"
        };

        _repositoryMock.Setup(r => r.GetByIdentifierAsync(identifier, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(motorcycle);
        _repositoryMock.Setup(r => r.GetByPlateAsync(dto.Plate, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(existingMotorcycle);

        Result<MotorcycleDto> result = await _useCase.ExecuteAsync(identifier, dto);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorType, Is.EqualTo(ResultErrorType.BusinessError));
            Assert.That(result.ErrorMessage, Is.EqualTo("Já existe uma moto com esta placa registrada no sistema"));
        });
    }

    [Test]
    public async Task ExecuteAsync_WithValidPlateUpdate_ShouldUpdatePlateSuccessfully()
    {
        string identifier = "123";
        UpdateMotorcycleDto dto = new()
        {
            Plate = "XYZ789"
        };

        Motorcycle motorcycle = new()
        {
            Identifier = identifier,
            Plate = "OLD123"
        };

        _repositoryMock.Setup(r => r.GetByIdentifierAsync(identifier, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(motorcycle);
        _repositoryMock.Setup(r => r.GetByPlateAsync(dto.Plate, It.IsAny<CancellationToken>()))
                       .ReturnsAsync((Motorcycle?)null);

        Result<MotorcycleDto> result = await _useCase.ExecuteAsync(identifier, dto);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Data?.Plate, Is.EqualTo(dto.Plate));
        });

        _repositoryMock.Verify(r => r.UpdateAsync(motorcycle, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task ExecuteAsync_WithValidYearUpdate_ShouldUpdateYearSuccessfully()
    {
        string identifier = "123";
        UpdateMotorcycleDto dto = new()
        {
            Year = 2023
        };

        Motorcycle motorcycle = new()
        {
            Identifier = identifier,
            Year = 2000
        };

        _repositoryMock.Setup(r => r.GetByIdentifierAsync(identifier, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(motorcycle);

        Result<MotorcycleDto> result = await _useCase.ExecuteAsync(identifier, dto);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Data?.Year, Is.EqualTo(dto.Year));
        });

        _repositoryMock.Verify(r => r.UpdateAsync(motorcycle, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task ExecuteAsync_WithValidModelUpdate_ShouldUpdateModelSuccessfully()
    {
        string identifier = "123";
        UpdateMotorcycleDto dto = new()
        {
            Model = "Touring"
        };

        Motorcycle motorcycle = new()
        {
            Identifier = identifier,
            Model = "Sport"
        };

        _repositoryMock.Setup(r => r.GetByIdentifierAsync(identifier, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(motorcycle);

        Result<MotorcycleDto> result = await _useCase.ExecuteAsync(identifier, dto);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Data?.Model, Is.EqualTo(dto.Model));
        });

        _repositoryMock.Verify(r => r.UpdateAsync(motorcycle, It.IsAny<CancellationToken>()), Times.Once);
    }
}