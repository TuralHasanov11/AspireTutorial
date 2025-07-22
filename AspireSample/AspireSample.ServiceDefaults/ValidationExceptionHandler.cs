using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AspireSample.ServiceDefaults;

public class ValidationExceptionHandler(
    IProblemDetailsService problemDetailsService)
    : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetailsService = problemDetailsService;

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not System.ComponentModel.DataAnnotations.ValidationException and not FluentValidation.ValidationException)
        {
            return false;
        }

        Dictionary<string, string[]> errors = exception switch
        {
            FluentValidation.ValidationException fluentValidationExceptions => fluentValidationExceptions.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key.ToUpperInvariant(),
                    g => g.Select(e => e.ErrorMessage).ToArray(),
                    StringComparer.OrdinalIgnoreCase
                ),
            System.ComponentModel.DataAnnotations.ValidationException validationExceptions => validationExceptions.ValidationResult?.MemberNames
                .ToDictionary(
                    name => name.ToUpperInvariant(),
                    _ => new[] { validationExceptions.ValidationResult.ErrorMessage ?? string.Empty }
                ) ?? [],
            _ => []
        };

        var problemDetails = new ValidationProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Validation Error",
            Detail = exception.Message,
            Type = exception.GetType().Name,
            Errors = errors
        };

        return await _problemDetailsService.TryWriteAsync(
            new ProblemDetailsContext
            {
                HttpContext = httpContext,
                Exception = exception,
                ProblemDetails = problemDetails,
            });
    }
}
