using System;
using Microsoft.Data.Sqlite;

namespace GestionMagasin.Services
{
    public class StockService
    {
        public int ObtenirStock(int produitId)
        {
            using var connexion = new SqliteConnection(
                $"Data Source={GestionMagasin.Data.DatabaseInitializer.CheminBase}");

            connexion.Open();

            using var commande = connexion.CreateCommand();

            commande.CommandText = @"
                SELECT COALESCE(SUM(QuantiteSignee), 0)
                FROM MouvementStock
                WHERE ProduitId = @produitId;
            ";

            commande.Parameters.AddWithValue("@produitId", produitId);

            return Convert.ToInt32(commande.ExecuteScalar());
        }

        public void EnregistrerEntree(
            int produitId,
            int auteurId,
            int quantite,
            string motif)
        {
            EnregistrerMouvement(
                produitId, auteurId, quantite, "ENTREE", motif);
        }

        public void EnregistrerSortie(
            int produitId,
            int auteurId,
            int quantite,
            string motif)
        {
            EnregistrerMouvement(
                produitId, auteurId, -quantite, "SORTIE", motif);
        }

        private void EnregistrerMouvement(
            int produitId,
            int auteurId,
            int quantiteSignee,
            string type,
            string motif)
        {
            if (quantiteSignee == 0)
                throw new ArgumentException(
                    "La quantité doit être supérieure à zéro.");

            if (string.IsNullOrWhiteSpace(motif))
                throw new ArgumentException(
                    "Le motif est obligatoire.");

            using var connexion = new SqliteConnection(
                $"Data Source={GestionMagasin.Data.DatabaseInitializer.CheminBase}");

            connexion.Open();

            using var transaction = connexion.BeginTransaction();

            using var commandeStock = connexion.CreateCommand();
            commandeStock.Transaction = transaction;

            commandeStock.CommandText = @"
                SELECT COALESCE(SUM(QuantiteSignee), 0)
                FROM MouvementStock
                WHERE ProduitId = @produitId;
            ";

            commandeStock.Parameters.AddWithValue(
                "@produitId", produitId);

            int stockActuel = Convert.ToInt32(
                commandeStock.ExecuteScalar());

            if (stockActuel + quantiteSignee < 0)
                throw new InvalidOperationException(
                    "Stock insuffisant pour effectuer cette sortie.");

            using var commande = connexion.CreateCommand();
            commande.Transaction = transaction;

            commande.CommandText = @"
                INSERT INTO MouvementStock
                (ProduitId, AuteurId, DateHeureUtc, Type,
                 QuantiteSignee, Motif)
                VALUES
                (@produitId, @auteurId, @date, @type,
                 @quantite, @motif);
            ";

            commande.Parameters.AddWithValue(
                "@produitId", produitId);

            commande.Parameters.AddWithValue(
                "@auteurId", auteurId);

            commande.Parameters.AddWithValue(
                "@date", DateTime.UtcNow.ToString("o"));

            commande.Parameters.AddWithValue("@type", type);

            commande.Parameters.AddWithValue(
                "@quantite", quantiteSignee);

            commande.Parameters.AddWithValue("@motif", motif.Trim());

            commande.ExecuteNonQuery();

            transaction.Commit();
        }
    }
}
