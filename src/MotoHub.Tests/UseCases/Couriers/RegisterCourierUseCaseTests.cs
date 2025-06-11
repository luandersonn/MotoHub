using MotoHub.Application.DTOs;
using MotoHub.Application.Interfaces;
using MotoHub.Application.Interfaces.Repositories;
using MotoHub.Application.UseCases.Couriers;
using MotoHub.Domain.Common;
using MotoHub.Domain.Entities;
using MotoHub.Domain.ValueObjects;

namespace MotoHub.Tests.UseCases.Couriers;

[TestFixture]
public class RegisterCourierUseCaseTests
{
    private Mock<IUserRepository> _userRepositoryMock;
    private Mock<IImageStorage> _imageStorageMock;
    private RegisterCourierUseCase _useCase;

    [SetUp]
    public void Setup()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _imageStorageMock = new Mock<IImageStorage>();
        _useCase = new RegisterCourierUseCase(_userRepositoryMock.Object, _imageStorageMock.Object);
    }

    [Test]
    public async Task ExecuteAsync_WithInvalidIdentifier_ShouldReturnValidationError()
    {
        RegisterCourierDto dto = new()
        {
            Identifier = "  ", // Identificador inv�lido
            Name = "Jo�o",
            TaxNumber = "12345678901234",
            BirthDate = DateTime.UtcNow.AddYears(-20),
            DriverLicenseNumber = "ABC123",
            DriverLicenseType = DriverLicenseType.A,
            DriverLicenseImageBase64 = "ImagemBase64"
        };

        Result<CourierDto> result = await _useCase.ExecuteAsync(dto);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorType, Is.EqualTo(ResultErrorType.ValidationError));
            Assert.That(result.ErrorMessage, Is.EqualTo("Identificador inv�lido"));
        });
    }

    [Test]
    public async Task ExecuteAsync_WithInvalidName_ShouldReturnValidationError()
    {
        RegisterCourierDto dto = new()
        {
            Identifier = "123",
            Name = "", // Nome inv�lido
            TaxNumber = "12345678901234",
            BirthDate = DateTime.UtcNow.AddYears(-20),
            DriverLicenseNumber = "ABC123",
            DriverLicenseType = DriverLicenseType.A,
            DriverLicenseImageBase64 = "ImagemBase64"
        };

        Result<CourierDto> result = await _useCase.ExecuteAsync(dto);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorType, Is.EqualTo(ResultErrorType.ValidationError));
            Assert.That(result.ErrorMessage, Is.EqualTo("Nome inv�lido"));
        });
    }

    [Test]
    public async Task ExecuteAsync_WithInvalidTaxNumber_ShouldReturnValidationError()
    {
        RegisterCourierDto dto = new()
        {
            Identifier = "123",
            Name = "Jo�o",
            TaxNumber = "12345", // CNPJ inv�lido
            BirthDate = DateTime.UtcNow.AddYears(-20),
            DriverLicenseNumber = "ABC123",
            DriverLicenseType = DriverLicenseType.A,
            DriverLicenseImageBase64 = "ImagemBase64"
        };

        Result<CourierDto> result = await _useCase.ExecuteAsync(dto);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorType, Is.EqualTo(ResultErrorType.ValidationError));
            Assert.That(result.ErrorMessage, Is.EqualTo("CNPJ inv�lido"));
        });
    }

    [Test]
    public async Task ExecuteAsync_WithUnderageCourier_ShouldReturnValidationError()
    {
        RegisterCourierDto dto = new()
        {
            Identifier = "123",
            Name = "Jo�o",
            TaxNumber = "12345678901234",
            BirthDate = DateTime.UtcNow.AddYears(-17), // Menor de 18 anos
            DriverLicenseNumber = "ABC123",
            DriverLicenseType = DriverLicenseType.A,
            DriverLicenseImageBase64 = "ImagemBase64"
        };

        Result<CourierDto> result = await _useCase.ExecuteAsync(dto);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorType, Is.EqualTo(ResultErrorType.ValidationError));
            Assert.That(result.ErrorMessage, Is.EqualTo("O entregador deve ter pelo menos 18 anos"));
        });
    }

    [Test]
    public async Task ExecuteAsync_WithExistingIdentifier_ShouldReturnBusinessError()
    {
        RegisterCourierDto dto = new()
        {
            Identifier = "123",
            Name = "Jo�o",
            TaxNumber = "12345678901234",
            BirthDate = DateTime.UtcNow.AddYears(-20),
            DriverLicenseNumber = "ABC123",
            DriverLicenseType = DriverLicenseType.A,
            DriverLicenseImageBase64 = "ImagemBase64"
        };

        User existingUser = new() { Identifier = "123" };

        _userRepositoryMock.Setup(r => r.GetByIdentifierAsync(dto.Identifier, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(existingUser);

        Result<CourierDto> result = await _useCase.ExecuteAsync(dto);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorType, Is.EqualTo(ResultErrorType.BusinessError));
            Assert.That(result.ErrorMessage, Is.EqualTo("J� existe um usu�rio com este identificador no sistema"));
        });
    }

    [Test]
    public async Task ExecuteAsync_WithValidParameters_ShouldRegisterCourierSuccessfully()
    {
        RegisterCourierDto dto = new()
        {
            Identifier = "123",
            Name = "Jo�o",
            TaxNumber = "12345678901234",
            BirthDate = DateTime.UtcNow.AddYears(-20),
            DriverLicenseNumber = "ABC123",
            DriverLicenseType = DriverLicenseType.A,
            DriverLicenseImageBase64 = "ImagemBase64"
        };

        _userRepositoryMock.Setup(r => r.GetByIdentifierAsync(dto.Identifier, It.IsAny<CancellationToken>()))
                           .ReturnsAsync((User?)null);
        _userRepositoryMock.Setup(r => r.GetUserByTaxNumberAsync(dto.TaxNumber, It.IsAny<CancellationToken>()))
                           .ReturnsAsync((User?)null);
        _userRepositoryMock.Setup(r => r.GetUserByLicenseNumberAsync(dto.DriverLicenseNumber, It.IsAny<CancellationToken>()))
                           .ReturnsAsync((User?)null);
        _imageStorageMock.Setup(r => r.UploadImageAsBase64Async(dto.DriverLicenseImageBase64, It.IsAny<CancellationToken>()))
                         .ReturnsAsync("image-123");

        Result<CourierDto> result = await _useCase.ExecuteAsync(dto);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Data?.Identifier, Is.EqualTo(dto.Identifier));
            Assert.That(result.Data?.Name, Is.EqualTo(dto.Name));
            Assert.That(result.Data?.TaxNumber, Is.EqualTo(dto.TaxNumber));
            Assert.That(result.Data?.DriverLicenseImageBase64, Is.EqualTo(dto.DriverLicenseImageBase64));
        });

        _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        _imageStorageMock.Verify(r => r.UploadImageAsBase64Async(dto.DriverLicenseImageBase64, It.IsAny<CancellationToken>()), Times.Once);
    }
}