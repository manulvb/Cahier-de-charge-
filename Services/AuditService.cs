using System;
using Microsoft.Data.Sqlite;

namespace GestionMagasin.Services
{
    public class AuditService
    {
        public void Enregistrer(
            int? auteurId,
            string action,
            string entite,
            int? entiteId,
            string details)
        {
            using var connexion = new SqliteConnection(
                $"Data Source={GestionMagasin.Data.DatabaseInitializer.CheminBase}");

            connexion.Open();

            using var commande = connexion.CreateCommand();

            commande.CommandText = @"
                INSERT INTO JournalAudit
                (AuteurId, DateHeureUtc, Action, Entite, EntiteId, Details)
                VALUES
                (@auteur, @date, @action, @entite, @entiteId, @details);
            ";

            commande.Parameters.AddWithValue(
                "@auteur", (object?)auteurId ?? DBNull.Value);

            commande.Parameters.AddWithValue(
                "@date", DateTime.UtcNow.ToString("o"));

            commande.Parameters.AddWithValue("@action", action);
            commande.Parameters.AddWithValue("@entite", entite);

            commande.Parameters.AddWithValue(
                "@entiteId", (object?)entiteId ?? DBNull.Value);

            commande.Parameters.AddWithValue("@details", details);

            commande.ExecuteNonQuery();
        }
    }
}
