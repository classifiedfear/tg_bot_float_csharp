using System;
using System.Net;
using CsgoDbSource.Exceptions;

namespace CsgoDbSource.CsgoDbSource.Middlewares;

public sealed class ExceptionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleException(context, ex);
        }
    }

    private static async Task HandleException(HttpContext context, Exception ex)
    {
        if (context.Response.HasStarted)
            throw ex;

        var status = ex switch
        {
            OperationCanceledException => StatusCodes.Status499ClientClosedRequest,
            PageException => StatusCodes.Status404NotFound,
            SourceStructureException => StatusCodes.Status502BadGateway,
            _ => StatusCodes.Status500InternalServerError
        };

        context.Response.Clear();
        context.Response.StatusCode = status;
        context.Response.ContentType = "application/json";

        var problem = new
        {
            type = ex.GetType().Name,
            message = ex.Message,
            status,
            path = context.Request.Path
        };

        await context.Response.WriteAsJsonAsync(problem);
    }
}
