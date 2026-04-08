using System.Net;

public class ErrorHandlingMiddleware
{
    //RequestDelegate représente l'étape suivante dans le pipeline ASP.NET.
    private readonly RequestDelegate _next;
    public ErrorHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Alerte Microservice] Erreur détectée : {ex.Message}");
            context.Response.StatusCode = 500;
            await context.Response.WriteAsJsonAsync(new
            {
                error = "Une erreur est survenue",
                Message = ex.Message
            });
        }
    }
}