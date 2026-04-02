using System.ComponentModel.DataAnnotations;

namespace GestionProduits.Models;

public class Produit
{
    public int Id {get; set;}

    [Required(ErrorMessage =  "Le nom est obligatoire")]
    [MaxLength(100, ErrorMessage =  "Le nom ne peut pas dépasser 100 caractères")]
    public string Nom {get; set;} = string.Empty;

    [Required(ErrorMessage = "Le prix est obligatoire")]
    [Range(0.01, 999999, ErrorMessage = "Le prix doit être supérieur à 0")]
    public double Prix {get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Le stock ne peut pas être négatif")]
    public int Stock {get; set;}
}