using MotoHub.Application.DTOs;
using MotoHub.Application.Interfaces;
using MotoHub.Application.Interfaces.Repositories;
using MotoHub.Application.Interfaces.UseCases.Couriers;
using MotoHub.Domain.Common;
using MotoHub.Domain.Entities;
using MotoHub.Domain.ValueObjects;

namespace MotoHub.Application.UseCases.Couriers;

public class RegisterCourierUseCase(IUserRepository userRepository, IImageStorage imageStorage) : IRegisterCourierUseCase
{
    public async Task<Result<CourierDto>> ExecuteAsync(RegisterCourierDto dto, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(dto.Identifier))
        {
            return Result<CourierDto>.Failure("Identificador inválido", ResultErrorType.ValidationError);
        }

        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            return Result<CourierDto>.Failure("Nome inválido", ResultErrorType.ValidationError);
        }

        if (string.IsNullOrWhiteSpace(dto.TaxNumber) || dto.TaxNumber.Length != 14)
        {
            return Result<CourierDto>.Failure("CNPJ inválido", ResultErrorType.ValidationError);
        }

        if (dto.BirthDate >= DateTime.UtcNow.AddYears(-18))
        {
            return Result<CourierDto>.Failure("O entregador deve ter pelo menos 18 anos", ResultErrorType.ValidationError);
        }

        User? user = await userRepository.GetByIdAsync(dto.Identifier, cancellationToken);

        if (user is not null)
        {
            return Result<CourierDto>.Failure("Já existe um usuário com este identificador no sistema", ResultErrorType.BusinessError);
        }

        user = await userRepository.GetUserByTaxNumberAsync(dto.TaxNumber, cancellationToken);

        if (user is not null)
        {
            return Result<CourierDto>.Failure("Já existe um usuário com este CNPJ", ResultErrorType.BusinessError);
        }

        user = await userRepository.GetUserByLicenseNumberAsync(dto.DriverLicenseNumber, cancellationToken);

        if (user is not null)
        {
            return Result<CourierDto>.Failure("Já existe um usuário com esta CNH", ResultErrorType.BusinessError);
        }

        string imageIdentifier = await imageStorage.UploadImageAsBase64Async(dto.DriverLicenseImageBase64, cancellationToken);

        user = new()
        {
            Id = dto.Identifier,
            Name = dto.Name,
            TaxNumber = dto.TaxNumber,
            BirthDate = dto.BirthDate,
            DriverLicenseNumber = dto.DriverLicenseNumber,
            DriverLicenseType = dto.DriverLicenseType,
            DriverLicenseImageIdentifier = imageIdentifier,
            Role = UserRole.Courier,
        };

        await userRepository.AddAsync(user, cancellationToken);

        CourierDto resultDto = new()
        {
            Identifier = user.Id,
            Name = user.Name,
            TaxNumber = user.TaxNumber,
            BirthDate = DateOnly.FromDateTime(user.BirthDate),
            DriverLicenseNumber = user.DriverLicenseNumber,
            DriverLicenseType = user.DriverLicenseType,
            DriverLicenseImageBase64 = dto.DriverLicenseImageBase64,
        };

        return Result<CourierDto>.Success(resultDto);
    }
}
