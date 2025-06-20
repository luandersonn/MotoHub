﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MotoHub.API.Requests;
using MotoHub.Application.DTOs;
using MotoHub.Application.Interfaces.UseCases.Couriers;
using MotoHub.Domain.Common;

namespace MotoHub.API.Controllers;

[Route("entregadores")]
[Tags("Entregadores")]
public class CourierController(ILogger<CourierController> logger, IMapper mapper) : ApiControllerBase
{
    [HttpPost]
    [EndpointSummary("Cadastrar um novo entregador")]
    [EndpointDescription("Registra um novo entregador no sistema para realizar entregas")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(CourierDto), StatusCodes.Status201Created, "application/json")]
    public async Task<IActionResult> Register([FromServices] IRegisterCourierUseCase useCase,
                                              [FromBody] RegisterCourierRequest registerCourierRequest,
                                              CancellationToken cancellationToken)
    {
        logger.LogInformation("Registering new courier");

        RegisterCourierDto dto = mapper.Map<RegisterCourierDto>(registerCourierRequest);

        Result<CourierDto> result = await useCase.ExecuteAsync(dto, cancellationToken);

        return result.IsSuccess ? Created() : HandleError(result);
    }

    [HttpPost("{id}/cnh")]
    [EndpointSummary("Enviar foto da CNH")]
    [EndpointDescription("Atualiza a foto da CNH de um entregador no sistema com base no identificador")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(CourierDto), StatusCodes.Status200OK, "application/json")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromServices] IUpdateCourierUseCase useCase,
                                            [FromRoute] string id,
                                            [FromBody] UpdateCourierRequest updateCourierRequest,
                                            CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating courier with identifier: {Identifier}", id);

        UpdateCourierDto dto = mapper.Map<UpdateCourierDto>(updateCourierRequest);

        Result<CourierDto> result = await useCase.ExecuteAsync(id, dto, cancellationToken);

        return result.IsSuccess ? Ok(new
        {
            mensagem = "Entregador atualizado com sucesso",
        }) : HandleError(result);
    }

}
