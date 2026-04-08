🟢 Module 5 — Routes, Contrôleurs, Services

5.1 — Les Routes en profondeur
Il existe 3 façons de définir des routes en ASP.NET Core :

**Façon 1 — Route par attribut (recommandée en microservices)**
[ApiController]
[Route("api/v1/[controller]")] // ← "v1" = versioning de l'API
public class ProduitsController : ControllerBase
{
    // GET api/v1/produits
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProduitResponseDto>>> GetTous()
    { ... }

// GET api/v1/produits/5
    [HttpGet("{id:int}")] // ← ":int" = contrainte de type
    public async Task<ActionResult<ProduitResponseDto>> GetParId(int id)
    { ... }

   -// GET api/v1/produits/search?nom=télé-
    [HttpGet("search")] // ← route personnalisée
    public async Task<ActionResult<IEnumerable<ProduitResponseDto>>> Rechercher(
        [FromQuery] string nom) // ← depuis l'URL ?nom=...
    { ... }
}


Convention Microsoft : toujours versionner ton API avec v1, v2 etc. Si tu changes l'API, tu crées v2 sans casser les clients qui utilisent v1.

<!-- Façon 2 — Contraintes de routes -->

[HttpGet("{id:int}")]        // id doit être un entier
[HttpGet("{nom:alpha}")]     // nom doit être alphabétique
[HttpGet("{id:int:min(1)}")] // id doit être un entier >= 1
[HttpGet("{code:length(5)}")] // code doit avoir exactement 5 caractères



<!-- Façon 3 — Sources des paramètres -->

// Depuis l'URL : /api/produits/5
[HttpGet("{id}")]
public async Task<ActionResult> Get([FromRoute] int id) { ... }

// Depuis le body JSON
[HttpPost]
public async Task<ActionResult> Creer([FromBody] ProduitRequestDto dto) { ... }

// Depuis l'URL ?page=1&taille=10
[HttpGet]
public async Task<ActionResult> GetTous(
    [FromQuery] int page = 1,
    [FromQuery] int taille = 10) { ... }

// Depuis le header HTTP
[HttpGet]
public async Task<ActionResult> Get(
    [FromHeader(Name = "X-Api-Key")] string apiKey) { ... }


##5.2 — Les Contrôleurs — Conventions officielles
Voici un Controller complet aux normes .NET 8 :


// Controllers/ProduitsController.cs
using Microsoft.AspNetCore.Mvc;
using GestionProduits.DTOs;
using GestionProduits.Services;

namespace GestionProduits.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")] // ← dit au client qu'on retourne du JSON
public class ProduitsController : ControllerBase
{
    private readonly IProduitService _service;
    private readonly ILogger<ProduitsController> _logger;

    public ProduitsController(
        IProduitService service,
        ILogger<ProduitsController> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <summary>Récupère tous les produits</summary>
    /// <returns>Liste de produits</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ProduitResponseDto>>> GetTous()
    {
        try
        {
            var produits = await _service.GetTousAsync();
            return Ok(produits);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des produits");
            return StatusCode(500, "Erreur interne du serveur.");
        }
    }

    /// <summary>Récupère un produit par son ID</summary>
    /// <param name="id">L'identifiant du produit</param>
    [HttpGet("{id:int}", Name = "GetProduitParId")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProduitResponseDto>> GetParId(int id)
    {
        try
        {
            var produit = await _service.GetParIdAsync(id);
            if (produit is null)
                return NotFound(new { message = $"Produit {id} introuvable." });

            return Ok(produit);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur GetParId {Id}", id);
            return StatusCode(500, "Erreur interne du serveur.");
        }
    }

    /// <summary>Recherche des produits par nom</summary>
    [HttpGet("search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<ProduitResponseDto>>> Rechercher(
        [FromQuery] string nom,
        [FromQuery] int page = 1,
        [FromQuery] int taille = 10)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(nom))
                return BadRequest(new { message = "Le nom de recherche est requis." });

            var produits = await _service.RechercherAsync(nom, page, taille);
            return Ok(produits);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur Rechercher {Nom}", nom);
            return StatusCode(500, "Erreur interne du serveur.");
        }
    }

    /// <summary>Crée un nouveau produit</summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProduitResponseDto>> Creer(
        [FromBody] ProduitRequestDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var produit = await _service.CreerAsync(dto);

            // ← Retourne 201 + l'URL de la nouvelle ressource
            return CreatedAtRoute("GetProduitParId",
                new { id = produit.Id }, produit);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur Creer produit");
            return StatusCode(500, "Erreur interne du serveur.");
        }
    }

    /// <summary>Modifie un produit existant</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProduitResponseDto>> Modifier(
        int id, [FromBody] ProduitRequestDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var produit = await _service.ModifierAsync(id, dto);
            if (produit is null)
                return NotFound(new { message = $"Produit {id} introuvable." });

            return Ok(produit);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur Modifier {Id}", id);
            return StatusCode(500, "Erreur interne du serveur.");
        }
    }

    /// <summary>Supprime un produit</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Supprimer(int id)
    {
        try
        {
            var supprime = await _service.SupprimerAsync(id);
            if (!supprime)
                return NotFound(new { message = $"Produit {id} introuvable." });

            return NoContent(); // ← 204 = supprimé, pas de contenu à retourner
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur Supprimer {Id}", id);
            return StatusCode(500, "Erreur interne du serveur.");
        }
    }
}




public interface IProduitService
│
├── using GestionProduits.DTOs     → j'importe les DTOs
├── namespace GestionProduits.Services → mon adresse dans le projet
│
├── Task<>          → méthode asynchrone
├── IEnumerable<>   → collection flexible (plusieurs éléments)
├── ?               → peut retourner null
│
├── GetTousAsync()              → liste tous les produits
├── GetParIdAsync(id)           → un produit par ID
├── RechercherAsync(nom,pg,t)   → recherche avec pagination
├── CreerAsync(dto)             → crée un produit
├── ModifierAsync(id, dto)      → modifie un produit
└── SupprimerAsync(id)          → supprime, retourne vrai/faux



Tu as maintenant une base solide sur l'Injection de Dépendances, qui est le cœur de l'architecture .NET moderne.

Pour conclure ce "mini-cours" et t'assurer d'être fin prêt pour ton entretien chez Orange Money (ou ailleurs), retiens bien ces 3 piliers que nous avons pratiqués :

Le Contrat (Interface) : On ne parle jamais à une classe directement, on passe par une interface. C'est ce qui rend ton microservice flexible (Loi de l'abstraction).

L'Enregistrement (Program.cs) : C'est là qu'on décide quel "ouvrier" (classe) va réaliser quel "contrat" (interface) et pour combien de temps (Scoped, Singleton, Transient).

L'Injection par Constructeur : C'est la manière propre et automatique de recevoir ses outils. On ne fait plus jamais de new Classe(), on laisse le système nous "livrer" ce dont on a besoin.