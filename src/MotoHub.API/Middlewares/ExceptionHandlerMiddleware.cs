namespace MotoHub.API.Middlewares;

public class ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            logger.LogWarning("Request was cancelled by the client.");

            context.Response.StatusCode = StatusCodes.Status408RequestTimeout;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsJsonAsync(new { message = "Request was cancelled." });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled exception occurred while processing the request.");

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            await context.Response.WriteAsJsonAsync(new
            {
                messagem = "An unexpected error occurred."
            });
        }
    }
}
