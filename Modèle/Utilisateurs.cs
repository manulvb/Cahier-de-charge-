namespace GestionMagasin.Models
{
    public class Utilisateur
    {
        public int Id { get; set; }
        public string NomUtilisateur { get; set; } = "";
        public string MotDePasse { get; set; } = "";
        public string NomComplet { get; set; } = "";
        public string Role { get; set; } = "";
        public bool Actif { get; set; } = true;
    }
}
