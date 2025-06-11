using MotoHub.Application.DTOs;
using MotoHub.Application.Interfaces.Repositories;
using MotoHub.Application.UseCases.Renting;
using MotoHub.Domain.Common;
using MotoHub.Domain.Entities;
using MotoHub.Domain.Interfaces;

namespace MotoHub.Tests.UseCases.Renting;

[TestFixture]
public class ReturnMotorcycleUseCaseTests
{
    private Mock<IRentRepository> _rentRepositoryMock;
    private Mock<IRentPricingCalculator> _pricingCalculatorMock;
    private ReturnMotorcycleUseCase _useCase;

    [SetUp]
    public void Setup()
    {
        _rentRepositoryMock = new Mock<IRentRepository>();
        _pricingCalculatorMock = new Mock<IRentPricingCalculator>();
        _useCase = new ReturnMotorcycleUseCase(_rentRepositoryMock.Object, _pricingCalculatorMock.Object);
    }

    [Test]
    public async Task ExecuteAsync_WithNonExistingRent_ShouldReturnNotFoundError()
    {
        ReturnMotorcycleDto dto = new()
        {
            RentIdentifier = "rent-001",
            ReturnDate = DateTime.UtcNow
        };

        _rentRepositoryMock.Setup(r => r.GetByIdentifierAsync(dto.RentIdentifier, It.IsAny<CancellationToken>()))
                           .ReturnsAsync((Rent?)null);

        Result<CompletedRentalDto> result = await _useCase.ExecuteAsync(dto);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorType, Is.EqualTo(ResultErrorType.NotFound));
            Assert.That(result.ErrorMessage, Is.EqualTo("Aluguel não encontrado"));
        });
    }

    [Test]
    public async Task ExecuteAsync_WithAlreadyCompletedRent_ShouldReturnBusinessError()
    {
        ReturnMotorcycleDto dto = new()
        {
            RentIdentifier = "rent-001",
            ReturnDate = DateTime.UtcNow
        };

        Rent rent = new()
        {
            Identifier = dto.RentIdentifier,
            Status = RentStatus.Completed
        };

        _rentRepositoryMock.Setup(r => r.GetByIdentifierAsync(dto.RentIdentifier, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(rent);

        Result<CompletedRentalDto> result = await _useCase.ExecuteAsync(dto);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorType, Is.EqualTo(ResultErrorType.BusinessError));
            Assert.That(result.ErrorMessage, Is.EqualTo("Este aluguel já foi encerrado"));
        });
    }

    [Test]
    public async Task ExecuteAsync_WithInvalidReturnDate_ShouldReturnBusinessError()
    {
        ReturnMotorcycleDto dto = new()
        {
            RentIdentifier = "rent-001",
            ReturnDate = DateTime.UtcNow.AddDays(-1) // Data de devolução anterior a data de início
        };

        Rent rent = new()
        {
            Identifier = dto.RentIdentifier,
            Status = RentStatus.Active,
            StartDate = DateTime.UtcNow
        };
        _rentRepositoryMock.Setup(r => r.GetByIdentifierAsync(dto.RentIdentifier, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(rent);

        Result<CompletedRentalDto> result = await _useCase.ExecuteAsync(dto);
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorType, Is.EqualTo(ResultErrorType.BusinessError));
            Assert.That(result.ErrorMessage, Is.EqualTo("Data de devolução inválida"));
        });
    }

    [Test]
    public async Task ExecuteAsync_WithValidData_ShouldReturnCompletedRentalSuccessfully()
    {
        ReturnMotorcycleDto dto = new()
        {
            RentIdentifier = "rent-001",
            ReturnDate = DateTime.UtcNow
        };

        Rent rent = new()
        {
            Identifier = dto.RentIdentifier,
            MotorcycleIdentifier = "moto-001",
            CourierIdentifier = "courier-001",
            StartDate = DateTime.UtcNow.AddDays(-7),
            EstimatedEndDate = DateTime.UtcNow.AddDays(2),
            Status = RentStatus.Active,
            DailyRate = 150.00m
        };

        _rentRepositoryMock.Setup(r => r.GetByIdentifierAsync(dto.RentIdentifier, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(rent);
        _pricingCalculatorMock.Setup(p => p.CalculateRentalCost(rent, dto.ReturnDate))
                              .Returns(1050.00m);

        Result<CompletedRentalDto> result = await _useCase.ExecuteAsync(dto);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Data?.Identifier, Is.EqualTo(rent.Identifier));
            Assert.That(result.Data?.MotorcycleIdentifier, Is.EqualTo(rent.MotorcycleIdentifier));
            Assert.That(result.Data?.CourierIdentifier, Is.EqualTo(rent.CourierIdentifier));
            Assert.That(result.Data?.StartDate, Is.EqualTo(rent.StartDate));
            Assert.That(result.Data?.EndDate, Is.EqualTo(dto.ReturnDate));
            Assert.That(result.Data?.TotalCost, Is.EqualTo(1050.00m));
            Assert.That(result.Data?.Status, Is.EqualTo(RentStatus.Completed));
        });

        _rentRepositoryMock.Verify(r => r.UpdateAsync(rent, It.IsAny<CancellationToken>()), Times.Once);
    }
}