using Microsoft.AspNetCore.Mvc;
using MotoHub.API.Requests;
using MotoHub.Application.DTOs;
using MotoHub.Application.Interfaces.UseCases.Renting;
using MotoHub.Domain.Common;

namespace MotoHub.API.Controllers;

[Route("locacao")]
public class RentingController(ILogger<RentingController> logger) : ApiControllerBase
{
    [HttpPost]
    [EndpointSummary("Alugar uma moto")]
    [EndpointDescription("Registra um novo aluguel de moto no sistema")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(RentDto), StatusCodes.Status201Created, "application/json")]
    public async Task<IActionResult> RentMotorcycle([FromServices] IRentMotorcycleUseCase useCase,
                                                    [FromBody] RentMotorcycleRequest rentMotorcycleRequest,
                                                    CancellationToken cancellationToken)
    {
        logger.LogInformation("Registering new motorcycle rent");

        RentMotorcycleDto dto = new()
        {
            TenantIdentifier = rentMotorcycleRequest.TenantIdentifier,
            MotorcycleIdentifier = rentMotorcycleRequest.MotorcycleIdentifier,
            StartDate = rentMotorcycleRequest.StartDate,
            EndDate = rentMotorcycleRequest.EndDate,
            EstimatedEndDate = rentMotorcycleRequest.EstimatedEndDate,
            Plan = rentMotorcycleRequest.Plan
        };

        Result<RentDto> result = await useCase.ExecuteAsync(dto, cancellationToken);

        return result.IsSuccess ? Created() : HandleError(result);
    }

    [HttpGet("{id}")]
    [EndpointSummary("Consultar locação por id")]
    [EndpointDescription("Retorna os detalhes de um aluguel específico baseado no identificador")]
    [ProducesResponseType(typeof(RentDto), StatusCodes.Status200OK, "application/json")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdentifier([FromServices] IGetRentByIdentifierUseCase useCase,
                                                     [FromRoute] string id,
                                                     CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching rent details for ID: {Id}", id);

        Result<RentDto> result = await useCase.ExecuteAsync(id, cancellationToken);

        return HandleResult(result);
    }

    [HttpPut("{id}/devolucao")]
    [EndpointSummary("Informar data de devolução e calcular valor")]
    [EndpointDescription("Define a data em que a moto foi devolvida no sistema e calcula o custo do aluguel")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(RentDto), StatusCodes.Status200OK, "application/json")]
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

        Result<CompletedRentalDto> result = await useCase.ExecuteAsync(dto, cancellationToken);

        return HandleResult(result);
    }

}