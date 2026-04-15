// DTOs/Message/MessageMasseResultatDto.cs
namespace ProjetPro.DTOs.Message;

public class MessageMasseResultatDto
{
    // Nombre total de messages à envoyer
    public int TotalDemande { get; set; }

    // Nombre de messages envoyés avec succès
    public int TotalSucces { get; set; }

    // Nombre de messages échoués
    public int TotalEchecs { get; set; }

    // Détail des échecs
    public List<string> Echecs { get; set; } = new();

    // Temps d'exécution en millisecondes
    public long TempsExecutionMs { get; set; }
}