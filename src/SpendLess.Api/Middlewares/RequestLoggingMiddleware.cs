using Newtonsoft.Json;
using SpendLess.Application.Exceptions;
using System.Net;

namespace SpendLess.Api.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        public RequestLoggingMiddleware(RequestDelegate next, ILogger logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            _logger.LogInformation("Request {@RequestId} started {@UtcNow}", 
                                    httpContext.TraceIdentifier, DateTime.UtcNow);

            await _next(httpContext);

            _logger.LogInformation("Request {@RequestId} ended {@UtcNow}",
                                    httpContext.TraceIdentifier, DateTime.UtcNow);      
        }
    }
}
