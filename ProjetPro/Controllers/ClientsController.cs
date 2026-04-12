using System.Net;
using Microsoft.AspNetCore.Mvc;
using ProjetPro.DTOs.Client;
using ProjetPro.Services.Interfaces;

namespace ProjetPro.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]

public class ClientsController : ControllerBase
{
    private readonly IClientService _service;
    private readonly ILogger<ClientsController> _logger;
    public ClientsController(IClientService service, ILogger<ClientsController> logger)
    {
        _service = service;
        _logger = logger;
    }

    // GET api/v1/clients
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ClientResponseDto>>> GetTous()
    {
        try
        {
            var clients = await _service.GetTousAsync();
            return Ok(clients);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur GetTous Client controller");
            return StatusCode(500, "Erreur interne du serveur");
        }
    }

    // GET api/v1/clients/1
    [HttpGet("{id: int}", Name = "GetClientParId")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClientResponseDto>> GetParId(int id)
    {
        try
        {
            var client = await _service.GetParIdAsync(id);
            if (client is null)
                return NotFound(new { message = $"Client avec l'id {id} n'existe pas" });
            return Ok(client);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur GetParId Client controller");
            return StatusCode(500, "Erreur interne du serveur");
        }
    }

    // POST api/v1/clients
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ClientResponseDto>> Creer([FromBody] ClientRequestDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var client = await _service.CreerAsync(dto);
        
        return CreatedAtRoute("GetClientParId", new { id = client.Id }, client);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur Creer Client");
            return StatusCode(500, "Erreur interne du serveur.");
        }
    }
     // PUT api/v1/clients/1/recharger?montant=100
    [HttpPut("{id:int}/recharger")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClientResponseDto>> RechargerSolde(
        int id, [FromQuery] int montant)
    {
        try
        {
            if (montant <= 0)
                return BadRequest(new { message = "Le montant doit être positif." });

            var client = await _service.RechargerSoldeAsync(id, montant);

            if (client is null)
                return NotFound(new { message = $"Client {id} introuvable." });

            return Ok(client);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur RechargerSolde {Id}", id);
            return StatusCode(500, "Erreur interne du serveur.");
        }
    }
// DELETE api/v1/clients/1
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Desactiver(int id)
    {
        try
        {
            var resultat = await _service.DesactiverAsync(id);

            if (!resultat)
                return NotFound(new { message = $"Client {id} introuvable." });

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur Desactiver {Id}", id);
            return StatusCode(500, "Erreur interne du serveur.");
        }
    }
}