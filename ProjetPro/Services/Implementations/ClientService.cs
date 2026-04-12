using Microsoft.EntityFrameworkCore;
using ProjetPro.Data;
using ProjetPro.DTOs.Client;
using ProjetPro.Models;
using ProjetPro.Services.Interfaces;

namespace ProjetPro.Services.Implementations;

public class ClientService : IClientService
{
    private readonly AppDbContext _context;
    private readonly ILogger<ClientService> _logger;

    public ClientService(AppDbContext context, ILogger<ClientService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<ClientResponseDto>> GetTousAsync()
    {
        try
        {
            return await _context.Clients
                .AsNoTracking() //lecture seule = plus rapide
                .Select(c => new ClientResponseDto
                {
                    Id = c.Id,
                    Nom = c.Nom,
                    ApiKey = c.ApiKey,
                    SoldeMessages = c.SoldeMessages,
                    EstActif = c.EstActif,
                    CreatedAt = c.CreatedAt

                })
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur GetTousAsync Clients");
            throw;
        }
    }

    public async Task<ClientResponseDto?> GetParIdAsync(int id)
    {
        try
        {
            var client = await _context.Clients
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
            if (client is null) return null;
            return MapToResponseDto(client);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur GetParIdAsync Client {Id}", id);
            throw;
        }
    }


public async Task<ClientResponseDto?> GetParApiKeyAsync(string apiKey)
    {
        try
        {
            var client = await _context.Clients
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.ApiKey == apiKey);
            if (client is null) return null;
            
            return MapToResponseDto(client);
        }catch(Exception ex)
        {
            _logger.LogError(ex, "Erreur GetParApiKeyAsync Client {ApiKey}", apiKey);
            throw;
        }
    }

public async Task<ClientResponseDto> CreerAsync(ClientRequestDto dto)
    {
        try
        {
            var client =new Client
            {
                Nom = dto.Nom,
                SoldeMessages = dto.SoldeMessages,
              // ApiKey généré automatiquement dans le Model
            };
            await _context.Clients.AddAsync(client);
            await _context.SaveChangesAsync();
            _logger.LogInformation(
                "Nouveau client créé : {Nom} avec ApiKey : {ApiKey}",
                client.Nom, 
                client.ApiKey
            );
 return MapToResponseDto(client);

        }catch(Exception ex)
        {
            _logger.LogError(ex, "Erreur CreerAsync Client");
            throw;
        }
    }


public async Task<ClientResponseDto?> RechargerSoldeAsync(int id, int montant)
    {
        var client = await _context.Clients.FindAsync(id);

        if (client is null) return null;

        if (montant <= 0)
        throw new ArgumentException("Le montant du rechargement doit être supérieur à zéro.", nameof(montant));

        client.SoldeMessages += montant;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Solde rechargé pour client {Id} : +{Montant} messages", id, montant);

        return MapToResponseDto(client);
    }

    public async Task<bool> DesactiverAsync(int id)
    {
        try
        {
            var client = await _context.Clients.FindAsync(id);

            if (client is null) return false;

            client.EstActif = false;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Client {Id} désactivé", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur DesactiverAsync Client {Id}", id);
            throw;
        }
    }
    private static ClientResponseDto MapToResponseDto(Client client) => new()
    {
        Id = client.Id,
        Nom = client.Nom,
        ApiKey = client.ApiKey,
        SoldeMessages = client.SoldeMessages,
        EstActif = client.EstActif,
        CreatedAt = client.CreatedAt
    };
}