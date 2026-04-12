
namespace ProjetPro.DTOs.Client;

public class ClientResponseDto
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;

    // On retourne la ApiKey au client pour qu'il puisse l'utiliser
    public string ApiKey { get; set; } = string.Empty;
    public int SoldeMessages { get; set; }
    public bool EstActif { get; set; }
    public DateTime CreatedAt { get; set; }
}