using System;
using System.Drawing;
using System.Windows.Forms;

namespace GestionMagasin.Forms
{
    public class DashboardForm : Form
    {
        public DashboardForm()
        {
            Text = "Gestion Magasin - Tableau de bord";
            Size = new Size(1000, 650);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.White;

            Label titre = new Label();
            titre.Text = "GESTION DE MAGASIN";
            titre.Font = new Font("Arial", 22, FontStyle.Bold);
            titre.Location = new Point(30, 25);
            titre.AutoSize = true;
            Controls.Add(titre);

            Label sousTitre = new Label();
            sousTitre.Text = "Tableau de bord";
            sousTitre.Font = new Font("Arial", 14);
            sousTitre.Location = new Point(35, 80);
            sousTitre.AutoSize = true;
            Controls.Add(sousTitre);

            AjouterBouton("Produits", 35, 150);
            AjouterBouton("Catégories", 250, 150);
            AjouterBouton("Gestion du stock", 465, 150);
            AjouterBouton("Nouvelle vente", 35, 260);
            AjouterBouton("Historique des ventes", 250, 260);
            AjouterBouton("Utilisateurs", 465, 260);
            AjouterBouton("Rapports", 35, 370);
            AjouterBouton("Paramètres", 250, 370);
        }

        private void AjouterBouton(string texte, int x, int y)
        {
            Button bouton = new Button();

            bouton.Text = texte;
            bouton.Size = new Size(190, 65);
            bouton.Location = new Point(x, y);
            bouton.Font = new Font("Arial", 11, FontStyle.Bold);
            bouton.BackColor = Color.LightSteelBlue;

            bouton.Click += (sender, e) =>
            {
                MessageBox.Show(
                    "Le module " + texte + " sera bientôt disponible."
                );
            };

            Controls.Add(bouton);
        }
    }
}
