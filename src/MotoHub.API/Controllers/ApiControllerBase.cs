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
        return result.ErrorType switch
        {
            ResultErrorType.NotFound => NotFound(result.Error),
            ResultErrorType.ValidationError => BadRequest(result.Error),
            ResultErrorType.Unauthorized => Unauthorized(result.Error),
            _ => StatusCode(StatusCodes.Status500InternalServerError, result.Error)
        };
    }
}
