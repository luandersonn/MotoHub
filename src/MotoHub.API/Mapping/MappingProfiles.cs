using AutoMapper;
using MotoHub.API.Requests;
using MotoHub.API.Responses;
using MotoHub.Application.DTOs;

namespace MotoHub.API.Mapping;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        // Courier
        CreateMap<RegisterCourierRequest, RegisterCourierDto>();
        CreateMap<UpdateCourierRequest, UpdateCourierDto>();

        // Motorcycle
        CreateMap<RegisterMotorcycleRequest, RegisterMotorcycleDto>();
        CreateMap<MotorcycleDto, MotorcycleResponse>();
        CreateMap<UpdateMotorcycleRequest, UpdateMotorcycleDto>();

        // Rent
        CreateMap<RentMotorcycleRequest, RentMotorcycleDto>();
        CreateMap<RentDto, RentDetailsResponse>();
        CreateMap<CompletedRentalDto, CompletedRentalResponse>();
    }
}
