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
    private const string DiscordWebhookUrl = "https://webhook.site/00922766-58f4-48c6-91ee-74fd49ffe107";

    public NotificationController(HttpClient httpClient)
    {
        _httpClient = httpClient;
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
        var payload = new {content = request.Content};
            
        var jsonPayload = JsonSerializer.Serialize(payload);
        
        var httpContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
  
        try
        {
            //envoi simultané via le webhook
            // cette ligne pousse le message sur le réseau choisi
            var response = await _httpClient.PostAsync(DiscordWebhookUrl, httpContent);
            if(response.IsSuccessStatusCode)
            {
                return Ok(new {status = "Envoyé !", message = request.Content});
            }
            return StatusCode((int)response.StatusCode, "Erreur lors de l'envoi sur Discord.");
        }catch (Exception ex)
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
        foreach(var url in urls)
        {
            var json = JsonSerializer.Serialize(new {content = request.Content});
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            //IMPORTANT: on ne met pas "await ici !
            //on ajoute juste l'action de poster dans notre liste de tâches à faire.
            tasks.Add(_httpClient.PostAsync(url, content));
        }
        await Task.WhenAll(tasks);
        return Ok("Le message a été diffusé partout instantanément !");

    }
}