var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddHttpClient();
builder.Services.AddControllers();

var app = builder.Build();




app.UseHttpsRedirection();




app.MapControllers();
app.Run();


