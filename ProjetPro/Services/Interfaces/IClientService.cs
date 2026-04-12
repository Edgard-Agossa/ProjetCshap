using ProjetPro.DTOs.Client;

namespace ProjetPro.Services.Interfaces;

public interface IClientService
{
    Task<IEnumerable<ClientResponseDto>> GetTousAsync();
    Task<ClientResponseDto?> GetParIdAsync(int id);
    Task<ClientResponseDto?> GetParApiKeyAsync(string apiKey);
    Task<ClientResponseDto> CreerAsync(ClientRequestDto dto);
    Task<ClientResponseDto?> RechargerSoldeAsync(int id, int montant);
    Task<bool> DesactiverAsync(int id);
}