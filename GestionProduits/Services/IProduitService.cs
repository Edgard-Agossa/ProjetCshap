using GestionProduits.DTOs;

namespace GestionProduits.Services;

public interface IProduitService
{
    Task<List<ProduitResponseDto>> GetTousAsync();
    Task<ProduitResponseDto?> GetParIdAsync(int id);
    Task<ProduitResponseDto> CreerAsync(ProduitRequestDto dto);
    Task<ProduitResponseDto?> ModifierAsync(int id, ProduitRequestDto dto);
    Task<bool> SupprimerAsync(int id);
}

// IEnumerable<> = une collection qu'on peut parcourir. C'est plus général que List<>.

// Task<> = "cette méthode est asynchrone". Elle va prendre du temps (accès DB) donc on la marque Task pour ne pas bloquer le serveur.

// Le ? = "peut retourner null". Si le produit n'existe pas, on retourne null au lieu de planter.