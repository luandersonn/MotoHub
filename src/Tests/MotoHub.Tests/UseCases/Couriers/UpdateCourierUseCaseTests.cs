using MotoHub.Application.DTOs;
using MotoHub.Application.Interfaces;
using MotoHub.Application.Interfaces.Repositories;
using MotoHub.Application.UseCases.Couriers;
using MotoHub.Domain.Common;
using MotoHub.Domain.Entities;
using MotoHub.Domain.ValueObjects;

namespace MotoHub.Tests.UseCases.Couriers;

[TestFixture]
public class UpdateCourierUseCaseTests
{
    private Mock<IUserRepository> _userRepositoryMock;
    private Mock<IImageStorage> _imageStorageMock;
    private UpdateCourierUseCase _useCase;

    [SetUp]
    public void Setup()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _imageStorageMock = new Mock<IImageStorage>();
        _useCase = new UpdateCourierUseCase(_userRepositoryMock.Object, _imageStorageMock.Object);
    }

    [Test]
    public async Task ExecuteAsync_WithNonExistingUser_ShouldReturnNotFoundError()
    {
        string identifier = "123";
        UpdateCourierDto dto = new()
        {
            Name = "Novo Nome"
        };

        _userRepositoryMock.Setup(r => r.GetByIdentifierAsync(identifier, It.IsAny<CancellationToken>()))
                           .ReturnsAsync((User?)null);

        Result<CourierDto> result = await _useCase.ExecuteAsync(identifier, dto);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorType, Is.EqualTo(ResultErrorType.NotFound));
            Assert.That(result.ErrorMessage, Is.EqualTo("Usuário não encontrado"));
        });
    }

    [Test]
    public async Task ExecuteAsync_WithValidName_ShouldUpdateName()
    {
        string identifier = "123";
        UpdateCourierDto dto = new()
        {
            Name = "Novo Nome"
        };

        User user = new()
        {
            Identifier = identifier,
            Name = "Antigo Nome",
            TaxNumber = "12345678901234",
            BirthDate = DateTime.UtcNow.AddYears(-20),
            DriverLicenseNumber = "ABC123",
            DriverLicenseType = DriverLicenseType.A,
            DriverLicenseImageIdentifier = "image-123"
        };

        _userRepositoryMock.Setup(r => r.GetByIdentifierAsync(identifier, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(user);

        Result<CourierDto> result = await _useCase.ExecuteAsync(identifier, dto);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Data?.Name, Is.EqualTo(dto.Name));
        });

        _userRepositoryMock.Verify(r => r.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task ExecuteAsync_WithDuplicateDriverLicense_ShouldReturnBusinessError()
    {
        string identifier = "123";
        UpdateCourierDto dto = new()
        {
            DriverLicenseNumber = "ABC123"
        };

        User user = new()
        {
            Identifier = identifier,
            DriverLicenseNumber = "OLD123"
        };

        User existingUser = new()
        {
            Identifier = "456",
            DriverLicenseNumber = "ABC123"
        };

        _userRepositoryMock.Setup(r => r.GetByIdentifierAsync(identifier, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(user);
        _userRepositoryMock.Setup(r => r.GetUserByLicenseNumberAsync(dto.DriverLicenseNumber, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(existingUser);

        Result<CourierDto> result = await _useCase.ExecuteAsync(identifier, dto);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorType, Is.EqualTo(ResultErrorType.BusinessError));
            Assert.That(result.ErrorMessage, Is.EqualTo("Já existe um usuário com este número de CNH"));
        });
    }

    [Test]
    public async Task ExecuteAsync_WithUpdatedDriverLicenseImage_ShouldRemoveOldImageAndUploadNew()
    {
        string userIdentifier = "123";
        string oldImageIdentifier = "OldImage123";
        string newImageIdentifier = "NewImage456";

        UpdateCourierDto dto = new()
        {
            DriverLicenseImageBase64 = "NovaImagemBase64"
        };

        User user = new()
        {
            Identifier = userIdentifier,
            DriverLicenseImageIdentifier = oldImageIdentifier
        };

        _userRepositoryMock.Setup(r => r.GetByIdentifierAsync(userIdentifier, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(user);
        _imageStorageMock.Setup(r => r.UploadImageAsBase64Async(dto.DriverLicenseImageBase64, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(newImageIdentifier);

        Result<CourierDto> result = await _useCase.ExecuteAsync(userIdentifier, dto);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Data?.DriverLicenseImageBase64, Is.EqualTo(dto.DriverLicenseImageBase64));
        });

        _imageStorageMock.Verify(r => r.RemoveAsync(oldImageIdentifier, It.IsAny<CancellationToken>()), Times.Once);
        _imageStorageMock.Verify(r => r.UploadImageAsBase64Async(dto.DriverLicenseImageBase64, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task ExecuteAsync_WithValidData_ShouldUpdateUserSuccessfully()
    {
        string identifier = "123";
        UpdateCourierDto dto = new()
        {
            Name = "Novo Nome",
            TaxNumber = "98765432109876",
            BirthDate = DateTime.UtcNow.AddYears(-25),
            DriverLicenseNumber = "XYZ789",
            DriverLicenseType = DriverLicenseType.B,
            DriverLicenseImageBase64 = "NovaImagemBase64"
        };

        User user = new()
        {
            Identifier = identifier,
            Name = "Antigo Nome",
            TaxNumber = "12345678901234",
            BirthDate = DateTime.UtcNow.AddYears(-20),
            DriverLicenseNumber = "ABC123",
            DriverLicenseType = DriverLicenseType.A,
            DriverLicenseImageIdentifier = "OldImage123"
        };

        _userRepositoryMock.Setup(r => r.GetByIdentifierAsync(identifier, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(user);
        _userRepositoryMock.Setup(r => r.GetUserByLicenseNumberAsync(dto.DriverLicenseNumber, It.IsAny<CancellationToken>()))
                           .ReturnsAsync((User?)null);
        _imageStorageMock.Setup(r => r.UploadImageAsBase64Async(dto.DriverLicenseImageBase64, It.IsAny<CancellationToken>()))
                         .ReturnsAsync("NewImage456");

        Result<CourierDto> result = await _useCase.ExecuteAsync(identifier, dto);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Data?.Name, Is.EqualTo(dto.Name));
            Assert.That(result.Data?.TaxNumber, Is.EqualTo(dto.TaxNumber));
            Assert.That(result.Data?.DriverLicenseNumber, Is.EqualTo(dto.DriverLicenseNumber));
            Assert.That(result.Data?.DriverLicenseType, Is.EqualTo(dto.DriverLicenseType));
            Assert.That(result.Data?.DriverLicenseImageBase64, Is.EqualTo(dto.DriverLicenseImageBase64));
        });

        _userRepositoryMock.Verify(r => r.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
    }

}