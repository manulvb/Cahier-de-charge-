using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using GestionMagasin.Models;

namespace GestionMagasin.Services
{
    public class ProduitService
    {
        public void Ajouter(Produit produit)
        {
            using var connexion =
                new SqliteConnection(
                    $"Data Source={GestionMagasin.Data.DatabaseInitializer.CheminBase}");

            connexion.Open();

            using var commande = connexion.CreateCommand();

            commande.CommandText = @"
                INSERT INTO Produit
                (Reference, Nom, CategorieId, PrixVente,
                 SeuilAlerte, Actif, DateCreation)
                VALUES
                (@reference, @nom, @categorie, @prix,
                 @seuil, @actif, @date);
            ";

            commande.Parameters.AddWithValue(
                "@reference", produit.Reference);

            commande.Parameters.AddWithValue(
                "@nom", produit.Nom);

            commande.Parameters.AddWithValue(
                "@categorie",
                produit.CategorieId.HasValue
                    ? produit.CategorieId.Value
                    : DBNull.Value);

            commande.Parameters.AddWithValue(
                "@prix", produit.PrixVente);

            commande.Parameters.AddWithValue(
                "@seuil", produit.SeuilAlerte);

            commande.Parameters.AddWithValue(
                "@actif", produit.Actif ? 1 : 0);

            commande.Parameters.AddWithValue(
                "@date", DateTime.UtcNow.ToString("o"));

            commande.ExecuteNonQuery();
        }

        public List<Produit> Lister()
        {
            var produits = new List<Produit>();

            using var connexion =
                new SqliteConnection(
                    $"Data Source={GestionMagasin.Data.DatabaseInitializer.CheminBase}");

            connexion.Open();

            using var commande = connexion.CreateCommand();

            commande.CommandText = @"
                SELECT Id, Reference, Nom, CategorieId,
                       PrixVente, SeuilAlerte, Actif, DateCreation
                FROM Produit
                ORDER BY Nom;
            ";

            using var lecteur = commande.ExecuteReader();

            while (lecteur.Read())
            {
                produits.Add(new Produit
                {
                    Id = lecteur.GetInt32(0),
                    Reference = lecteur.GetString(1),
                    Nom = lecteur.GetString(2),
                    CategorieId = lecteur.IsDBNull(3)
                        ? null
                        : lecteur.GetInt32(3),
                    PrixVente = lecteur.GetDecimal(4),
                    SeuilAlerte = lecteur.GetInt32(5),
                    Actif = lecteur.GetInt32(6) == 1,
                    DateCreation = DateTime.Parse(
                        lecteur.GetString(7))
                });
            }

            return produits;
        }
    }
}
