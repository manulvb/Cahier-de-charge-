using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

namespace GestionMagasin.Services
{
    public class HistoriqueVenteService
    {
        public List<string> ListerVentes()
        {
            var ventes = new List<string>();

            using var connexion = new SqliteConnection(
                $"Data Source={GestionMagasin.Data.DatabaseInitializer.CheminBase}");

            connexion.Open();

            using var commande = connexion.CreateCommand();

            commande.CommandText = @"
                SELECT Numero, DateMetier, Total,
                       ModePaiement, Statut
                FROM Vente
                ORDER BY DateHeureUtc DESC;
            ";

            using var lecteur = commande.ExecuteReader();

            while (lecteur.Read())
            {
                string numero = lecteur.GetString(0);
                string date = lecteur.GetString(1);
                decimal total = lecteur.GetDecimal(2);
                string paiement = lecteur.GetString(3);
                string statut = lecteur.GetString(4);

                ventes.Add(
                    $"{numero} | {date} | {total:N0} FCFA | {paiement} | {statut}"
                );
            }

            return ventes;
        }
    }
}
