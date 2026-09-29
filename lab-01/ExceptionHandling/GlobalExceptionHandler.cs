using FluentValidation;
using lab_01.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace lab_01.ExceptionHandling
{
    public class GlobalExceptionHandler
        : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(
            ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            int statusCode;
            string title;

            switch (exception)
            {
                case ValidationException:
                    statusCode =
                        StatusCodes.Status400BadRequest;

                    title =
                        "Помилка валідації";
                    break;


                case ArgumentException:
                    statusCode =
                        StatusCodes.Status400BadRequest;

                    title =
                        "Некоректний запит";
                    break;


                case NotFoundException:
                    statusCode =
                        StatusCodes.Status404NotFound;

                    title =
                        "Ресурс не знайдено";
                    break;


                case ForbiddenOperationException:
                    statusCode =
                        StatusCodes.Status403Forbidden;

                    title =
                        "Доступ заборонено";
                    break;


                case BookingConflictException:
                    statusCode =
                        StatusCodes.Status409Conflict;

                    title =
                        "Конфлікт бронювання";
                    break;


                case ConflictException:
                    statusCode =
                        StatusCodes.Status409Conflict;

                    title =
                        "Конфлікт даних";
                    break;


                case IdentityOperationException:
                    statusCode =
                        StatusCodes.Status400BadRequest;

                    title =
                        "Помилка операції з користувачем";
                    break;


                default:
                    statusCode =
                        StatusCodes.Status500InternalServerError;

                    title =
                        "Внутрішня помилка сервера";
                    break;
            }


            if (statusCode ==
                StatusCodes.Status500InternalServerError)
            {
                _logger.LogError(
                    exception,
                    "Необроблена помилка. TraceId: {TraceId}",
                    httpContext.TraceIdentifier
                );
            }


            ProblemDetails problemDetails;

            if (exception is ValidationException validationException)
            {
                Dictionary<string, string[]> errors =
                    validationException.Errors
                        .GroupBy(
                            error => error.PropertyName
                        )
                        .ToDictionary(
                            group => group.Key,
                            group => group
                                .Select(error =>
                                    error.ErrorMessage
                                )
                                .ToArray()
                        );

                problemDetails =
                    new ValidationProblemDetails(
                        errors
                    )
                    {
                        Status = statusCode,
                        Title = title,
                        Detail =
                            "Передані дані не пройшли валідацію.",
                        Instance =
                            httpContext.Request.Path
                    };
            }
            else
            {
                problemDetails =
                    new ProblemDetails
                    {
                        Status = statusCode,
                        Title = title,

                        Detail =
                            statusCode ==
                            StatusCodes.Status500InternalServerError
                                ? "Під час обробки запиту виникла непередбачена помилка."
                                : exception.Message,

                        Instance =
                            httpContext.Request.Path
                    };
            }


            problemDetails.Extensions[
                "traceId"
            ] = httpContext.TraceIdentifier;


            httpContext.Response.StatusCode =
                statusCode;

            await httpContext.Response
                .WriteAsJsonAsync(
                    problemDetails,
                    cancellationToken
                );


            return true;
        }
    }
}