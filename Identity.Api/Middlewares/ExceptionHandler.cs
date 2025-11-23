using System.Net;
using System.Text.Json;
using Identity.Api.DTO;

namespace Identity.Api.Middlewares;

public class ExceptionHandler
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandler> _logger;
    
    public ExceptionHandler(RequestDelegate next, ILogger<ExceptionHandler> logger) {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = new ResponseDto<object>
            {
                Success = false,
                Message = "An unexpected error occurred. Please try again later.",
                Data = null
            };
            
            var json = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(json);
        }
    }
}