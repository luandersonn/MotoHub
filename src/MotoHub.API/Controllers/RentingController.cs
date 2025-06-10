using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MotoHub.API.Extensions;
using MotoHub.API.Requests;
using MotoHub.API.Responses;
using MotoHub.Application.DTOs;
using MotoHub.Application.Interfaces.UseCases.Renting;
using MotoHub.Domain.Common;

namespace MotoHub.API.Controllers;

[Route("locacao")]
[Tags("Locação")]
public class RentingController(IMapper mapper, ILogger<RentingController> logger) : ApiControllerBase
{
    [HttpPost]
    [EndpointSummary("Alugar uma moto")]
    [EndpointDescription("Registra um novo aluguel de moto no sistema")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(RentDetailsResponse), StatusCodes.Status201Created, "application/json")]
    public async Task<IActionResult> RentMotorcycle([FromServices] IRentMotorcycleUseCase useCase,
                                                    [FromBody] RentMotorcycleRequest rentMotorcycleRequest,
                                                    CancellationToken cancellationToken)
    {
        logger.LogInformation("Registering new motorcycle rent");

        RentMotorcycleDto dto = mapper.Map<RentMotorcycleDto>(rentMotorcycleRequest);

        Result<RentDetailsResponse> result = await useCase.ExecuteAsync(dto, cancellationToken)
                                                          .MapResultTo(mapper.Map<RentDetailsResponse>);

        return result.IsSuccess ? Created($"{Request.Path}/{result.Data!.Identifier}", result.Data) : HandleError(result);
    }

    [HttpGet("{id}")]
    [EndpointSummary("Consultar locação por id")]
    [EndpointDescription("Retorna os detalhes de um aluguel específico baseado no identificador")]
    [ProducesResponseType(typeof(RentDetailsResponse), StatusCodes.Status200OK, "application/json")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdentifier([FromServices] IGetRentByIdentifierUseCase useCase,
                                                     [FromRoute] string id,
                                                     CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching rent details for ID: {Id}", id);

        Result<RentDetailsResponse> result = await useCase.ExecuteAsync(id, cancellationToken)
                                                          .MapResultTo(mapper.Map<RentDetailsResponse>);

        return HandleResult(result);
    }

    [HttpPut("{id}/devolucao")]
    [EndpointSummary("Informar data de devolução e calcular valor")]
    [EndpointDescription("Define a data em que a moto foi devolvida no sistema e calcula o custo do aluguel")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(CompletedRentalResponse), StatusCodes.Status200OK, "application/json")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ReturnMotorcycle([FromServices] IReturnMotorcycleUseCase useCase,
                                                      [FromRoute] string id,
                                                      [FromBody] ReturnMotorcycleRequest returnMotorcycleRequest,
                                                      CancellationToken cancellationToken)
    {
        logger.LogInformation("Processing motorcycle return for rent ID: {Id}", id);

        ReturnMotorcycleDto dto = new()
        {
            RentIdentifier = id,
            ReturnDate = returnMotorcycleRequest.ReturnDate
        };

        Result<CompletedRentalResponse> result = await useCase.ExecuteAsync(dto, cancellationToken)
                                                              .MapResultTo(mapper.Map<CompletedRentalResponse>);

        return HandleResult(result);
    }

}