using Microsoft.EntityFrameworkCore;
using GestionProduits.Data;
using GestionProduits.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Base de données
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=gestion.db"));

// Injection de dépendances — le cœur des microservices
// "Quand quelqu'un demande IProduitService, donne-lui ProduitService"
builder.Services.AddScoped<IProduitService, ProduitService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();