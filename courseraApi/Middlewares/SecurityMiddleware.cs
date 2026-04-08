public class SecurityMiddleware
{
    private readonly RequestDelegate _next;
    public SecurityMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        if(!context.Request.Headers.ContainsKey("x-Orange-key"))
        {
            context.Response.StatusCode = 401;//Unauthorized
           await context.Response.WriteAsJsonAsync(new 
{
    error = "Acces refuse : Cle de securite manquante."
});
            return; //On arrête tout ici (on n'appelle pas _next)
        }
        await _next(context);
    }
}