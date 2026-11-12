namespace GestionMagasin.Models
{
    public class JournalAudit
    {
        public int Id { get; set; }
        public int? AuteurId { get; set; }
        public System.DateTime DateHeureUtc { get; set; }
        public string Action { get; set; } = "";
        public string Entite { get; set; } = "";
        public int? EntiteId { get; set; }
        public string Details { get; set; } = "";
    }
}
