namespace GestionMagasin.Models
{
    public class Vente
    {
        public int Id { get; set; }
        public string Numero { get; set; } = "";
        public string JetonValidation { get; set; } = "";
        public System.DateTime DateHeureUtc { get; set; }
        public System.DateTime DateMetier { get; set; }
        public int VendeurId { get; set; }
        public decimal Total { get; set; }
        public string ModePaiement { get; set; } = "";
        public decimal MontantRecu { get; set; }
        public decimal MonnaieRendue { get; set; }
        public string Statut { get; set; } = "";
    }
}
