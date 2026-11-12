namespace GestionMagasin.Models
{
    public class Utilisateur
    {
        public int Id { get; set; }
        public string Nom { get; set; } = "";
        public string Identifiant { get; set; } = "";
        public string MotDePasseHash { get; set; } = "";
        public string Sel { get; set; } = "";
        public string ParametresHachage { get; set; } = "";
        public string Role { get; set; } = "";
        public bool Actif { get; set; } = true;
    }
}
