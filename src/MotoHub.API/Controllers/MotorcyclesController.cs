using Microsoft.AspNetCore.Mvc;
using MotoHub.Application.DTOs;
using MotoHub.Application.Interfaces.UseCases;
using MotoHub.Domain.Common;
using MotoHub.Domain.Entities;

namespace MotoHub.API.Controllers;

[Route("Motos")]
public class MotorcyclesController(ILogger<MotorcyclesController> logger) : ApiControllerBase
{
    [HttpGet]
    [EndpointSummary("Consultar motos existentes")]
    [EndpointDescription("Consulta e retorna as motos existentes através da placa")]
    [ProducesResponseType(typeof(IEnumerable<Motorcycle>), StatusCodes.Status200OK, "application/json")]
    public async Task<IActionResult> GetAsync([FromServices] ISearchMotorcyclesUseCase useCase,
                                              [FromQuery] string? plate,
                                              CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching motorcycles data.");

        MotorcycleSearchParametersDTO queryDTO = new()
        {
            Plate = plate,
            Offset = 0,
            Limit = 100,
        };

        Result<List<MotorcycleDto>> result = await useCase.ExecuteAsync(queryDTO, cancellationToken);

        return HandleResult(result);
    }
}