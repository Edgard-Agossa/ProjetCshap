using System.ComponentModel.DataAnnotations;
namespace GestionProduits.DTOs;

public class ProduitRequestDto
{
    [Required(ErrorMessage = "Le nom est obligatoire")]
    [MaxLength(100, ErrorMessage = "Maximum 100 caractères")]
    public string Nom { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le prix est obligatoire")]
    [Range(0.01, 999999, ErrorMessage = "Le prix doit être supérieur à 0")]
    public double Prix {get; set;}

    [Range(0, int.MaxValue, ErrorMessage = "Le stock ne peut pas être négatif")]

    public int Stock {get; set;}

}


public class ProduitResponseDto
{
    public int Id {get; set;}
    public string Nom {get; set;} = string.Empty;
    public double Prix {get; set;}
    public int Stock {get; set;}
}