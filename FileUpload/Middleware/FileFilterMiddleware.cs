namespace FileUpload.Middleware
{
    public class FileFilterMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string[] _allowedFileTypes;
        private readonly int _maxFileSizeMB;
        private readonly int _minFileSizeMB;

        public FileFilterMiddleware(RequestDelegate next, string[] allowedFileTypes, int maxFileSizeMB, int minFileSizeMB)
        {
            _next = next;
            _allowedFileTypes = allowedFileTypes;
            _maxFileSizeMB = maxFileSizeMB;
            _minFileSizeMB = minFileSizeMB;
        }

        public async Task InvokeAsync(HttpContext context)
        {

            await _next(context);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline
    public static class FileFilterMiddlewareExtensions
    {
        public static IApplicationBuilder UseFileFilterMiddleware(this IApplicationBuilder builder, string[] allowedFileTypes, int maxFileSizeMB, int minFileSizeMB)
        {
            return builder.UseMiddleware<FileFilterMiddleware>(allowedFileTypes, maxFileSizeMB, minFileSizeMB);
        }
    }
}
