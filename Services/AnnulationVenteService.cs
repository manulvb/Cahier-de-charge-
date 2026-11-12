using System;
using Microsoft.Data.Sqlite;

namespace GestionMagasin.Services
{
    public class AnnulationVenteService
    {
        public void Annuler(
            int venteId,
            int auteurId,
            string motif)
        {
            if (string.IsNullOrWhiteSpace(motif))
                throw new Exception("Le motif est obligatoire.");

            using var connexion = new SqliteConnection(
                $"Data Source={GestionMagasin.Data.DatabaseInitializer.CheminBase}");

            connexion.Open();

            using var transaction = connexion.BeginTransaction();

            using var verification = connexion.CreateCommand();
            verification.Transaction = transaction;

            verification.CommandText = @"
                SELECT Statut
                FROM Vente
                WHERE Id = @id;
            ";

            verification.Parameters.AddWithValue("@id", venteId);

            object? resultat = verification.ExecuteScalar();

            if (resultat == null)
                throw new Exception("Vente introuvable.");

            if (resultat.ToString() != "VALIDEE")
                throw new Exception("Cette vente ne peut pas être annulée.");

            using var annulation = connexion.CreateCommand();
            annulation.Transaction = transaction;

            annulation.CommandText = @"
                INSERT INTO AnnulationVente
                (VenteId, AuteurId, DateHeureUtc, Motif)
                VALUES
                (@vente, @auteur, @date, @motif);

                SELECT last_insert_rowid();
            ";

            annulation.Parameters.AddWithValue("@vente", venteId);
            annulation.Parameters.AddWithValue("@auteur", auteurId);
            annulation.Parameters.AddWithValue(
                "@date", DateTime.UtcNow.ToString("o"));
            annulation.Parameters.AddWithValue("@motif", motif.Trim());

            int annulationId =
                Convert.ToInt32(annulation.ExecuteScalar());

            using var lignes = connexion.CreateCommand();
            lignes.Transaction = transaction;

            lignes.CommandText = @"
                SELECT ProduitId, Quantite, Id
                FROM LigneVente
                WHERE VenteId = @vente;
            ";

            lignes.Parameters.AddWithValue("@vente", venteId);

            using var lecteur = lignes.ExecuteReader();

            var articles = new System.Collections.Generic.List<
                (int ProduitId, int Quantite, int LigneId)>();

            while (lecteur.Read())
            {
                articles.Add((
                    lecteur.GetInt32(0),
                    lecteur.GetInt32(1),
                    lecteur.GetInt32(2)
                ));
            }

            lecteur.Close();

            foreach (var article in articles)
            {
                using var mouvement = connexion.CreateCommand();
                mouvement.Transaction = transaction;

                mouvement.CommandText = @"
                    INSERT INTO MouvementStock
                    (ProduitId, AuteurId, DateHeureUtc, Type,
                     QuantiteSignee, Motif, AnnulationId)
                    VALUES
                    (@produit, @auteur, @date, 'ANNULATION',
                     @quantite, @motif, @annulation);
                ";

                mouvement.Parameters.AddWithValue(
                    "@produit", article.ProduitId);

                mouvement.Parameters.AddWithValue("@auteur", auteurId);

                mouvement.Parameters.AddWithValue(
                    "@date", DateTime.UtcNow.ToString("o"));

                mouvement.Parameters.AddWithValue(
                    "@quantite", article.Quantite);

                mouvement.Parameters.AddWithValue(
                    "@motif", "Annulation de la vente " + venteId);

                mouvement.Parameters.AddWithValue(
                    "@annulation", annulationId);

                mouvement.ExecuteNonQuery();
            }

            using var modifier = connexion.CreateCommand();
            modifier.Transaction = transaction;

            modifier.CommandText = @"
                UPDATE Vente
                SET Statut = 'ANNULEE'
                WHERE Id = @id;
            ";

            modifier.Parameters.AddWithValue("@id", venteId);
            modifier.ExecuteNonQuery();

            transaction.Commit();
        }
    }
}
