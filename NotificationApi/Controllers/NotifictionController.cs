using Microsoft.AspNetCore.Mvc;
using NotificationApi.Models;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;

namespace NotificationApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationController : ControllerBase
{
    // HttpClient est l'outil qui permet à notre API de parler à une autre API
    private readonly HttpClient _httpClient;

    // Remplace cette URL par ton URL de Webhook Discord réelle
    private const string DiscordWebhookUrl = "https://cshap.free.beeceptor.com";
    private const string DiscordWebhookUrl2 = "https://pipedream.com/@edgardagossa5/invite?token=a86f7d49f32ba27db0eda7808fa19025";
    private readonly IConfiguration _configuration;

    public NotificationController(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    [HttpPost("send")]
    public async Task<IActionResult> sendNotification([FromBody] MessageRequest request)
    {
        //Validation de base
        if (string.IsNullOrEmpty(request.Content))
        {
            return BadRequest("Le contenu du message ne peut pas être vide.");
        }
        //préparation du paquet pour discord
        //discord attend un json avec une propriété content
        var payload = new { content = request.Content };

        var jsonPayload = JsonSerializer.Serialize(payload);

        var httpContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        try
        {
            //envoi simultané via le webhook
            // cette ligne pousse le message sur le réseau choisi
            var response = await _httpClient.PostAsync(DiscordWebhookUrl, httpContent);
            if (response.IsSuccessStatusCode)
            {
                return Ok(new { status = "Envoyé !", message = request.Content });
            }
            return StatusCode((int)response.StatusCode, "Erreur lors de l'envoi sur Discord.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erreur interne : {ex.Message}");
        }
    }


    [HttpPost("sent-multi")]
    public async Task<IActionResult> sendMulti([FromBody] MessageRequest request)
    {
        //On simule 3 destinations différentes 
        // (ici je mets 3 fois le même pour le test, mais imagine 3 amis différents)
        var urls = new List<string> {
            DiscordWebhookUrl,
            DiscordWebhookUrl,
            DiscordWebhookUrl,
            DiscordWebhookUrl,
            DiscordWebhookUrl,
            DiscordWebhookUrl,
            DiscordWebhookUrl,
            DiscordWebhookUrl,
            DiscordWebhookUrl,
            DiscordWebhookUrl
        };
        //on crée une liste de tâches vides 
        var tasks = new List<Task<HttpResponseMessage>>();
        foreach (var url in urls)
        {
            var json = JsonSerializer.Serialize(new { content = request.Content });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            //IMPORTANT: on ne met pas "await ici !
            //on ajoute juste l'action de poster dans notre liste de tâches à faire.
            tasks.Add(_httpClient.PostAsync(url, content));
        }
        await Task.WhenAll(tasks);
        return Ok("Le message a été diffusé partout instantanément !");

    }

    [HttpPost("alert")]
    public async Task<IActionResult> Alert([FromBody] AlertRequest request)
    {
        if (string.IsNullOrEmpty(request.Message))
        {
            return BadRequest("Le message ne peut pas être vide.");
        }
        string urgentOrInfo = request.IsUrgent ? "URGENT :" : "INFO :";

        string finalMessage = urgentOrInfo + request.Message;

        //préparation du contenu json(une seule fois !)
        var payload = new { content = finalMessage };
        var json = JsonSerializer.Serialize(payload);
        var messageContent = new StringContent(json, Encoding.UTF8, "application/json");

        var urls = new List<string> { DiscordWebhookUrl, DiscordWebhookUrl };

        if (request.IsUrgent)
        {
            var tasks = urls.Select(url => _httpClient.PostAsync(url, messageContent));
            await Task.WhenAll(tasks);
            return Ok("Alertes urgentes difusées.");
        }
        else
        {
            var response = await _httpClient.PostAsync(DiscordWebhookUrl, messageContent);
            return response.IsSuccessStatusCode ? Ok("Info envoyée") : StatusCode(500);
        }
    }

    [HttpPost("message-departement")]
    public async Task<IActionResult> MessageDepartementAsync([FromBody] DepartmentMessage request)
    {
        //On valide d'abord les données reçues
        if (string.IsNullOrEmpty(request.Content) || string.IsNullOrEmpty(request.DepartementName))
        {
            return BadRequest("Le contenu du message ou le Nom du Departement ne peut pas être vide.");
        }
        //On va chercher les URLs dans le JSON (Attention à l'orthographe exacte ici !)
        var urls = _configuration.GetSection($"DepartementSettings:{request.DepartementName}")
                                .Get<List<string>>();

        //On vérifie si on a trouvé des URLs
        if (urls == null || !urls.Any())
        {
            return NotFound($"Le département {request.DepartementName} n'est pas configuré dans le JSON.");
        }
        //Préparation du message
        var payload = new { content = request.Content };
        var json = JsonSerializer.Serialize(payload);
        var messageContent = new StringContent(json, Encoding.UTF8, "application/json");

        //On utilise la liste 'urls' qu'on vient de récupérer du JSON !
        var tasks = urls.Select(url => _httpClient.PostAsync(url, messageContent));

        var responses = await Task.WhenAll(tasks);

        //Vérification du succès
        if (responses.Any(r => !r.IsSuccessStatusCode)) return StatusCode(500, "Erreur lors de la diffusion");
        return Ok("Message distribué avec succès via la configuration JSON !");
    }


    [HttpPost("urgent")]
    public async Task<IActionResult> sendMessageUrgentAsync([FromBody] DepartmentMessage request)
    {
        if (string.IsNullOrEmpty(request.Content) || string.IsNullOrEmpty(request.DepartementName))
        {
            return BadRequest("Le contenu du message ou le nom du département est vide");
        }

        var urls = _configuration.GetSection($"DepartementSettings").GetChildren();
        var allUrls = urls.SelectMany(u => u.Get<List<string>>()).Distinct().ToList();

        var urlAsk = _configuration.GetSection($"DepartementSettings:{request.DepartementName}").Get<List<string>>();
        if (urlAsk == null) return NotFound("Département non configuré.");
        //serialisation du message
        var json = JsonSerializer.Serialize(new { content = request.Content });
        var messageContent = new StringContent(json, Encoding.UTF8, "application/json");

        //si la taille est inférieure à 5 -> errure
        if (request.Content.Length < 5)
        {
            return BadRequest("Le message doit faire plus de 5 caractères");
        }
        //si la taille est supérieure à 100 -> envoi vers url unique
        if (request.Content.Length > 100)
        {

            var task = _httpClient.PostAsync(DiscordWebhookUrl, messageContent);
            var response = await Task.WhenAll(task);

            return response.All(r => r.IsSuccessStatusCode) ? Ok("Message de 100 caractères  distribué !") : StatusCode(500);
        }

        if (request.Content.Contains("URGENT"))
        {

            var ticket = allUrls.Select(u => _httpClient.PostAsync(u, messageContent));
            var response = await Task.WhenAll(ticket);

            return response.All(r => r.IsSuccessStatusCode) ? Ok("Message urgent distribué !") : StatusCode(500);
        }
        var tasks = urlAsk.Select(u => _httpClient.PostAsync(u, messageContent));
        var responses = await Task.WhenAll(tasks);
        return responses.All(r => r.IsSuccessStatusCode) ? Ok("Message distribué !") : StatusCode(500);



    }

}