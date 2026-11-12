namespace GestionMagasin.Models
{
    public class MouvementStock
    {
        public int Id { get; set; }
        public int ProduitId { get; set; }
        public int UtilisateurId { get; set; }
        public string TypeMouvement { get; set; } = "";
        public int Quantite { get; set; }
        public DateTime DateMouvement { get; set; }
        public string Motif { get; set; } = "";
    }
}
