using System;
using System.Windows.Forms;
using GestionMagasin.Data;
using GestionMagasin.Forms;
using GestionMagasin.Services;

namespace GestionMagasin
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            DatabaseInitializer.Initialiser();

            var utilisateurService = new UtilisateurService();

            if (!utilisateurService.ExisteAdministrateur())
            {
                utilisateurService.Ajouter(
                    "Administrateur",
                    "admin",
                    "Admin@123",
                    "ADMIN"
                );
            }

            Application.Run(new LoginForm());
        }
    }
}
