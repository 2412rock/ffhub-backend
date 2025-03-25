using FFhub_backend.Services;
using Microsoft.Extensions.Primitives;

namespace FFhub_backend.Middleware
{
    public class AccessLogMiddleware
    {
        private readonly RequestDelegate _next;

        public AccessLogMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, AccessLogService accessLogService)
        {
            try
            {
                // var ipAddress = context.Connection.RemoteIpAddress;
                var ip = context.Connection.RemoteIpAddress?.ToString();
                if (ip != null)
                {
                    await accessLogService.AddLog(ip);
                }
            }
            catch { }
            await _next(context);
        }
    }
}
