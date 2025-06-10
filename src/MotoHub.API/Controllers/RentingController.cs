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
public class RentingController(ILogger<RentingController> logger) : ApiControllerBase
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

        RentMotorcycleDto dto = new()
        {
            Identifier = rentMotorcycleRequest.Identifier,
            CourierIdentifier = rentMotorcycleRequest.CourierIdentifier,
            MotorcycleIdentifier = rentMotorcycleRequest.MotorcycleIdentifier,
            Plan = rentMotorcycleRequest.Plan,
        };

        Result<RentDetailsResponse> result = await useCase.ExecuteAsync(dto, cancellationToken)
                                                          .MapResultTo(r => new RentDetailsResponse
                                                          {
                                                              Identifier = r.Identifier,
                                                              MotorcycleIdentifier = r.MotorcycleIdentifier,
                                                              CourierIdentifier = r.CourierIdentifier,
                                                              StartDate = r.StartDate,
                                                              EndDate = r.EndDate,
                                                              EstimatedEndDate = r.EstimatedEndDate,
                                                              DailyRate = r.DailyRate,
                                                              Status = r.Status,
                                                          });

        return result.IsSuccess ? Created($"{Request.Path}/{result.Value!.Identifier}", result.Value) : HandleError(result);
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
                                                          .MapResultTo(r => new RentDetailsResponse
                                                          {
                                                              Identifier = r.Identifier,
                                                              MotorcycleIdentifier = r.MotorcycleIdentifier,
                                                              CourierIdentifier = r.CourierIdentifier,
                                                              StartDate = r.StartDate,
                                                              EndDate = r.EndDate,
                                                              EstimatedEndDate = r.EstimatedEndDate,
                                                              DailyRate = r.DailyRate,
                                                              ReturnDate = r.EndDate,
                                                              Status = r.Status,
                                                          });

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
                                                              .MapResultTo(x => new CompletedRentalResponse
                                                              {
                                                                  Identifier = x.Identifier,
                                                                  MotorcycleIdentifier = x.MotorcycleIdentifier,
                                                                  CourierIdentifier = x.CourierIdentifier,
                                                                  StartDate = x.StartDate,
                                                                  EndDate = x.EndDate,
                                                                  TotalCost = x.TotalCost,
                                                                  Status = x.Status,
                                                              });

        return HandleResult(result);
    }

}