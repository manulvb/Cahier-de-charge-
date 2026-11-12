using System;
using System.Security.Cryptography;
using Microsoft.Data.Sqlite;
using GestionMagasin.Models;

namespace GestionMagasin.Services
{
    public class UtilisateurService
    {
        private const int Iterations = 210000;

        public void Ajouter(
            string nom,
            string identifiant,
            string motDePasse,
            string role)
        {
            if (string.IsNullOrWhiteSpace(nom) ||
                string.IsNullOrWhiteSpace(identifiant) ||
                string.IsNullOrWhiteSpace(motDePasse))
                throw new Exception("Tous les champs sont obligatoires.");

            if (role != "ADMIN" && role != "VENDEUR")
                throw new Exception("Rôle invalide.");

            byte[] sel = RandomNumberGenerator.GetBytes(16);

            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                motDePasse,
                sel,
                Iterations,
                HashAlgorithmName.SHA256,
                32
            );

            using var connexion = new SqliteConnection(
                $"Data Source={GestionMagasin.Data.DatabaseInitializer.CheminBase}");

            connexion.Open();

            using var commande = connexion.CreateCommand();

            commande.CommandText = @"
                INSERT INTO Utilisateur
                (Nom, Identifiant, MotDePasseHash, Sel,
                 ParametresHachage, Role, Actif)
                VALUES
                (@nom, @identifiant, @hash, @sel,
                 @parametres, @role, 1);
            ";

            commande.Parameters.AddWithValue("@nom", nom.Trim());
            commande.Parameters.AddWithValue("@identifiant", identifiant.Trim());
            commande.Parameters.AddWithValue("@hash", Convert.ToBase64String(hash));
            commande.Parameters.AddWithValue("@sel", Convert.ToBase64String(sel));
            commande.Parameters.AddWithValue(
                "@parametres", "PBKDF2-SHA256:" + Iterations);
            commande.Parameters.AddWithValue("@role", role);

            commande.ExecuteNonQuery();
        }

        public Utilisateur? Authentifier(
            string identifiant,
            string motDePasse)
        {
            using var connexion = new SqliteConnection(
                $"Data Source={GestionMagasin.Data.DatabaseInitializer.CheminBase}");

            connexion.Open();

            using var commande = connexion.CreateCommand();

            commande.CommandText = @"
                SELECT Id, Nom, Identifiant, MotDePasseHash,
                       Sel, ParametresHachage, Role, Actif
                FROM Utilisateur
                WHERE Identifiant = @identifiant;
            ";

            commande.Parameters.AddWithValue(
                "@identifiant", identifiant.Trim());

            using var lecteur = commande.ExecuteReader();

            if (!lecteur.Read() || lecteur.GetInt32(7) != 1)
                return null;

            string hashEnregistre = lecteur.GetString(3);
            byte[] sel = Convert.FromBase64String(lecteur.GetString(4));

            string parametres = lecteur.GetString(5);

            if (parametres != "PBKDF2-SHA256:" + Iterations)
                return null;

            byte[] hashSaisi = Rfc2898DeriveBytes.Pbkdf2(
                motDePasse,
                sel,
                Iterations,
                HashAlgorithmName.SHA256,
                32
            );

            byte[] hashOriginal = Convert.FromBase64String(hashEnregistre);

            if (!CryptographicOperations.FixedTimeEquals(
                hashSaisi, hashOriginal))
                return null;

            return new Utilisateur
            {
                Id = lecteur.GetInt32(0),
                Nom = lecteur.GetString(1),
                Identifiant = lecteur.GetString(2),
                MotDePasseHash = hashEnregistre,
                Sel = lecteur.GetString(4),
                ParametresHachage = parametres,
                Role = lecteur.GetString(6),
                Actif = true
            };
        }

        // Vérifie si un administrateur actif existe déjà
        public bool ExisteAdministrateur()
        {
            using var connexion = new SqliteConnection(
                $"Data Source={GestionMagasin.Data.DatabaseInitializer.CheminBase}");

            connexion.Open();

            using var commande = connexion.CreateCommand();

            commande.CommandText = @"
                SELECT COUNT(*)
                FROM Utilisateur
                WHERE Role = 'ADMIN' AND Actif = 1;
            ";

            return Convert.ToInt32(
                commande.ExecuteScalar()
            ) > 0;
        }
    }
}
