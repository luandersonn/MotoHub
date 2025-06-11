using MotoHub.Application.DTOs;
using MotoHub.Application.Interfaces.Repositories;
using MotoHub.Application.UseCases.Renting;
using MotoHub.Domain.Common;
using MotoHub.Domain.Entities;
using MotoHub.Domain.Interfaces;
using MotoHub.Domain.ValueObjects;

namespace MotoHub.Tests.UseCases.Renting;

[TestFixture]
public class RentMotorcycleUseCaseTests
{
    private Mock<IRentRepository> _rentRepositoryMock;
    private Mock<IMotorcycleRepository> _motorcycleRepositoryMock;
    private Mock<IUserRepository> _userRepositoryMock;
    private Mock<IRentPlanCatalog> _rentPlanCatalogMock;
    private RentMotorcycleUseCase _useCase;

    [SetUp]
    public void Setup()
    {
        _rentRepositoryMock = new Mock<IRentRepository>();
        _motorcycleRepositoryMock = new Mock<IMotorcycleRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _rentPlanCatalogMock = new Mock<IRentPlanCatalog>();
        _useCase = new RentMotorcycleUseCase(
            _rentRepositoryMock.Object,
            _motorcycleRepositoryMock.Object,
            _userRepositoryMock.Object,
            _rentPlanCatalogMock.Object);
    }

    [Test]
    public async Task ExecuteAsync_WithExistingRentIdentifier_ShouldReturnValidationError()
    {
        RentMotorcycleDto dto = new()
        {
            Identifier = "rent-001",
            CourierIdentifier = "courier-001",
            MotorcycleIdentifier = "moto-001",
            Plan = 1
        };

        _rentRepositoryMock.Setup(r => r.GetByIdentifierAsync(dto.Identifier, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(new Rent());

        Result<RentDto> result = await _useCase.ExecuteAsync(dto);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorType, Is.EqualTo(ResultErrorType.ValidationError));
            Assert.That(result.ErrorMessage, Is.EqualTo("Identificador de locação já existe"));
        });
    }

    [Test]
    public async Task ExecuteAsync_WithInvalidCourierIdentifier_ShouldReturnValidationError()
    {
        RentMotorcycleDto dto = new()
        {
            Identifier = "rent-001",
            CourierIdentifier = " ",
            MotorcycleIdentifier = "moto-001",
            Plan = 1
        };

        Result<RentDto> result = await _useCase.ExecuteAsync(dto);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorType, Is.EqualTo(ResultErrorType.ValidationError));
            Assert.That(result.ErrorMessage, Is.EqualTo("Identificador do locatário inválido"));
        });
    }

    [Test]
    public async Task ExecuteAsync_WithInvalidMotorcycleIdentifier_ShouldReturnValidationError()
    {
        RentMotorcycleDto dto = new()
        {
            Identifier = "rent-001",
            CourierIdentifier = "courier-001",
            MotorcycleIdentifier = " ",
            Plan = 1
        };

        Result<RentDto> result = await _useCase.ExecuteAsync(dto);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorType, Is.EqualTo(ResultErrorType.ValidationError));
            Assert.That(result.ErrorMessage, Is.EqualTo("Identificador da moto inválido"));
        });
    }

    [Test]
    public async Task ExecuteAsync_WithInvalidRentPlan_ShouldReturnValidationError()
    {
        RentMotorcycleDto dto = new()
        {
            Identifier = "rent-001",
            CourierIdentifier = "courier-001",
            MotorcycleIdentifier = "moto-001",
            Plan = 99
        };

        _rentPlanCatalogMock.Setup(r => r.FindPlanByNumber(dto.Plan))
                            .Returns((RentPlan?)null);

        Result<RentDto> result = await _useCase.ExecuteAsync(dto);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorType, Is.EqualTo(ResultErrorType.ValidationError));
            Assert.That(result.ErrorMessage, Is.EqualTo("Plano de aluguel inválido"));
        });
    }

    [Test]
    public async Task ExecuteAsync_WithExistingActiveRent_ShouldReturnBusinessError()
    {
        RentMotorcycleDto dto = new()
        {
            Identifier = "rent-001",
            CourierIdentifier = "courier-001",
            MotorcycleIdentifier = "moto-001",
            Plan = 1
        };


        _rentRepositoryMock.Setup(r => r.GetActiveRentByMotorcycleAsync(dto.MotorcycleIdentifier, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(new Rent());

        _rentPlanCatalogMock.Setup(r => r.FindPlanByNumber(dto.Plan))
                            .Returns(new RentPlan()
                            {
                                DailyRate = 0,
                                DurationInDays = 0,
                                EarlyReturnDailyPenalty = 0,
                                LateReturnDailyFee = 0,
                                PlanNumber = 0
                            });

        _motorcycleRepositoryMock.Setup(r => r.GetByIdentifierAsync(dto.MotorcycleIdentifier, It.IsAny<CancellationToken>()))
                                 .ReturnsAsync(new Motorcycle());

        _userRepositoryMock.Setup(r => r.GetByIdentifierAsync(dto.CourierIdentifier, It.IsAny<CancellationToken>()))
                            .ReturnsAsync(new User { DriverLicenseType = DriverLicenseType.A });

        Result<RentDto> result = await _useCase.ExecuteAsync(dto);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorType, Is.EqualTo(ResultErrorType.BusinessError));
            Assert.That(result.ErrorMessage, Is.EqualTo("Esta moto já está alugada"));
        });
    }

    [Test]
    public async Task ExecuteAsync_WithValidData_ShouldRegisterRentSuccessfully()
    {
        RentMotorcycleDto dto = new()
        {
            Identifier = "rent-001",
            CourierIdentifier = "courier-001",
            MotorcycleIdentifier = "moto-001",
            Plan = 1
        };

        RentPlan plan = new()
        {
            PlanNumber = dto.Plan,
            DailyRate = 150.00m,
            DurationInDays = 7,
            EarlyReturnDailyPenalty = 30.00m,
            LateReturnDailyFee = 20.00m
        };

        _rentRepositoryMock.Setup(r => r.GetByIdentifierAsync(dto.Identifier, It.IsAny<CancellationToken>()))
                           .ReturnsAsync((Rent?)null);
        _rentRepositoryMock.Setup(r => r.GetActiveRentByMotorcycleAsync(dto.MotorcycleIdentifier, It.IsAny<CancellationToken>()))
                           .ReturnsAsync((Rent?)null);
        _motorcycleRepositoryMock.Setup(r => r.GetByIdentifierAsync(dto.MotorcycleIdentifier, It.IsAny<CancellationToken>()))
                                  .ReturnsAsync(new Motorcycle());
        _userRepositoryMock.Setup(r => r.GetByIdentifierAsync(dto.CourierIdentifier, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(new User { DriverLicenseType = DriverLicenseType.A });
        _rentPlanCatalogMock.Setup(r => r.FindPlanByNumber(dto.Plan))
                            .Returns(plan);

        Result<RentDto> result = await _useCase.ExecuteAsync(dto);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Data?.Identifier, Is.EqualTo(dto.Identifier));
            Assert.That(result.Data?.MotorcycleIdentifier, Is.EqualTo(dto.MotorcycleIdentifier));
            Assert.That(result.Data?.CourierIdentifier, Is.EqualTo(dto.CourierIdentifier));
            Assert.That(result.Data?.Plan, Is.EqualTo(plan.PlanNumber));
            Assert.That(result.Data?.DailyRate, Is.EqualTo(plan.DailyRate));
        });

        _rentRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Rent>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}