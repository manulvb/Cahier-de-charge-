namespace GestionMagasin.Models
{
    public class AnnulationVente
    {
        public int Id { get; set; }
        public int VenteId { get; set; }
        public int AuteurId { get; set; }
        public System.DateTime DateHeureUtc { get; set; }
        public string Motif { get; set; } = "";
    }
}
