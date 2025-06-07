using MotoHub.Application.DTOs;
using MotoHub.Application.Interfaces;
using MotoHub.Application.Interfaces.UseCases;
using MotoHub.Domain.Common;
using MotoHub.Domain.Entities;

namespace MotoHub.Application.UseCases;

public class SearchMotorcyclesUseCase(IMotorcycleRepository motorcycleRepository) : ISearchMotorcyclesUseCase
{
    public async Task<Result<List<MotorcycleDto>>> ExecuteAsync(MotorcycleSearchParameters dto, CancellationToken cancellationToken = default)
    {
        if (dto.Offset < 0 || dto.Limit <= 0)
        {
            return Result<List<MotorcycleDto>>.Failure("Offset and limit must be greater than or equal to zero.", ResultErrorType.ValidationError);
        }

        if (dto.Limit > 100)
        {
            return Result<List<MotorcycleDto>>.Failure("Limit cannot exceed 100.", ResultErrorType.ValidationError);
        }

        List<Motorcycle> motorcycleList = await motorcycleRepository.SearchAsync(dto, cancellationToken);

        List<MotorcycleDto> dtoList = [.. motorcycleList.Select(ToDto)];

        return Result<List<MotorcycleDto>>.Success(dtoList);
    }

    private static MotorcycleDto ToDto(Motorcycle motorcycle)
    {
        return new MotorcycleDto
        {
            Identifier = motorcycle.Identifier,
            Year = motorcycle.Year,
            Plate = motorcycle.Plate,
            Model = motorcycle.Model,
        };
    }
}