using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MotoHub.API.Extensions;
using MotoHub.API.Requests;
using MotoHub.API.Responses;
using MotoHub.Application.DTOs;
using MotoHub.Application.Interfaces.UseCases.Motorcycles;
using MotoHub.Domain.Common;

namespace MotoHub.API.Controllers;

[Route("motos")]
[Tags("Motos")]
public class MotorcyclesController(IMapper mapper, ILogger<MotorcyclesController> logger) : ApiControllerBase
{
    [HttpPost]
    [EndpointSummary("Cadastrar uma nova moto")]
    [EndpointDescription("Cadastrar uma nova moto no sistema e disponibilizar para aluguel")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> Register([FromServices] IRegisterMotorcycleUseCase useCase,
                                              [FromBody] RegisterMotorcycleRequest registerMotorcycleRequest,
                                              CancellationToken cancellationToken)
    {
        logger.LogInformation("Registering new motorcycle");

        RegisterMotorcycleDto dto = mapper.Map<RegisterMotorcycleDto>(registerMotorcycleRequest);

        Result<MotorcycleDto> result = await useCase.ExecuteAsync(dto, cancellationToken);

        return result.IsSuccess ? Created() : HandleError(result);
    }

    [HttpGet]
    [EndpointSummary("Consultar motos existentes")]
    [EndpointDescription("Consulta e retorna as motos existentes através da placa")]
    [ProducesResponseType(typeof(IEnumerable<MotorcycleResponse>), StatusCodes.Status200OK, "application/json")]
    public async Task<IActionResult> SearchAsync([FromServices] ISearchMotorcyclesUseCase useCase,
                                                 [FromQuery(Name = "placa")] string? plate,
                                                 CancellationToken cancellationToken)
    {
        logger.LogInformation("Searching motorcycles");

        MotorcycleSearchParameters queryDTO = new()
        {
            Plate = plate,
            Offset = 0,
            Limit = 100,
        };

        Result<List<MotorcycleResponse>> result = await useCase.ExecuteAsync(queryDTO, cancellationToken)
                                                               .MapResultTo((MotorcycleDto m) => mapper.Map<MotorcycleResponse>(m));
        return HandleResult(result);
    }

    [HttpPut("{id}/placa")]
    [EndpointSummary("Modificar a placa de uma moto")]
    [EndpointDescription("Atualiza parcialmente os dados de uma moto no sistema com base no identificador")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(MotorcycleDto), StatusCodes.Status200OK, "application/json")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromServices] IUpdateMotorcycleUseCase useCase,
                                            [FromRoute] string id,
                                            [FromBody] UpdateMotorcycleRequest updateMotorcycleRequest,
                                            CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating motorcycle with identifier: {Identifier}", id);

        UpdateMotorcycleDto dto = mapper.Map<UpdateMotorcycleDto>(updateMotorcycleRequest);

        Result<MotorcycleDto> result = await useCase.ExecuteAsync(id, dto, cancellationToken);

        return result.IsSuccess ? Ok(new
        {
            mensagem = "Placa atualizada com sucesso",
        }) : HandleError(result);
    }


    [HttpGet("{id}")]
    [EndpointSummary("Consultar motos existentes por id")]
    [EndpointDescription("Consulta e retorna as motos existentes através do identificador")]
    [ProducesResponseType(typeof(MotorcycleResponse), StatusCodes.Status200OK, "application/json")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdentifier([FromServices] IGetMotorcycleByIdentifierUseCase useCase,
                                                     [FromRoute] string id,
                                                     CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching motorcycle by identifier");

        Result<MotorcycleResponse> result = await useCase.ExecuteAsync(id, cancellationToken)
                                                         .MapResultTo(mapper.Map<MotorcycleResponse>);

        return HandleResult(result);
    }

    [HttpDelete("{id}")]
    [EndpointSummary("Remover moto cadastrada")]
    [EndpointDescription("Remove uma moto pelo identificador")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteMotorcycle([FromServices] IDeleteMotorcycleUseCase useCase,
                                                      [FromRoute] string id,
                                                      CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting motorcycle by identifier");

        Result result = await useCase.ExecuteAsync(id, cancellationToken);

        return HandleResult(result);
    }

}