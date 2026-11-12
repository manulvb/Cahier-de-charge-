using Microsoft.Data.Sqlite;

namespace GestionMagasin.Services
{
    public class TableauDeBordService
    {
        private readonly string chaineConnexion =
            "Data Source=gestion_magasin.db";

        public int NombreProduits()
        {
            using var connexion =
                new SqliteConnection(chaineConnexion);
            connexion.Open();

            using var commande = connexion.CreateCommand();
            commande.CommandText =
                "SELECT COUNT(*) FROM Produit WHERE Actif = 1";

            return Convert.ToInt32(commande.ExecuteScalar());
        }

        public int NombreVentes()
        {
            using var connexion =
                new SqliteConnection(chaineConnexion);
            connexion.Open();

            using var commande = connexion.CreateCommand();
            commande.CommandText =
                "SELECT COUNT(*) FROM Vente WHERE Statut = 'VALIDEE'";

            return Convert.ToInt32(commande.ExecuteScalar());
        }

        public decimal ChiffreAffaires()
        {
            using var connexion =
                new SqliteConnection(chaineConnexion);
            connexion.Open();

            using var commande = connexion.CreateCommand();
            commande.CommandText =
                "SELECT COALESCE(SUM(Total), 0) FROM Vente WHERE Statut = 'VALIDEE'";

            return Convert.ToDecimal(commande.ExecuteScalar());
        }

        public int NombreProduitsEnAlerte()
        {
            using var connexion =
                new SqliteConnection(chaineConnexion);
            connexion.Open();

            using var commande = connexion.CreateCommand();
            commande.CommandText = @"
                SELECT COUNT(*)
                FROM Produit p
                WHERE p.Actif = 1
                AND (
                    SELECT COALESCE(SUM(m.QuantiteSignee), 0)
                    FROM MouvementStock m
                    WHERE m.ProduitId = p.Id
                ) <= p.SeuilAlerte";

            return Convert.ToInt32(commande.ExecuteScalar());
        }
    }
}
