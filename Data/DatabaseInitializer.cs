using System;
using System.IO;
using Microsoft.Data.Sqlite;

namespace GestionMagasin.Data
{
    public static class DatabaseInitializer
    {
        private static readonly string Dossier =
            Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData),
                "GestionMagasin"
            );

        public static string CheminBase =>
            Path.Combine(Dossier, "gestion_magasin.db");

        public static void Initialiser()
        {
            Directory.CreateDirectory(Dossier);

            using var connexion =
                new SqliteConnection($"Data Source={CheminBase}");

            connexion.Open();

            using var commande = connexion.CreateCommand();

            commande.CommandText = @"
                PRAGMA foreign_keys = ON;
            ";

            commande.ExecuteNonQuery();
        }
    }
}
