using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using GestionMagasin.Models;

namespace GestionMagasin.Services
{
    public class CategorieService
    {
        public void Ajouter(string nom)
        {
            using var connexion =
                new SqliteConnection(
                    $"Data Source={GestionMagasin.Data.DatabaseInitializer.CheminBase}");

            connexion.Open();

            using var commande = connexion.CreateCommand();

            commande.CommandText = @"
                INSERT INTO Categorie (Nom)
                VALUES (@nom);
            ";

            commande.Parameters.AddWithValue("@nom", nom.Trim());

            commande.ExecuteNonQuery();
        }

        public List<Categorie> Lister()
        {
            var categories = new List<Categorie>();

            using var connexion =
                new SqliteConnection(
                    $"Data Source={GestionMagasin.Data.DatabaseInitializer.CheminBase}");

            connexion.Open();

            using var commande = connexion.CreateCommand();

            commande.CommandText = @"
                SELECT Id, Nom
                FROM Categorie
                ORDER BY Nom;
            ";

            using var lecteur = commande.ExecuteReader();

            while (lecteur.Read())
            {
                categories.Add(new Categorie
                {
                    Id = lecteur.GetInt32(0),
                    Nom = lecteur.GetString(1)
                });
            }

            return categories;
        }
    }
}
