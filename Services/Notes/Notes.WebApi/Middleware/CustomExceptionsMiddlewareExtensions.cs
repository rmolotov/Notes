namespace Notes.WebApi.Middleware;

public static class CustomExceptionsMiddlewareExtensions
{
    public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder builder) => 
        builder.UseMiddleware<CustomExceptionHandlerMiddleware>();
}