using Microsoft.AspNetCore.Mvc;
using GestionProduits.DTOs;
using GestionProduits.Services;

namespace GestionProduits.Controllers;

[ApiController]

[Route("api/[controller]")]
[Produces("application/json")]// ← dit au client qu'on retourne du JSON
public class ProduitsController : ControllerBase

{
    private readonly IProduitService _service;
    private readonly ILogger<ProduitsController> _logger;


    public ProduitsController(IProduitService service, ILogger<ProduitsController> logger)
    {
        _service = service;
        _logger = logger;
    }

[HttpGet]
    public async Task<ActionResult<List<ProduitResponseDto>>> GetAll()
    {
        try
        {
            var produits = await _service.GetTousAsync();
            return Ok(produits);
        } catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des produits");
            return StatusCode(500, "Une erreur interne est survenue.");

        }
    }

    // GET api/produits/1
    [HttpGet("{id:int:min(1)}")]
    public async Task<ActionResult<ProduitResponseDto>> GetParId(int id)
    {
        try
        {
            var produit = await _service.GetParIdAsync(id);
            if (produit == null)
                return NotFound($"Produit avec l'id {id} introuvable.");

            return Ok(produit);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur GetParId {Id}", id);
            return StatusCode(500, "Une erreur interne est survenue.");
        }
    }


    [HttpPost]
    public async Task<ActionResult<ProduitResponseDto>> Creer([FromBody] ProduitRequestDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var produit = await _service.CreerAsync(dto);
            return CreatedAtAction(nameof(GetParId), new { id = produit.Id }, produit);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur Creer");
            return StatusCode(500, "Une erreur interne est survenue.");
        }
    }

  [HttpDelete("{id:int}")]
    public async Task<ActionResult> Supprimer(int id)
    {
        try
        {
            var supprime = await _service.SupprimerAsync(id);
            if (!supprime)
                return NotFound($"Produit avec l'id {id} introuvable.");

            return Ok("Produit supprimé avec succès.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur Supprimer {Id}", id);
            return StatusCode(500, "Une erreur interne est survenue.");
        }
    }
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ProduitResponseDto>> Modifier(int id, [FromBody] ProduitRequestDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var produit = await _service.ModifierAsync(id, dto);
            if (produit == null)
                return NotFound($"Produit avec l'id {id} introuvable.");

            return Ok(produit);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur Modifier {Id}", id);
            return StatusCode(500, "Une erreur interne est survenue.");
        }
    }
}


