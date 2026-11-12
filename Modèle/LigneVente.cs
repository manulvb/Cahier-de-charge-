namespace GestionMagasin.Models
{
    public class LigneVente
    {
        public int Id { get; set; }
        public int VenteId { get; set; }
        public int ProduitId { get; set; }
        public string ReferenceSnapshot { get; set; } = "";
        public string NomSnapshot { get; set; } = "";
        public int Quantite { get; set; }
        public decimal PrixUnitaire { get; set; }
        public decimal SousTotal { get; set; }
    }
}
