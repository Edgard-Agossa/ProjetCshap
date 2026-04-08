using System.Diagnostics;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    public RequestLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            //on demande un chronomètre
            var wacth = Stopwatch.StartNew();

            //on récupère les infos de la requête
            var methode = context.Request.Method;//GET, POST, etc.
            var path = context.Request.Path;//l'url appelée

            Console.WriteLine($"[AUDIT] Début : {methode} {path}");
            //on lasse la requête continuer son chemin
            await _next(context);
            wacth.Stop();
            var elapsedMs = wacth.ElapsedMilliseconds;
            var statusCode = context.Response.StatusCode;

            Console.WriteLine($"[Audit] fin : {methode} {path} | Statut : {statusCode} | Durée : {elapsedMs}ms");


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