
using ProjetPro.Models;

namespace ProjetPro.DTOs.Message;

public class MessageResponseDto
{
    public int Id { get; set; }
    public string Expediteur { get; set; } = string.Empty;
    public string Destinataire { get; set; } = string.Empty;
    public string Contenu { get; set; } = string.Empty;

    // On retourne le statut en texte lisible
    public string Statut { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
    public DateTime? EnvoyeAt { get; set; }
    public DateTime? DelivreAt { get; set; }

    public int NombreTentatives { get; set; }

    // On n'expose l'erreur que si elle existe
    public string? ErreurMessage { get; set; }
}