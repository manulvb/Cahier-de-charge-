namespace GestionMagasin.Models
{
    public class MouvementStock
    {
        public int Id { get; set; }
        public int ProduitId { get; set; }
        public int AuteurId { get; set; }
        public System.DateTime DateHeureUtc { get; set; }
        public string Type { get; set; } = "";
        public int QuantiteSignee { get; set; }
        public string Motif { get; set; } = "";
        public int? LigneVenteId { get; set; }
        public int? AnnulationId { get; set; }
    }
}
