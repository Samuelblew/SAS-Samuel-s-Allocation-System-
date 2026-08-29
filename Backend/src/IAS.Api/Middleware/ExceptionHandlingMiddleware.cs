using FluentValidation;
using IAS.Application.Common.Exceptions;

namespace IAS.Api.Middleware;

public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (status, title, detail) = exception switch
        {
            ValidationException validation => (
                StatusCodes.Status400BadRequest,
                "Erro de validação",
                string.Join("; ", validation.Errors.Select(e => e.ErrorMessage))),
            NotFoundException notFound => (
                StatusCodes.Status404NotFound,
                "Não encontrado",
                notFound.Message),
            ConflictException conflict => (
                StatusCodes.Status409Conflict,
                "Conflito",
                conflict.Message),
            _ => (
                StatusCodes.Status500InternalServerError,
                "Erro interno",
                "Ocorreu um erro inesperado.")
        };

        if (status == StatusCodes.Status500InternalServerError)
            logger.LogError(exception, "Erro não tratado");

        context.Response.StatusCode = status;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsJsonAsync(new
        {
            type = "https://tools.ietf.org/html/rfc7807",
            title,
            status,
            detail
        });
    }
}
