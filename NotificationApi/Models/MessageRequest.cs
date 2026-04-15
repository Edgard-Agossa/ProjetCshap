using System.ComponentModel.DataAnnotations;

namespace NotificationApi.Models;

public class MessageRequest
{
    [Required(ErrorMessage ="Le message est obligatoire")]
    [StringLength(500, MinimumLength =3)]
  public string Content {get; set;} = string.Empty;
}