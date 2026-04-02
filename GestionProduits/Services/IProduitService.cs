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