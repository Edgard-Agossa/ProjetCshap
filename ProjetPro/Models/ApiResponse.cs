namespace ProjetPro.Models;

public class ApiResponse<T>
{
    public bool Succes {get; set;}
    public string Message {get; set;} = string.Empty;
    public T? Data {get; set;}
    public DateTime Timestamp {get; set;} = DateTime.UtcNow;

    public static ApiResponse<T> Ok(T data, string message ="Opération réussie") => new() {
        Succes = true,
        Data = data,
        Message = message
    };
   public static ApiResponse<T> Erreur(string message)
        => new() { Succes = false, Message = message, Data = default };
}