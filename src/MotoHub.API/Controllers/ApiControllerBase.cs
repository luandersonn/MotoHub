using Microsoft.AspNetCore.Mvc;
using MotoHub.Domain.Common;

namespace MotoHub.API.Controllers;

[ApiController]
public class ApiControllerBase : ControllerBase
{
    protected virtual IActionResult HandleResult(Result result)
    {
        return result.IsSuccess
            ? Ok()
            : HandleError(result);
    }

    protected virtual IActionResult HandleResult<T>(Result<T> result)
    {
        return result.IsSuccess
            ? Ok(result.Value)
            : HandleError(result);
    }

    protected virtual IActionResult HandleError(Result result)
    {
        var resultObject = new
        {
            mensagem = result.Error
        };

        return result.ErrorType switch
        {
            ResultErrorType.NotFound => NotFound(resultObject),
            ResultErrorType.ValidationError => BadRequest(resultObject),
            ResultErrorType.Unauthorized => Unauthorized(resultObject),
            _ => StatusCode(StatusCodes.Status500InternalServerError, resultObject)
        };
    }
}
