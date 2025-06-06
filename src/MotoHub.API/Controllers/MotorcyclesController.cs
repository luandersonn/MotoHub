using Microsoft.AspNetCore.Mvc;
using MotoHub.API.Requests;
using MotoHub.Application.DTOs;
using MotoHub.Application.Interfaces.UseCases;
using MotoHub.Domain.Common;
using MotoHub.Domain.Entities;

namespace MotoHub.API.Controllers;

[Route("Motos")]
public class MotorcyclesController(ILogger<MotorcyclesController> logger) : ApiControllerBase
{
    [HttpPost]
    [EndpointSummary("Cadastrar uma nova moto")]
    [EndpointDescription("Cadastrar uma nova moto no sistema e disponibilizar para aluguel")]
    [Consumes("application/json")]
    [Produces("application/json")]
    public async Task<IActionResult> Register([FromServices] IRegisterMotorcycleUseCase useCase,
                                              [FromBody] RegisterMotorcycleRequest registerMotorcycleRequest,
                                              CancellationToken cancellationToken)
    {
        RegisterMotorcycleDto dto = new()
        {
            Identifier = registerMotorcycleRequest.Identifier,
            Model = registerMotorcycleRequest.Model,
            Plate = registerMotorcycleRequest.Plate,
            Year = registerMotorcycleRequest.Year,
        };

        Result<MotorcycleDto> result = await useCase.ExecuteAsync(dto, cancellationToken);

        return result.IsSuccess ? Created() : HandleError(result);
    }

    [HttpGet]
    [EndpointSummary("Consultar motos existentes")]
    [EndpointDescription("Consulta e retorna as motos existentes através da placa")]
    [ProducesResponseType(typeof(IEnumerable<Motorcycle>), StatusCodes.Status200OK, "application/json")]
    public async Task<IActionResult> GetAsync([FromServices] ISearchMotorcyclesUseCase useCase,
                                              [FromQuery] string? plate,
                                              CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching motorcycles data.");

        MotorcycleSearchParametersDto queryDTO = new()
        {
            Plate = plate,
            Offset = 0,
            Limit = 100,
        };

        Result<List<MotorcycleDto>> result = await useCase.ExecuteAsync(queryDTO, cancellationToken);

        return HandleResult(result);
    }
}