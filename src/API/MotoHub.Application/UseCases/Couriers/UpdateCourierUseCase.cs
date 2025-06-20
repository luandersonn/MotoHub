﻿using MotoHub.Application.DTOs;
using MotoHub.Application.Interfaces;
using MotoHub.Application.Interfaces.Repositories;
using MotoHub.Application.Interfaces.UseCases.Couriers;
using MotoHub.Domain.Common;
using MotoHub.Domain.Entities;

namespace MotoHub.Application.UseCases.Couriers;

public class UpdateCourierUseCase(IUserRepository userRepository, IImageStorage imageStorage) : IUpdateCourierUseCase
{
    public async Task<Result<CourierDto>> ExecuteAsync(string identifier, UpdateCourierDto dto, CancellationToken cancellationToken = default)
    {
        User? user = await userRepository.GetByIdAsync(identifier, cancellationToken);

        if (user is null)
        {
            return Result<CourierDto>.Failure("Usuário não encontrado", ResultErrorType.NotFound);
        }

        if (!string.IsNullOrWhiteSpace(dto.Name))
        {
            user.Name = dto.Name;
        }

        if (!string.IsNullOrWhiteSpace(dto.TaxNumber) && dto.TaxNumber.Length == 14)
        {
            user.TaxNumber = dto.TaxNumber;
        }

        if (dto.BirthDate.HasValue && dto.BirthDate.Value < DateTime.UtcNow.AddYears(-18))
        {
            user.BirthDate = dto.BirthDate.Value;
        }

        if (!string.IsNullOrWhiteSpace(dto.DriverLicenseNumber))
        {
            if (await IsDriverLicenseNumberInUseAsync(dto.DriverLicenseNumber, identifier, cancellationToken))
            {
                return Result<CourierDto>.Failure("Já existe um usuário com este número de CNH", ResultErrorType.BusinessError);
            }

            user.DriverLicenseNumber = dto.DriverLicenseNumber;
        }

        if (dto.DriverLicenseType is not null)
        {
            user.DriverLicenseType = dto.DriverLicenseType;
        }

        if (!string.IsNullOrWhiteSpace(dto.DriverLicenseImageBase64))
        {
            await imageStorage.RemoveAsync(user.DriverLicenseImageIdentifier, cancellationToken);
            user.DriverLicenseImageIdentifier = await imageStorage.UploadImageAsBase64Async(dto.DriverLicenseImageBase64, cancellationToken);
        }

        user.UpdatedAt = DateTime.UtcNow;

        await userRepository.UpdateAsync(user, cancellationToken);

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

    private async Task<bool> IsDriverLicenseNumberInUseAsync(string licenseNumber, string identifier, CancellationToken cancellationToken)
    {
        User? userWithSameDriverLicense = await userRepository.GetUserByLicenseNumberAsync(licenseNumber, cancellationToken);
        return userWithSameDriverLicense is not null && userWithSameDriverLicense.Id != identifier;
    }
}