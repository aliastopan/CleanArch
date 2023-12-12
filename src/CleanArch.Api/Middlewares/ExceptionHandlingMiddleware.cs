using System.Net;
using System.Text.Json;
using System.Diagnostics;

namespace CleanArch.Api.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch(Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var exceptionDetails = new
            {
                title = exception.GetType().Name,
                code = context.Response.StatusCode,
                source = exception.Source,
                detail = exception.Message,
                stackTrace = exception.Demystify().StackTrace!
                    .Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries)
            };

            var json = JsonSerializer.Serialize(exceptionDetails, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            await context.Response.WriteAsync(json);
        }
    }
}
