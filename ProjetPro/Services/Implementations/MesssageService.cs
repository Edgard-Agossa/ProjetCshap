// Services/Implementations/MessageService.cs
using Microsoft.EntityFrameworkCore;
using ProjetPro.Data;
using ProjetPro.DTOs.Message;
using ProjetPro.Models;
using ProjetPro.Services.Interfaces;


namespace ProjetPro.Services.Implementations;

public class MessageService : IMessageService
{
    private readonly AppDbContext _context;
    private readonly ILogger<MessageService> _logger;

    public MessageService(AppDbContext context, ILogger<MessageService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<MessageResponseDto>> GetTousAsync()
    {
        try
        {
            return await _context.Messages
                .AsNoTracking()
                .Select(m => MapToResponseDto(m))
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur GetTousAsync Messages");
            throw;
        }
    }

    public async Task<MessageResponseDto?> GetParIdAsync(int id)
    {
        try
        {
            var message = await _context.Messages
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (message is null) return null;

            return MapToResponseDto(message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur GetParIdAsync Message {Id}", id);
            throw;
        }
    }

    public async Task<IEnumerable<MessageResponseDto>> GetParClientAsync(int clientId)
    {
        try
        {
            return await _context.Messages
                .AsNoTracking()
                .Where(m => m.ClientId == clientId) // filtre par client
                .OrderByDescending(m => m.CreatedAt) // plus récents en premier
                .Select(m => MapToResponseDto(m))
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur GetParClientAsync {ClientId}", clientId);
            throw;
        }
    }

    public async Task<MessageResponseDto> EnvoyerAsync(int clientId, MessageRequestDto dto)
    {
        try
        {
            // 1. Vérifier que le client existe et est actif
            var client = await _context.Clients.FindAsync(clientId);

            if (client is null)
                throw new KeyNotFoundException($"Client {clientId} introuvable.");

            if (!client.EstActif)
                throw new InvalidOperationException("Ce client est désactivé.");

            // 2. Vérifier que le client a assez de solde
            if (client.SoldeMessages <= 0)
                throw new InvalidOperationException("Solde insuffisant.");

            // 3. Créer le message
            var message = new Message
            {
                ClientId = clientId,
                Expediteur = dto.Expediteur,
                Destinataire = dto.Destinataire,
                Contenu = dto.Contenu,
                Type = dto.Type,
                Statut = StatutMessage.EnAttente
            };

            // 4. Déduire du solde
            client.SoldeMessages -= 1;

            // 5. Sauvegarder
            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Message {Id} créé pour client {ClientId} → {Destinataire}",
                message.Id, clientId, dto.Destinataire);

            return MapToResponseDto(message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur EnvoyerAsync");
            throw;
        }
    }

    public async Task<MessageResponseDto?> MettreAJourStatutAsync(int id, string statut)
    {
        try
        {
            var message = await _context.Messages.FindAsync(id);

            if (message is null) return null;

            // Convertir le string en enum
            if (!Enum.TryParse<StatutMessage>(statut, true, out var nouveauStatut))
                throw new ArgumentException($"Statut '{statut}' invalide.");

            message.Statut = nouveauStatut;

            // Mettre à jour les dates selon le statut
            if (nouveauStatut == StatutMessage.Envoye)
                message.EnvoyeAt = DateTime.UtcNow;
            else if (nouveauStatut == StatutMessage.Delivre)
                message.DelivreAt = DateTime.UtcNow;
            else if (nouveauStatut == StatutMessage.Echoue)
                message.NombreTentatives += 1;

            await _context.SaveChangesAsync();

            return MapToResponseDto(message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur MettreAJourStatutAsync {Id}", id);
            throw;
        }
    }

    // ✅ DRY — une seule méthode de mapping
    private static MessageResponseDto MapToResponseDto(Message m) => new()
    {
        Id = m.Id,
        Expediteur = m.Expediteur,
        Destinataire = m.Destinataire,
        Contenu = m.Contenu,
        Statut = m.Statut.ToString(),
        Type = m.Type.ToString(),
        CreatedAt = m.CreatedAt,
        EnvoyeAt = m.EnvoyeAt,
        DelivreAt = m.DelivreAt,
        NombreTentatives = m.NombreTentatives,
        ErreurMessage = m.ErreurMessage
    };
}