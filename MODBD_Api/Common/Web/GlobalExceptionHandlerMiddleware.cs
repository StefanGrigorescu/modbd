using MODBD_Api.Common.Abstractions.DomainExceptions;
using MODBD_Api.Common.Collections;
using MODBD_Api.Common.Contracts;
using MODBD_Api.Identity;
using System.Net;

namespace MODBD_Api.Common.Web;

public sealed class GlobalExceptionHandlerMiddleware : IMiddleware
{
    private readonly ILogger _logger;

    public GlobalExceptionHandlerMiddleware(ILogger<GlobalExceptionHandlerMiddleware> logger)
    {
        _logger = logger;            
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (ValueObjectException ex)
        {
            await HandleValueObjectException(context, ex);
        }
        catch (DomainObjectException ex)
        {
            await HandleDomainObjectException(context, ex);
        }
        catch (OperationCanceledException ex)
        {
            await HandleOperationCanceledException(context, ex);
        }
        catch(UnauthorizedResourceOperationException ex)
        {
            await HandleUnauthorizedResourceAccessException(context, ex);
        }
        catch (Exception ex)
        {
            await HandleException(context, ex);
        }
    }

    private async Task HandleValueObjectException(HttpContext context, ValueObjectException ex)
    {
        ex.ThrownBy ??= ex.TargetSite?.Name ?? "";
        _logger.LogWarning(ex, "{Caller} threw", ex.ThrownBy);

        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

        ApiProblemDetails problemDetails = new()
        {
            Status = (int)HttpStatusCode.BadRequest,
            Type = ex.GetType().Name,
            Title = "Request Validation Failure",
            Detail = ex.Message,
            ErrorMessages = [ex.Message],
            ErrorMetadata = ex.ErrorMetadata,
        };

        context.Response.ContentType = "application/json";

        await context.Response.WriteAsJsonAsync(problemDetails);
    }

    private async Task HandleDomainObjectException(HttpContext context, DomainObjectException ex)
    {
        ex.ThrownBy ??= ex.TargetSite?.Name ?? "";
        _logger.LogWarning(ex, "{Caller} threw", ex.ThrownBy);

        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

        ApiProblemDetails problemDetails = new()
        {
            Status = (int)HttpStatusCode.BadRequest,
            Type = ex.GetType().Name,
            Title = "Request Validation Failure",
            Detail = ex.Message,
            ErrorMessages = [ex.Message],
            ErrorMetadata = ex.ErrorMetadata,
        };

        context.Response.ContentType = "application/json";

        await context.Response.WriteAsJsonAsync(problemDetails);
    }

    private async Task HandleOperationCanceledException(HttpContext context, OperationCanceledException ex)
    {
        _logger.LogDebug(ex, "{Caller} threw", ex.TargetSite);

        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        ApiProblemDetails problemDetails = new()
        {
            Status = (int)HttpStatusCode.InternalServerError,
            Type = "Operation canceled",
            Title = "Client aborted request",
            Detail = "Operation was aborted due to client canceling the request.",
            ErrorMessages = ["Operation was aborted due to client canceling the request."],
            ErrorMetadata = MetadataCollection.Empty,
        };

        context.Response.ContentType = "application/json";

        await context.Response.WriteAsJsonAsync(problemDetails);
    }

    private async Task HandleUnauthorizedResourceAccessException(HttpContext context, UnauthorizedResourceOperationException ex)
    {
        _logger.LogWarning(ex, "{Caller} threw", ex.TargetSite);

        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

        ApiProblemDetails problemDetails = new()
        {
            Status = (int)HttpStatusCode.Unauthorized,
            Type = "Unauthorized",
            Title = "Unauthorized resource access",
            Detail = ex.Message,
            ErrorMessages = [ex.Message],
            ErrorMetadata = MetadataCollection.Empty,
        };

        context.Response.ContentType = "application/json";

        await context.Response.WriteAsJsonAsync(problemDetails);
    }

    private async Task HandleException(HttpContext context, Exception ex)
    {
        _logger.LogError(ex, "{Caller} threw", ex.TargetSite);

        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        ApiProblemDetails problemDetails = new()
        {
            Status = (int)HttpStatusCode.InternalServerError,
            Type = "Server error",
            Title = "Server error",
            Detail = "We have encountered an internal server error",
            ErrorMessages = ["We have encountered an internal server error"],
            ErrorMetadata = MetadataCollection.Empty,
        };

        context.Response.ContentType = "application/json";

        await context.Response.WriteAsJsonAsync(problemDetails);
    }
}
