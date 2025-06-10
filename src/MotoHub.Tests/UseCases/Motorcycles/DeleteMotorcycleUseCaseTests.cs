using MotoHub.Application.Interfaces.Repositories;
using MotoHub.Application.UseCases.Motorcycles;
using MotoHub.Domain.Common;
using MotoHub.Domain.Entities;

namespace MotoHub.Tests.UseCases.Motorcycles;

[TestFixture]
public class DeleteMotorcycleUseCaseTests
{
    private Mock<IMotorcycleRepository> _repositoryMock;
    private DeleteMotorcycleUseCase _useCase;

    [SetUp]
    public void Setup()
    {
        _repositoryMock = new Mock<IMotorcycleRepository>();
        _useCase = new DeleteMotorcycleUseCase(_repositoryMock.Object);
    }

    [Test]
    public async Task ExecuteAsync_WithNonExistingMotorcycle_ShouldReturnNotFoundError()
    {
        string identifier = "123";

        _repositoryMock.Setup(r => r.GetByIdentifierAsync(identifier, It.IsAny<CancellationToken>()))
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
    public async Task ExecuteAsync_WithExistingMotorcycle_ShouldDeleteSuccessfully()
    {
        string identifier = "123";

        Motorcycle motorcycle = new()
        {
            Id = 1,
            Identifier = identifier
        };

        _repositoryMock.Setup(r => r.GetByIdentifierAsync(identifier, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(motorcycle);

        Result result = await _useCase.ExecuteAsync(identifier);

        Assert.That(result.IsSuccess, Is.True);

        _repositoryMock.Verify(r => r.DeleteAsync(motorcycle.Id!, It.IsAny<CancellationToken>()), Times.Once);
    }

}