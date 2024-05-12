using DeviceDetectorNET;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace FoodSearch
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class MobileDeviceMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;
        private bool _redirected = false;

        public async Task Invoke(HttpContext httpContext)
        {
            if (!_redirected && IsMobileDevice(httpContext))
            {
                _redirected = true;
                httpContext.Response.Redirect("../Home/MobileError");
                return;
            }

            // Продолжайте выполнение запроса, если это не мобильное устройство или если уже началось перенаправление.
            await _next(httpContext);

            // Восстановление флага после обработки запроса
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

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class MobileDeviceMiddlewareExtensions
    {
        public static IApplicationBuilder UseMobileDeviceMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<MobileDeviceMiddleware>();
        }
    }
}
