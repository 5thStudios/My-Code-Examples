using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;


namespace UmbracoProject.Models
{
    public static class DomainNameMiddlewareExtensions
    {
        public static IApplicationBuilder UseDomainNameMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<DomainNameMiddleware>();
        }
    }




    public class DomainNameMiddleware
    {
        private readonly RequestDelegate _next;

        public DomainNameMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Extract the domain name from the "Host" header and store it in the HttpContext
            var host = context.Request.Host.Host;
            context.Items["DomainName"] = host;

            // Call the next middleware in the pipeline
            await _next(context);
        }
    }



}
