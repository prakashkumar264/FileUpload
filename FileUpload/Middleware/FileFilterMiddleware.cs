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
            if (context.Request.Method == "POST")
            {
                var formCollection = await context.Request.ReadFormAsync();
                var file = formCollection.Files.GetFile("file");

                if (file != null)
                {
                    // Validate file type
                    string fileExtension = Path.GetExtension(file.FileName);
                    if (Array.IndexOf(_allowedFileTypes, fileExtension) == -1)
                    {
                        context.Response.StatusCode = 400; // Bad Request
                        await context.Response.WriteAsync("Uploaded file type is not allowed.");
                        return;
                    }

                    // Validate file size
                    long fileSizeMB = file.Length / (1024 * 1024); // Convert bytes to megabytes
                    if (fileSizeMB > _maxFileSizeMB || fileSizeMB < _minFileSizeMB)
                    {
                        context.Response.StatusCode = 400; // Bad Request
                        await context.Response.WriteAsync($"Uploaded file size should be between {_minFileSizeMB}MB and {_maxFileSizeMB}MB.");
                        return;
                    }
                }
            }

            // Call the next middleware in the pipeline
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
