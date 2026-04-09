
using Microsoft.Extensions.Options;
public class SecurityMiddleware
{
    private readonly RequestDelegate _next;
    private readonly OrangeOptions _options;

    public SecurityMiddleware(RequestDelegate next, IOptions<OrangeOptions> options)
    {
        _next = next;
        _options = options.Value;
    }

    public async Task Invoke(HttpContext context)
    {
        // On compare avec la clé qui vient du fichier appsettings.json
        if(!context.Request.Headers.TryGetValue("x-Orange-key", out var extractedKey) || extractedKey != _options.ApiKey)
        {
            context.Response.StatusCode = 401;//Unauthorized
           await context.Response.WriteAsJsonAsync(new { error = "Cle invalide."});
            return; //On arrête tout ici (on n'appelle pas _next)
        }
        await _next(context);
    }
}