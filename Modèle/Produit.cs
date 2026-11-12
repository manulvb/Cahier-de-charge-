namespace GestionMagasin.Models
{
    public class Produit
    {
        public int Id { get; set; }
        public string Nom { get; set; } = "";
        public string Reference { get; set; } = "";
        public decimal PrixAchat { get; set; }
        public decimal PrixVente { get; set; }
        public int QuantiteStock { get; set; }
        public int SeuilAlerte { get; set; }
    }
}
