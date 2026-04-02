using Microsoft.EntityFrameworkCore;
using GestionProduits.Data;
using GestionProduits.DTOs;
using GestionProduits.Models;

namespace GestionProduits.Services;

public class ProduitService : IProduitService

{
    private readonly AppDbContext _context; //C'est l'injection de dépendances.
    private readonly ILogger<ProduitService> _logger;
    public ProduitService(AppDbContext context, ILogger<ProduitService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<ProduitResponseDto>> GetTousAsync()
    {
        try
        {
            var produits = await _context.Produits.ToListAsync();
            return produits.Select(p => new ProduitResponseDto
            {
                Id = p.Id,
                Nom = p.Nom,
                Prix = p.Prix,
                Stock = p.Stock,
            }).ToList();

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des produits");
            throw;
        }
    }

    public async Task<ProduitResponseDto?> GetParIdAsync(int id)
    {
        try
        {
            var produit = await _context.Produits.FindAsync(id);

            if (produit == null) return null;

            return new ProduitResponseDto
            {
                Id = produit.Id,
                Nom = produit.Nom,
                Prix = produit.Prix,
                Stock = produit.Stock,

            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération du produit d'ID : {Id}", 1);
            throw;
        }
    }

    public async Task<ProduitResponseDto> CreerAsync(ProduitRequestDto dto)
    {
        try
        {
            var produit = new Produit
            {
                Nom = dto.Nom,
                Prix = dto.Prix,
                Stock = dto.Stock
            };

            _context.Produits.Add(produit);
            await _context.SaveChangesAsync();

            return new ProduitResponseDto
            {
                Id = produit.Id,
                Nom = produit.Nom,
                Prix = produit.Prix,
                Stock = produit.Stock
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la création d'un produit");
            throw;
        }

    }

    public async Task<ProduitResponseDto?> ModifierAsync(int id, ProduitRequestDto dto)
    {
        try
        {
            var produit = await _context.Produits.FindAsync(id);

            if (produit == null) return null;

            // Mise à jour des champs
            produit.Nom = dto.Nom;
            produit.Prix = dto.Prix;
            produit.Stock = dto.Stock;

            await _context.SaveChangesAsync();

            return new ProduitResponseDto
            {
                Id = produit.Id,
                Nom = produit.Nom,
                Prix = produit.Prix,
                Stock = produit.Stock
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la modification du produit {Id}", id);
            throw;
        }
    }
    public async Task<bool> SupprimerAsync(int id)
    {
        try
        {
            var produit = await _context.Produits.FindAsync(id);

            if (produit == null) return false;

            _context.Produits.Remove(produit);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la suppression du produit {Id}", id);
            throw;
        }
    }
}