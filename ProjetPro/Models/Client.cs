using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetPro.Models;

[Table("Client")]
public class Client
{
    [Key]
[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
public int Id {get; set;}

[Required]
[MaxLength(100)]
public string Nom {get; set;} = string.Empty;

//Clé Api unique pour authentifier le client
[Required]
public string ApiKey {get; set; } = Guid.NewGuid().ToString(); //GUID = Identifiant Unique Universel — une chaîne générée aléatoirement, impossible à deviner.

//Solde de message disponibles
public int SoldeMessage {get; set;} = 0;
public bool EstActif {get; set;} = true;
public DateTime CreatedAt {get; set;} = DateTime.UtcNow;

//un client a plusieurs  messages
public ICollection<Message>Messages {get; set;} = new List<Message>();
}
