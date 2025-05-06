using System;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace AvionManagment
{
    public partial class AjoutAirport : Form
    {
        // Chaîne de connexion à la base de données
        private string connectionString = "Server=localhost;Database=avionDb;Uid=root;Pwd=;";

        // Contrôles UI
        private Label lblTitle;
        private Label lblNom;
        private TextBox txtNom;
        private Label lblVille;
        private TextBox txtVille;
        private Label lblPays;
        private TextBox txtPays;
        private Button btnSave;
        private Button btnCancel;

        public AjoutAirport()
        {
            InitializeComponent();
            ConfigureForm();
            InitializeCustomControls();
        }

        private void ConfigureForm()
        {
            // Configuration de base du formulaire
            this.BackColor = Color.FromArgb(240, 240, 240);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "X";
            this.ClientSize = new Size(470, 380); // Augmenter légèrement la hauteur

            // Ajouter une ombre à la fenêtre (effet visuel)
            this.FormBorderStyle = FormBorderStyle.None;
            this.ShowInTaskbar = false;
        }

        private void InitializeCustomControls()
        {
            // Titre
            lblTitle = new Label
            {
                Text = "Ajout d'aéroport",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.Black,
                Location = new Point(30, 30),
                AutoSize = true
            };
            this.Controls.Add(lblTitle);

            // Nom de l'aéroport
            lblNom = new Label
            {
                Text = "Nom d'aéroport",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                Location = new Point(30, 90),
                AutoSize = true
            };
            this.Controls.Add(lblNom);

            txtNom = new TextBox
            {
                Location = new Point(30, 115),
                Size = new Size(410, 30),
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(txtNom);

            // Ville
            lblVille = new Label
            {
                Text = "Ville",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                Location = new Point(30, 155),
                AutoSize = true
            };
            this.Controls.Add(lblVille);

            txtVille = new TextBox
            {
                Location = new Point(30, 180),
                Size = new Size(410, 30),
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(txtVille);

            // Pays
            lblPays = new Label
            {
                Text = "Pays",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                Location = new Point(30, 220),
                AutoSize = true
            };
            this.Controls.Add(lblPays);

            txtPays = new TextBox
            {
                Location = new Point(30, 245),
                Size = new Size(410, 30),
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(txtPays);

            // Bouton Annuler
            btnCancel = new Button
            {
                Text = "Annuler",
                Location = new Point(230, 300),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(220, 100, 100), // Rouge clair
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += BtnCancel_Click;
            this.Controls.Add(btnCancel);

            // Bouton Ajouter
            btnSave = new Button
            {
                Text = "Ajouter",
                Location = new Point(340, 300),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(10, 20, 30), // Bleu très foncé/noir
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                Cursor = Cursors.Hand
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);

            // Ajouter un bouton de fermeture en haut à droite
            Button btnClose = new Button
            {
                Text = "×",
                Size = new Size(30, 30),
                Location = new Point(this.ClientSize.Width - 30, 0),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = Color.Black,
                Font = new Font("Arial", 12, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => this.Close();
            this.Controls.Add(btnClose);

            // Ajouter un panel pour créer un effet de bordure
            Panel borderPanel = new Panel
            {
                Size = new Size(this.ClientSize.Width, this.ClientSize.Height),
                Location = new Point(0, 0),
                BackColor = Color.Transparent,
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(borderPanel);
            borderPanel.SendToBack();
        }

        // Ajouter une méthode pour permettre de déplacer la fenêtre sans bordure
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                const int WM_NCLBUTTONDOWN = 0xA1;
                const int HTCAPTION = 0x2;

                // Envoyer un message Windows pour permettre le déplacement de la fenêtre
                NativeMethods.ReleaseCapture();
                NativeMethods.SendMessage(this.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            // Vérifier que tous les champs sont remplis
            if (string.IsNullOrWhiteSpace(txtNom.Text) ||
                string.IsNullOrWhiteSpace(txtVille.Text) ||
                string.IsNullOrWhiteSpace(txtPays.Text))
            {
                MessageBox.Show("Veuillez remplir tous les champs.", "Champs manquants", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Ajouter l'aéroport à la base de données
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "INSERT INTO aeroport (nom_aeroport, ville, pays) VALUES (@nom, @ville, @pays)";
                    MySqlCommand command = new MySqlCommand(query, connection);

                    command.Parameters.AddWithValue("@nom", txtNom.Text.Trim());
                    command.Parameters.AddWithValue("@ville", txtVille.Text.Trim());
                    command.Parameters.AddWithValue("@pays", txtPays.Text.Trim());

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Aéroport ajouté avec succès.", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Erreur lors de l'ajout de l'aéroport.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur: {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }

    // Classe pour les méthodes natives Windows (pour permettre le déplacement de la fenêtre sans bordure)
    internal static class NativeMethods
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        internal static extern bool ReleaseCapture();

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        internal static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
    }
}