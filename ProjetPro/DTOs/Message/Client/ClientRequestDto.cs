using System.ComponentModel.DataAnnotations;

namespace ProjetPro.DTOs.Client;

public class ClientRequestDto
{
    [Required(ErrorMessage = "Le nom du client est obligatoire")]
    [MinLength(2, ErrorMessage = "Minimum 2 caractères")]
    [MaxLength(100, ErrorMessage = "Maximum 100 caractères")]
    public string Nom { get; set; } = string.Empty;

    // Solde initial de messages
    [Range(0, int.MaxValue, ErrorMessage = "Le solde ne peut pas être négatif")]
    public int SoldeMessages { get; set; } = 0;
}