using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace FruitShop.Api.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, IWebHostEnvironment webHostEnvironment, ILogger<ExceptionHandlingMiddleware> logger)
{
    private const int KnownExceptionId = 4000;
    private const int UnhandledExcpetionId = 5000;
    private const string CorrelationIdHeader = "X-Correlation-Id";
    private readonly RequestDelegate _next = next;
    private readonly IWebHostEnvironment _webHostEnvironment = webHostEnvironment;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger = logger;

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            bool knownException = exception is WebException;
            HttpResponse response = context.Response;
            string userMessage = exception.InnerException != null ? exception.InnerException.Message : exception.Message;
            string correlationId = Guid.NewGuid().ToString();        
           
            response.StatusCode = knownException ? (int)((WebException)exception).StatusCode : StatusCodes.Status500InternalServerError;
            response.ContentType = "application/json";
            response.Headers.Append(CorrelationIdHeader, new StringValues(correlationId));

            if (exception is not WebException) 
                _logger.LogError(new EventId(UnhandledExcpetionId, exception.Message), exception, $"{correlationId} | {exception.Message}");

            await response.WriteAsJsonAsync(new ProblemDetails
            {
                Title = knownException ? userMessage : "An unexpected error occurred",
                Detail = knownException ? userMessage : $"Please contact FruitShop with Correlation Id: {correlationId}",
                Status = response.StatusCode
            }, CancellationToken.None);

        }
    }
}

