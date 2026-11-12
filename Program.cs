using System;
using System.Windows.Forms;
using GestionMagasin.Forms;
using GestionMagasin.Data;

namespace GestionMagasin
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            try
            {
                DatabaseInitializer.Initialiser();

                Application.Run(new LoginForm());
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Erreur au démarrage du logiciel : " + ex.Message,
                    "Gestion Magasin",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
    }
}
