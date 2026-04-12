// Controllers/MessagesController.cs
using Microsoft.AspNetCore.Mvc;
using ProjetPro.DTOs.Message;
using ProjetPro.Services.Interfaces;

namespace ProjetPro.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class MessagesController : ControllerBase
{
    private readonly IMessageService _service;
    private readonly ILogger<MessagesController> _logger;

    public MessagesController(
        IMessageService service,
        ILogger<MessagesController> logger)
    {
        _service = service;
        _logger = logger;
    }

    // GET api/v1/messages
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<MessageResponseDto>>> GetTous()
    {
        try
        {
            var messages = await _service.GetTousAsync();
            return Ok(messages);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur GetTous Messages");
            return StatusCode(500, "Erreur interne du serveur.");
        }
    }

    // GET api/v1/messages/1
    [HttpGet("{id:int}", Name = "GetMessageParId")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MessageResponseDto>> GetParId(int id)
    {
        try
        {
            var message = await _service.GetParIdAsync(id);

            if (message is null)
                return NotFound(new { message = $"Message {id} introuvable." });

            return Ok(message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur GetParId Message {Id}", id);
            return StatusCode(500, "Erreur interne du serveur.");
        }
    }

    // GET api/v1/messages/client/1
    [HttpGet("client/{clientId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<MessageResponseDto>>> GetParClient(
        int clientId)
    {
        try
        {
            var messages = await _service.GetParClientAsync(clientId);
            return Ok(messages);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur GetParClient {ClientId}", clientId);
            return StatusCode(500, "Erreur interne du serveur.");
        }
    }

    // POST api/v1/messages/envoyer/1
    [HttpPost("envoyer/{clientId:int}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MessageResponseDto>> Envoyer(
        int clientId, [FromBody] MessageRequestDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var message = await _service.EnvoyerAsync(clientId, dto);

            return CreatedAtRoute("GetMessageParId",
                new { id = message.Id }, message);
        }
        catch (KeyNotFoundException ex)
        {
            // Client introuvable
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            // Client inactif ou solde insuffisant
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur Envoyer Message");
            return StatusCode(500, "Erreur interne du serveur.");
        }
    }

    // PATCH api/v1/messages/1/statut?statut=Delivre
    [HttpPatch("{id:int}/statut")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MessageResponseDto>> MettreAJourStatut(
        int id, [FromQuery] string statut)
    {
        try
        {
            var message = await _service.MettreAJourStatutAsync(id, statut);

            if (message is null)
                return NotFound(new { message = $"Message {id} introuvable." });

            return Ok(message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur MettreAJourStatut {Id}", id);
            return StatusCode(500, "Erreur interne du serveur.");
        }
    }
}