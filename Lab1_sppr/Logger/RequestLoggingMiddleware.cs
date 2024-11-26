namespace Lab1_sppr.Logger
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await _next(context);

            // Логируем только если статус ответа не 2XX
            if (context.Response.StatusCode < 200 || context.Response.StatusCode >= 300)
            {
                var url = context.Request.Path;
                var statusCode = context.Response.StatusCode;
                _logger.LogInformation("---> request {Url} returns {StatusCode}", url, statusCode);
            }
        }
    }
}
