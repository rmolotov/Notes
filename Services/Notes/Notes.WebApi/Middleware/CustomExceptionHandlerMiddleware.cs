using System.Net;
using System.Text.Json;
using FluentValidation;
using Notes.Application.Common.Exceptions;

namespace Notes.WebApi.Middleware;

public class CustomExceptionHandlerMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var result = exception switch
        {
            ValidationException validationException => (
                HttpStatusCode.BadRequest,
                JsonSerializer.Serialize(validationException.Errors)),
            NotFoundException => (
                HttpStatusCode.NotFound, 
                string.Empty),
            _ => (
                HttpStatusCode.InternalServerError, 
                string.Empty)
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)result.Item1;

        if (result.Item2 == string.Empty)
            result.Item2 = JsonSerializer.Serialize(new { errpr = exception.Message });

        return context.Response.WriteAsync(result.Item2);
    }
}