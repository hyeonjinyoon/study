using Microsoft.Identity.Json.Linq;
using Newtonsoft.Json;
using WebAPI.Service;

namespace WebAPI.Middleware;

public class RequestParser
{
    private readonly RequestDelegate _next;

    public RequestParser(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext httpContext, RequestContext requestContext)
    {
        var text = await ReadBodyAsync(httpContext.Request);

        try
        {
            JsonConvert.PopulateObject(text, requestContext);
        }
        catch
        {
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }

        if (string.IsNullOrEmpty(requestContext.Uid) ||
            requestContext.Actions == null)
        {
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }

        await _next.Invoke(httpContext);
    }
    
    private Task<string> ReadBodyAsync(HttpRequest request)
    {
        var sr = new StreamReader(request.Body);
        return sr.ReadToEndAsync();
    }
}
