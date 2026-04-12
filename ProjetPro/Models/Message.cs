using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetPro.Models;

[Table("Message")]
public class Message
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

//L'expéditeur (ex: "banque", "1xbet" ..)
    [Required]
    [MaxLength(50)]
    public string Expediteur {get; set;} = string.Empty;

//le numéro de du destinataire(ex: "0612345678")
[Required]
[MaxLength(20)]
public string Destinataire {get; set;} = string.Empty;

//le contenu du message
[Required]
[MaxLength(1000)]
public string Contenu {get; set;} = string.Empty;

//Le statut du message
public StatutMessage Statut {get; set;} = StatutMessage.EnAttente;

//le type de message
public TypeMessage Type {get; set;} = TypeMessage.SMS;

public DateTime CreatedAt {get; set;} = DateTime.UtcNow;
public DateTime? EnvoyeAt {get; set;}
public DateTime? DelivreAt {get; set;}

//Nombre de tentatives d'envoi
public int NombreTentatives {get; set;} = 0;
//Message d'erreur si échec
public string? ErreurMessage {get; set;}


}

public enum StatutMessage
{
    EnAttente = 0,//pas encore envoyé
    EnCours = 1,//en cours d'envoi
    Envoye = 2,//envoyé à l'opérateur
    Delivre = 3,//reçu par le téléphone
    Echoue = 4//échec d'envoi
}

public enum TypeMessage
{
    SMS = 0,//sms classique
    RCS = 1,//rcs (nouveau format riche)
    Flash = 2,// sms flash (s'affiche direct à l'écran sans demande d'ouverture)
}