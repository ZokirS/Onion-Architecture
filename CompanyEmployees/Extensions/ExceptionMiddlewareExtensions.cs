using Contracts;
using Entities.ErrorModel;
using Entities.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using System.Text.Json;

namespace CompanyEmployees.Extensions
{
    public static class ExceptionMiddlewareExtensions
    {
        public static void ConfigureExceptionHandler(this WebApplication app, ILoggerManager loggerManager)
        {
            app.UseExceptionHandler(appError =>
            appError.Run(async context =>
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "aplication/json";

                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (contextFeature != null)
                {
                    context.Response.StatusCode = contextFeature.Error switch
                    {
                        NotFoundException => StatusCodes.Status404NotFound,
                        BadRequestException => StatusCodes.Status400BadRequest,
                        ValidationAppException => StatusCodes.Status422UnprocessableEntity,
                        _ => StatusCodes.Status500InternalServerError
                    };
                    loggerManager.LogError($"Something went wrong: {contextFeature.Error}");

                    if (contextFeature.Error is ValidationAppException exception)
                    {
                        await context.Response
                        .WriteAsync(JsonSerializer.Serialize(new { exception.Errors }));
                    }
                    else
                    {
                        await context.Response.WriteAsync(new ErrorDetails()
                        {
                            StatusCode = context.Response.StatusCode,
                            Message = contextFeature.Error.Message
                        }.ToString());
                    }
                }
            }));

        }
    }
}
