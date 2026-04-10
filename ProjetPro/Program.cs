//.NET 9 avec MySql
using Microsoft.EntityFrameworkCore;
using ProjetPro.Data;
using ProjetPro.Services.Interfaces;
using ProjectPro.Services.Implementations;

var builder = WebApplication.CreateBuilder(args);

//controllers
builder.Services.AddControllers();

//swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//MySQL + EF Core 9
var connectionStrisg = builder.ConfigurationString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(FileOptions =>
option.UseMySql(
    connectionStrisg,
    ServerVersion.AutoDetect(connectionStrisg)
    )
);// ← détecte la version MySQL auto


//injection de dépendances
builder.Services.AddScoped<IProduitService, ProduitService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
