using ProjetPro.DTOs.Message;


namespace ProjetPro.Services.Interfaces;

public interface IMessageService
{
    // Récupérer tous les messages
    Task<IEnumerable<MessageResponseDto>> GetTousAsync();
    // Récupérer un message par ID
    Task<MessageResponseDto?> GetParIdAsync(int id);
    // Récupérer tous les messages d'un client
    Task<IEnumerable<MessageResponseDto>> GetParClientAsync(int clientId);
    // Envoyer un nouveau message
    Task<MessageResponseDto> EnvoyerAsync(int clientId, MessageRequestDto dto);
    // Mettre à jour le statut d'un message (ex: Livré, Échoué)
    Task<MessageResponseDto?> MettreAJourStatutAsync(int id, string statut);
    Task<MessageMasseResultatDto> EnvoyerEnMasseAsync(
    int clientId, 
    MessageMasseRequestDto dto);
}