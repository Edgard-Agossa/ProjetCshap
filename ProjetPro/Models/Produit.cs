using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ProjetPro.Models;

[Table("Produit")]
public class Produit
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id {get; set;}

    [Required]
    [MaxLength(100)]
    public string Nom {get; set;} = string.Empty;//= string.Empty → valeur par défaut "" pour éviter un null

    [Column(TypeName = "decimal(10,2)")]//Force le type MySQL à decimal(10,2) → ex: 12345678.99 *10 chiffres au total, 2 après la virgule
    public decimal Prix {get; set;}
    public int Stock {get; set;}

    //audit automatique
    public DateTime CreatedAt {get; set;} = DateTime.UtcNow;
    public DateTime? UpdatedAt {get; set;}
}