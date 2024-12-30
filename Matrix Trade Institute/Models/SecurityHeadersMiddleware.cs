namespace www.Middleware;

public sealed class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;

    public SecurityHeadersMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public Task Invoke(HttpContext context)
    {
        var headers = context.Response.Headers;

        headers.Append("referrer-policy", "no-referrer");
        headers.Append("x-content-type-options", "nosniff");
        headers.Append("x-frame-options", "SAMEORIGIN");
        headers.Append("x-permitted-cross-domain-policies", "none");
        //headers.Append("x-xss-protection", "1; mode=block");
        headers.Append("permissions-policy", "accelerometer=(),autoplay=(),camera=(),display-capture=(),fullscreen=(),geolocation=(),gyroscope=(),magnetometer=(),microphone=(),midi=(),payment=()");

        return _next(context);
    }
}