using DeviceDetectorNET;

namespace FoodSearch
{
    public class MobileDeviceMiddleware
    {
        private readonly RequestDelegate _next;
        private static bool _redirected = false;

        public MobileDeviceMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var isMobile = IsMobileDevice(httpContext);
            httpContext.Items["IsMobileDevice"] = isMobile;

            if (isMobile && !_redirected)
            {
                _redirected = true;
                httpContext.Response.Redirect("../Home/MobileError");
                return;
            }

            await _next(httpContext);

            // Reset the flag after processing the request
            _redirected = false;
        }

        private bool IsMobileDevice(HttpContext httpContext)
        {
            var userAgent = httpContext.Request.Headers["User-Agent"].ToString();
            var dd = new DeviceDetector(userAgent);
            dd.Parse();
            return dd.IsMobile();
        }
    }

    public static class MobileDeviceMiddlewareExtensions
    {
        public static IApplicationBuilder UseMobileDeviceMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<MobileDeviceMiddleware>();
        }
    }
}
