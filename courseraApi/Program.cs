using System.IO.Pipelines;

var builder = WebApplication.CreateBuilder(args);

// On dit à l'app : "Chaque fois qu'on demande ISmsService, donne OrangeSmsService"
builder.Services.AddScoped<ISmsService, OrangeSemsService>();
builder.Services.AddScoped<IMyService, MyService>();

builder.Services.Configure<OrangeOptions>(
builder.Configuration.GetSection("OrangeConfig")
); //Configuration pro avec le pattern Options

// Test 1 : Scoped (L'ID reste le même pour un clic, mais change si tu rafraîchis la page)
// builder.Services.AddScoped<IMyService, MyService>();

// Test 2 : Transient (L'ID change TOUT LE TEMPS, même au sein d'une seule requête)
// builder.Services.AddTransient<IMyService, MyService>();

// Test 3 : Singleton (L'ID ne change JAMAIS, même si tu fermes et rouvres ton navigateur)
// builder.Services.AddSingleton<IMyService, MyService>();

var app = builder.Build();

//on active notre douane personnalisé (middelware)

// 1. D'abord le filet de sécurité global (Erreurs)
app.UseMiddleware<ErrorHandlingMiddleware>();
// 2. Ensuite la sécurité (On bloque les intrus immédiatement)
app.UseMiddleware<SecurityMiddleware>();
// 3. Enfin le log (On ne logge que ce qui a passé la sécurité)
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseHttpsRedirection();

app.MapGet("/test-sms", async (ISmsService smsService) =>
{
   await smsService.SendTransactionSms("+2290166589049", 5000);
   return Results.Ok("Le test a été lancé, console !");
});

app.MapGet("/test-logcreation", async (IMyService myService) =>
{
   //On utilise le nom de la variable 'myService'
   //On retire 'await' car LogCreation est 'void'
   myService.LogCreation("Moov");
   return Results.Ok("ok");
});

app.MapGet("/crash", () =>
{
   throw new Exception("Explosion de la base de données !");
});


app.MapPost("/send-bulk-sms", async(ISmsService smsService) =>
{
   List<string> phoneList = new List<string> {"+224621000001", "+224621000002", "+224621000003"};
   // string message = "Alerte Orange : Votre solde est faible.";

   var tasks = new List<Task>();

   foreach (string phone in phoneList)
   {
      //on lance les tâches et on les stocke
      tasks.Add(smsService.SendTransactionSms(phone, 0));//0 car c'est juste un test
   }
   // On attend que tout le monde ait fini
   await Task.WhenAll(tasks);

   return Results.Ok("Tous les SMS ont été transmis au reseau !");

});

app.Run();

