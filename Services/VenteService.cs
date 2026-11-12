using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

namespace GestionMagasin.Services
{
    public class VenteService
    {
        public int EnregistrerVente(
            int vendeurId,
            string modePaiement,
            decimal montantRecu,
            List<(int ProduitId, int Quantite)> panier)
        {
            if (panier == null || panier.Count == 0)
                throw new Exception("Le panier est vide.");

            using var connexion = new SqliteConnection(
                $"Data Source={GestionMagasin.Data.DatabaseInitializer.CheminBase}");

            connexion.Open();

            using var transaction = connexion.BeginTransaction();

            decimal total = 0;
            var details = new List<(int Id, string Reference,
                string Nom, int Quantite, decimal Prix, decimal SousTotal)>();

            foreach (var article in panier)
            {
                if (article.Quantite <= 0)
                    throw new Exception("Quantité invalide.");

                using var produit = connexion.CreateCommand();
                produit.Transaction = transaction;

                produit.CommandText = @"
                    SELECT Reference, Nom, PrixVente, Actif
                    FROM Produit
                    WHERE Id = @id;
                ";

                produit.Parameters.AddWithValue("@id", article.ProduitId);

                using var lecteur = produit.ExecuteReader();

                if (!lecteur.Read())
                    throw new Exception("Produit introuvable.");

                string reference = lecteur.GetString(0);
                string nom = lecteur.GetString(1);
                decimal prix = lecteur.GetDecimal(2);
                bool actif = lecteur.GetInt32(3) == 1;

                lecteur.Close();

                if (!actif)
                    throw new Exception("Produit désactivé : " + nom);

                using var stock = connexion.CreateCommand();
                stock.Transaction = transaction;

                stock.CommandText = @"
                    SELECT COALESCE(SUM(QuantiteSignee), 0)
                    FROM MouvementStock
                    WHERE ProduitId = @id;
                ";

                stock.Parameters.AddWithValue("@id", article.ProduitId);

                int quantiteStock = Convert.ToInt32(stock.ExecuteScalar());

                if (quantiteStock < article.Quantite)
                    throw new Exception("Stock insuffisant : " + nom);

                decimal sousTotal = prix * article.Quantite;
                total += sousTotal;

                details.Add((article.ProduitId, reference, nom,
                    article.Quantite, prix, sousTotal));
            }

            if (montantRecu < total)
                throw new Exception("Montant reçu insuffisant.");

            string numero = "VTE-" +
                DateTime.Now.ToString("yyyyMMddHHmmss") +
                "-" + Guid.NewGuid().ToString("N").Substring(0, 6);

            string jeton = Guid.NewGuid().ToString("N");
            string dateUtc = DateTime.UtcNow.ToString("o");
            string dateMetier = DateTime.Now.ToString("yyyy-MM-dd");

            using var vente = connexion.CreateCommand();
            vente.Transaction = transaction;

            vente.CommandText = @"
                INSERT INTO Vente
                (Numero, JetonValidation, DateHeureUtc, DateMetier,
                 VendeurId, Total, ModePaiement, MontantRecu,
                 MonnaieRendue, Statut)
                VALUES
                (@numero, @jeton, @date, @dateMetier,
                 @vendeur, @total, @mode, @recu,
                 @monnaie, 'VALIDEE');

                SELECT last_insert_rowid();
            ";

            vente.Parameters.AddWithValue("@numero", numero);
            vente.Parameters.AddWithValue("@jeton", jeton);
            vente.Parameters.AddWithValue("@date", dateUtc);
            vente.Parameters.AddWithValue("@dateMetier", dateMetier);
            vente.Parameters.AddWithValue("@vendeur", vendeurId);
            vente.Parameters.AddWithValue("@total", total);
            vente.Parameters.AddWithValue("@mode", modePaiement);
            vente.Parameters.AddWithValue("@recu", montantRecu);
            vente.Parameters.AddWithValue("@monnaie", montantRecu - total);

            int venteId = Convert.ToInt32(vente.ExecuteScalar());

            foreach (var article in details)
            {
                using var ligne = connexion.CreateCommand();
                ligne.Transaction = transaction;

                ligne.CommandText = @"
                    INSERT INTO LigneVente
                    (VenteId, ProduitId, ReferenceSnapshot,
                     NomSnapshot, Quantite, PrixUnitaire, SousTotal)
                    VALUES
                    (@vente, @produit, @reference,
                     @nom, @quantite, @prix, @sousTotal);

                    SELECT last_insert_rowid();
                ";

                ligne.Parameters.AddWithValue("@vente", venteId);
                ligne.Parameters.AddWithValue("@produit", article.Id);
                ligne.Parameters.AddWithValue("@reference", article.Reference);
                ligne.Parameters.AddWithValue("@nom", article.Nom);
                ligne.Parameters.AddWithValue("@quantite", article.Quantite);
                ligne.Parameters.AddWithValue("@prix", article.Prix);
                ligne.Parameters.AddWithValue("@sousTotal", article.SousTotal);

                int ligneId = Convert.ToInt32(ligne.ExecuteScalar());

                using var mouvement = connexion.CreateCommand();
                mouvement.Transaction = transaction;

                mouvement.CommandText = @"
                    INSERT INTO MouvementStock
                    (ProduitId, AuteurId, DateHeureUtc, Type,
                     QuantiteSignee, Motif, LigneVenteId)
                    VALUES
                    (@produit, @auteur, @date, 'VENTE',
                     @quantite, @motif, @ligne);
                ";

                mouvement.Parameters.AddWithValue("@produit", article.Id);
                mouvement.Parameters.AddWithValue("@auteur", vendeurId);
                mouvement.Parameters.AddWithValue("@date", dateUtc);
                mouvement.Parameters.AddWithValue("@quantite", -article.Quantite);
                mouvement.Parameters.AddWithValue("@motif", "Vente " + numero);
                mouvement.Parameters.AddWithValue("@ligne", ligneId);

                mouvement.ExecuteNonQuery();
            }

            transaction.Commit();

            return venteId;
        }
    }
}
