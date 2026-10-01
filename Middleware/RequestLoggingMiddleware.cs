namespace SmartGearWeb.Middleware
{
    // Custom middleware: logs the HTTP method and path of every request that
    // passes through the pipeline, plus the response status once it completes.
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
            var method = context.Request.Method;
            var path = context.Request.Path;

            _logger.LogInformation("Incoming request: {Method} {Path}", method, path);

            // Pass control to the next middleware in the pipeline
            await _next(context);

            _logger.LogInformation(
                "Completed request: {Method} {Path} -> {StatusCode}",
                method, path, context.Response.StatusCode);
        }
    }
}
