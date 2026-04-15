using System.ComponentModel.DataAnnotations;
using ProjetPro.Models;

namespace ProjetPro.DTOs.Message;

public class MessageMasseRequestDto
{
    // L'expéditeur commun à tous les messages
    [Required(ErrorMessage = "L'expéditeur est obligatoire")]
    [MaxLength(50)]
    public string Expediteur { get; set; } = string.Empty;

    // Liste des destinataires
    [Required(ErrorMessage = "La liste des destinataires est obligatoire")]
    [MinLength(1, ErrorMessage = "Au moins un destinataire requis")]
    public List<string> Destinataires { get; set; } = new();

    // Le contenu commun
    [Required(ErrorMessage = "Le contenu est obligatoire")]
    [MaxLength(1000)]
    public string Contenu { get; set; } = string.Empty;

    public TypeMessage Type { get; set; } = TypeMessage.SMS;

    // Taille de chaque lot (chunk) — 10 par défaut
    [Range(1, 100, ErrorMessage = "La taille du lot doit être entre 1 et 100")]
    public int TailleLot { get; set; } = 10;
}