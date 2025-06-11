using MotoHub.Application.Interfaces.Repositories;
using MotoHub.Application.UseCases.Motorcycles;
using MotoHub.Domain.Common;
using MotoHub.Domain.Entities;

namespace MotoHub.Tests.UseCases.Motorcycles;

[TestFixture]
public class DeleteMotorcycleUseCaseTests
{
    private Mock<IMotorcycleRepository> _motorcycleRepositoryMock;
    private Mock<IRentRepository> _rentRepositoryMock;
    private DeleteMotorcycleUseCase _useCase;

    [SetUp]
    public void Setup()
    {
        _motorcycleRepositoryMock = new Mock<IMotorcycleRepository>();
        _rentRepositoryMock = new Mock<IRentRepository>();
        _useCase = new DeleteMotorcycleUseCase(_motorcycleRepositoryMock.Object, _rentRepositoryMock.Object);
    }

    [Test]
    public async Task ExecuteAsync_WithNonExistingMotorcycle_ShouldReturnNotFoundError()
    {
        string identifier = "123";

        _motorcycleRepositoryMock.Setup(r => r.GetByIdAsync(identifier, It.IsAny<CancellationToken>()))
                       .ReturnsAsync((Motorcycle?)null);

        Result result = await _useCase.ExecuteAsync(identifier);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorType, Is.EqualTo(ResultErrorType.NotFound));
            Assert.That(result.ErrorMessage, Is.EqualTo("Moto não encontrada"));
        });
    }

    [Test]
    public async Task ExecuteAsync_WithMotorcycleCurrentlyRented_ShouldReturnBusinessError()
    {
        string identifier = "123";
        Motorcycle motorcycle = new()
        {
            Id = identifier
        };
        _motorcycleRepositoryMock.Setup(r => r.GetByIdAsync(identifier, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(motorcycle);
        _rentRepositoryMock.Setup(r => r.IsMotorcycleCurrentlyRentedAsync(motorcycle.Id!, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(true);

        Result result = await _useCase.ExecuteAsync(identifier);
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorType, Is.EqualTo(ResultErrorType.BusinessError));
            Assert.That(result.ErrorMessage, Is.EqualTo("Moto está atualmente alugada"));
        });
    }

    [Test]
    public async Task ExecuteAsync_WithExistingMotorcycle_ShouldDeleteSuccessfully()
    {
        string identifier = "123";

        Motorcycle motorcycle = new()
        {
            Id = identifier
        };

        _motorcycleRepositoryMock.Setup(r => r.GetByIdAsync(identifier, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(motorcycle);

        _rentRepositoryMock.Setup(r => r.IsMotorcycleCurrentlyRentedAsync(motorcycle.Id!, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(false);

        Result result = await _useCase.ExecuteAsync(identifier);

        Assert.That(result.IsSuccess, Is.True);

        _motorcycleRepositoryMock.Verify(r => r.DeleteAsync(motorcycle.Id!, It.IsAny<CancellationToken>()), Times.Once);
    }

}