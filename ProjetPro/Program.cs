//.NET 9 avec MySql
using Microsoft.EntityFrameworkCore;
using ProjetPro.Data;
using Microsoft.OpenApi.Models;
// using ProjetPro.Services.Interfaces;
// using ProjectPro.Services.Implementations;

var builder = WebApplication.CreateBuilder(args);

//controllers
builder.Services.AddControllers();

//swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//MySQL + EF Core 9
var connectionStrisg = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(option =>
option.UseMySql(
    connectionStrisg,
    ServerVersion.AutoDetect(connectionStrisg)
    )
);//détecte la version MySQL auto


//injection de dépendances
// builder.Services.AddScoped<IProduitService, ProduitService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

