namespace GestionMagasin.Models
{
    public class Vente
    {
        public int Id { get; set; }
        public int UtilisateurId { get; set; }
        public DateTime DateVente { get; set; }
        public decimal MontantTotal { get; set; }
        public decimal MontantPaye { get; set; }
        public decimal MonnaieRendue { get; set; }
        public string ModePaiement { get; set; } = "";
        public string Statut { get; set; } = "";
    }
}
