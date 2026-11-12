namespace GestionMagasin.Models
{
    public class Produit
    {
        public int Id { get; set; }
        public string Reference { get; set; } = "";
        public string Nom { get; set; } = "";
        public int? CategorieId { get; set; }
        public decimal PrixVente { get; set; }
        public int SeuilAlerte { get; set; }
        public bool Actif { get; set; } = true;
        public System.DateTime DateCreation { get; set; }
    }
}
