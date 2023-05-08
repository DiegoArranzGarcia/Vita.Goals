using FastEndpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;

internal class CustomExceptionHandler { }

public static class CustomExceptionHandlerExtension
{
    public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder app, ILogger? logger = null)
    {
        app.UseExceptionHandler(errApp =>
        {
            errApp.Run(async ctx =>
            {
                var exceptionHandler = ctx.Features.Get<IExceptionHandlerFeature>();

                if (exceptionHandler is null)
                    return;

                Exception exception = exceptionHandler.Error;
                ctx.Response.StatusCode = exception switch
                {
                    KeyNotFoundException => (int)HttpStatusCode.NotFound,
                    UnauthorizedAccessException => (int)HttpStatusCode.Forbidden,
                    _ => (int)HttpStatusCode.InternalServerError
                };

                logger ??= ctx.Resolve<ILogger<CustomExceptionHandler>>();

                var http = exceptionHandler.Endpoint?.DisplayName?.Split(" => ")[0];
                var type = exception.GetType().Name;
                var title = exception.Message;

                logger.LogError("{@http}{@type}{@reason}{@exception}", http, type, title, exception);

                ctx.Response.ContentType = "application/problem+json";

                await ctx.Response.WriteAsJsonAsync(new Microsoft.AspNetCore.Mvc.ProblemDetails()
                {
                    Status = ctx.Response.StatusCode,
                    Title = title,
                    Detail = exception.StackTrace,
                    Instance = exceptionHandler.Path,
                });
            });
        });

        return app;
    }
}