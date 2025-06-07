using MotoHub.Application.DTOs;
using MotoHub.Application.Interfaces;
using MotoHub.Application.Interfaces.UseCases.Couriers;
using MotoHub.Domain.Common;
using MotoHub.Domain.Entities;
using MotoHub.Domain.ValueObjects;

namespace MotoHub.Application.UseCases;

public class RegisterCourierUseCase(IUserRepository userRepository) : IRegisterCourierUseCase
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

        User? user = await userRepository.GetByIdentifierAsync(dto.Identifier, cancellationToken);

        if (user is not null)
        {
            return Result<CourierDto>.Failure("Já existe um usuário com este identificador no sistema", ResultErrorType.BusinessError);
        }

        user = new()
        {
            Identifier = dto.Identifier,
            Name = dto.Name,
            TaxNumber = dto.TaxNumber,
            BirthDate = dto.BirthDate,
            //DriverLicenseNumber = dto.DriverLicenseNumber,
            //DriverLicenseType = dto.DriverLicenseType,
            //DriverLicenseImage = dto.DriverLicenseImage,
            Role = UserRole.Courier,
            PasswordHash = string.Empty,
        };

        await userRepository.AddAsync(user, cancellationToken);

        CourierDto resultDto = new()
        {
            Identifier = user.Identifier,
            Name = user.Name,
            TaxNumber = user.TaxNumber,
            BirthDate = DateOnly.FromDateTime(user.BirthDate),
            //DriverLicenseNumber = user.DriverLicenseNumber,
            //DriverLicenseType = user.DriverLicenseType,
            //DriverLicenseImage = user.DriverLicenseImage
        };

        return Result<CourierDto>.Success(resultDto);
    }
}
