using System.ComponentModel.DataAnnotations;
using ProjetPro.Models;
namespace ProjetPro.DTOs.Message;

public class MessageRequestDto
{
    //L'expéditeur ex: banque, 1xbet
    [Required(ErrorMessage = "L'expéditeur est requis")]
    [MaxLength(50, ErrorMessage = "Maximum 50 caractères")]
    public string Expediteur { get; set; } = string.Empty;

    //Le numéro destinataire ex: "+22961000000"
    [Required(ErrorMessage = "Le destinataire est obligatoire")]
    [RegularExpression(@"^\+?[1-9]\d{7,14}$",
        ErrorMessage = "Numéro de téléphone invalide")]
    public string Destinataire { get; set; } = string.Empty;

        // Le contenu du message
    [Required(ErrorMessage = "Le contenu est obligatoire")]
    [MinLength(1, ErrorMessage = "Le message ne peut pas être vide")]
    [MaxLength(1000, ErrorMessage = "Maximum 1000 caractères")]
    public string Contenu { get; set; } = string.Empty;

  // SMS par défaut si non précisé
    public TypeMessage Type { get; set; } = TypeMessage.SMS;
}

