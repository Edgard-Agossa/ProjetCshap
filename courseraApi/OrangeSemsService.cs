public class OrangeSemsService : ISmsService
{

    private readonly IMyService _myservice;// On crée une place pour stocker le service 

    // LE CONSTRUCTEUR : .NET va voir qu'on demande IMyService et va nous l'injecter ici
    public OrangeSemsService(IMyService myservice)
    {
        _myservice = myservice;
    }

    public async Task SendTransactionSms(string phoneNumber, decimal amount)
    {

        _myservice.LogCreation($"Préparation de l'envoi pour {phoneNumber}");


        // List<string> numeros = new List<string> { "5122", "56525", "45556" };
        // foreach (string numero in numeros)
        // {
        //     Console.WriteLine($"Envoi à {numero}");
        // }

        Console.WriteLine($"[Orange Money] SMS envoyé au {phoneNumber} : Votre paiement de {amount} a été reçu avec succès");
        // Comme c'est une méthode Task, on simule une attente asynchrone (optionnel)
        await Task.CompletedTask;
    }
}