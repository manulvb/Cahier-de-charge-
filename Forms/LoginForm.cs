using System.Drawing;
using System.Windows.Forms;

namespace GestionMagasin.Forms
{
    public class LoginForm : Form
    {
        private TextBox txtIdentifiant;
        private TextBox txtMotDePasse;
        private Button btnConnexion;

        public LoginForm()
        {
            Text = "Connexion - Gestion Magasin";
            Size = new Size(450, 350);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            Label titre = new Label();
            titre.Text = "GESTION MAGASIN";
            titre.Font = new Font("Arial", 18, FontStyle.Bold);
            titre.Location = new Point(105, 30);
            titre.AutoSize = true;

            Label lblIdentifiant = new Label();
            lblIdentifiant.Text = "Identifiant :";
            lblIdentifiant.Location = new Point(60, 110);
            lblIdentifiant.AutoSize = true;

            txtIdentifiant = new TextBox();
            txtIdentifiant.Location = new Point(160, 105);
            txtIdentifiant.Width = 200;

            Label lblMotDePasse = new Label();
            lblMotDePasse.Text = "Mot de passe :";
            lblMotDePasse.Location = new Point(60, 160);
            lblMotDePasse.AutoSize = true;

            txtMotDePasse = new TextBox();
            txtMotDePasse.Location = new Point(160, 155);
            txtMotDePasse.Width = 200;
            txtMotDePasse.PasswordChar = '*';

            btnConnexion = new Button();
            btnConnexion.Text = "Se connecter";
            btnConnexion.Location = new Point(160, 220);
            btnConnexion.Width = 200;

            btnConnexion.Click += (sender, e) =>
            {
                MessageBox.Show(
                    "La vérification des identifiants sera ajoutée prochainement."
                );
            };

            Controls.Add(titre);
            Controls.Add(lblIdentifiant);
            Controls.Add(txtIdentifiant);
            Controls.Add(lblMotDePasse);
            Controls.Add(txtMotDePasse);
            Controls.Add(btnConnexion);
        }
    }
}
