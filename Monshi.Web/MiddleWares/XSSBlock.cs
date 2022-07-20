namespace WebApplication2.MiddleWares;

public class XSSBlock
{

    public RequestDelegate _next;

    public XSSBlock(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        context.Response.Headers.Add("Content-Security-Policy","script-src");
        context.Response.Headers.Add("X-WebKit-CSP","script-src");
        context.Response.Headers.Add("X-Content-Security-Policy", "script-src");
        await _next(context);
    }
}

public static class XSSBlocker
{
    public static void UseXSSBlocker(this WebApplication app)
    {
        app.UseMiddleware<XSSBlock>();
    }
}