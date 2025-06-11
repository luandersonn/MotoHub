using MotoHub.Application.DTOs;
using MotoHub.Application.Events;
using MotoHub.Application.Interfaces.Messaging;
using MotoHub.Application.Interfaces.Repositories;
using MotoHub.Application.UseCases.Motorcycles;
using MotoHub.Domain.Common;
using MotoHub.Domain.Entities;

namespace MotoHub.Tests.UseCases.Motorcycles;

[TestFixture]
public class RegisterMotorcycleUseCaseTests
{
    private Mock<IMotorcycleRepository> _repositoryMock;
    private Mock<IMotorcycleEventPublisher> _publisherMock;
    private RegisterMotorcycleUseCase _useCase;

    [SetUp]
    public void Setup()
    {
        _repositoryMock = new Mock<IMotorcycleRepository>();
        _publisherMock = new Mock<IMotorcycleEventPublisher>();
        _useCase = new RegisterMotorcycleUseCase(_repositoryMock.Object, _publisherMock.Object);
    }

    [Test]
    public async Task ExecuteAsync_WithInvalidPlate_ShouldReturnValidationError()
    {
        RegisterMotorcycleDto dto = new()
        {
            Plate = "  ", // Placa inválida
            Year = 2022,
            Model = "Sport",
            Identifier = "moto-001"
        };

        Result<MotorcycleDto> result = await _useCase.ExecuteAsync(dto);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorType, Is.EqualTo(ResultErrorType.ValidationError));
            Assert.That(result.ErrorMessage, Is.EqualTo("Placa inválida"));
        });
    }

    [Test]
    public async Task ExecuteAsync_WithInvalidYear_ShouldReturnValidationError()
    {
        RegisterMotorcycleDto dto = new()
        {
            Plate = "ABC123",
            Year = 1899, // Ano inválido
            Model = "Sport",
            Identifier = "moto-001"
        };

        Result<MotorcycleDto> result = await _useCase.ExecuteAsync(dto);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorType, Is.EqualTo(ResultErrorType.ValidationError));
            Assert.That(result.ErrorMessage, Is.EqualTo("O ano deve ser maior que 1900"));
        });
    }

    [Test]
    public async Task ExecuteAsync_WithExistingPlate_ShouldReturnBusinessError()
    {
        RegisterMotorcycleDto dto = new()
        {
            Plate = "ABC123",
            Year = 2022,
            Model = "Sport",
            Identifier = "moto-001"
        };

        Motorcycle existingMotorcycle = new() { Plate = "ABC123" };

        _repositoryMock.Setup(r => r.GetByPlateAsync(dto.Plate, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(existingMotorcycle);

        Result<MotorcycleDto> result = await _useCase.ExecuteAsync(dto);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorType, Is.EqualTo(ResultErrorType.BusinessError));
            Assert.That(result.ErrorMessage, Is.EqualTo("Já existe uma moto com esta placa registrado no sistema"));
        });
    }

    [Test]
    public async Task ExecuteAsync_WithExistingIdentifier_ShouldReturnBusinessError()
    {
        RegisterMotorcycleDto dto = new()
        {
            Plate = "XYZ789",
            Year = 2023,
            Model = "Touring",
            Identifier = "moto-001"
        };

        Motorcycle existingMotorcycle = new() { Id = "moto-001" };

        _repositoryMock.Setup(r => r.GetByIdAsync(dto.Identifier, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(existingMotorcycle);

        Result<MotorcycleDto> result = await _useCase.ExecuteAsync(dto);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorType, Is.EqualTo(ResultErrorType.BusinessError));
            Assert.That(result.ErrorMessage, Is.EqualTo("Já existe uma moto com este identificador no sistema"));
        });
    }

    [Test]
    public async Task ExecuteAsync_WithValidData_ShouldRegisterMotorcycleSuccessfully()
    {
        RegisterMotorcycleDto dto = new()
        {
            Plate = "XYZ789",
            Year = 2023,
            Model = "Touring",
            Identifier = "moto-001"
        };

        _repositoryMock.Setup(r => r.GetByPlateAsync(dto.Plate, It.IsAny<CancellationToken>()))
                       .ReturnsAsync((Motorcycle?)null);
        _repositoryMock.Setup(r => r.GetByIdAsync(dto.Identifier, It.IsAny<CancellationToken>()))
                       .ReturnsAsync((Motorcycle?)null);

        Result<MotorcycleDto> result = await _useCase.ExecuteAsync(dto);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Data?.Identifier, Is.EqualTo(dto.Identifier));
            Assert.That(result.Data?.Plate, Is.EqualTo(dto.Plate));
            Assert.That(result.Data?.Year, Is.EqualTo(dto.Year));
            Assert.That(result.Data?.Model, Is.EqualTo(dto.Model));
        });

        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Motorcycle>(), It.IsAny<CancellationToken>()), Times.Once);
        _publisherMock.Verify(r => r.PublishMotorcycleRegisteredAsync(It.IsAny<MotorcycleRegisteredEvent>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}