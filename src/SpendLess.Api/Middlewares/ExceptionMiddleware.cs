using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SpendLess.Application.Exceptions;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace SpendLess.Api.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ProblemDetailsFactory _problemDetailsFactory;
        public ExceptionMiddleware(RequestDelegate next, 
                                   ProblemDetailsFactory problemDetailsFactory)
        {
            _next = next;
            _problemDetailsFactory = problemDetailsFactory;
        }

        public async Task InvokeAsync(HttpContext httpContext, ILogger<ExceptionMiddleware> logger)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex, logger);
            }
        }
        private async Task HandleExceptionAsync(HttpContext httpContext, Exception exception, ILogger<ExceptionMiddleware> logger)
        {
            var problem = new ProblemDetails();
            problem.Instance = httpContext.Request.Path;
            problem.Detail = exception?.Message;

            switch (exception)
            {
                case ValidationException validationException:
                    problem.Status = (int)HttpStatusCode.InternalServerError;
                    foreach (var error in validationException.Errors)
                    {
                        problem.Extensions.Add(error.Key, error.Value);
                        logger.LogError("Error happened at {@ErrorKey}: {@ErrorValue}",
                                          error.Key, error.Value);
                    }
                    break;
                case NotFoundException:
                    problem.Status = (int)HttpStatusCode.NotFound;
                    break;
                case BadRequestException:
                    problem.Status = (int)HttpStatusCode.BadRequest;
                    break;
                default:
                    break;
            }

            ProblemDetails problemDetails;
            if (_problemDetailsFactory != null)
            {
                problemDetails = _problemDetailsFactory.CreateProblemDetails(httpContext, statusCode: problem.Status);
                problem.Title = problemDetails.Title;
                problem.Type = problemDetails.Type;
            }

            var result = new ObjectResult(problem)
            {
                StatusCode = problem.Status
            };
            var response = JsonConvert.SerializeObject(result.Value);
            httpContext.Response.ContentType = "application/problem+json";
            await httpContext.Response.WriteAsync(response);
        }
    }
}
