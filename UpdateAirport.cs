using System;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace AvionManagment
{
    public partial class UpdateAirport : Form
    {
        // Propriétés pour stocker les données de l'aéroport
        public int AirportId { get; set; }
        public string NomAeroport { get; set; }
        public string Ville { get; set; }
        public string Pays { get; set; }

        // Chaîne de connexion à la base de données
        private readonly string connectionString = "Server=localhost;Database=avionDb;Uid=root;Pwd=;";

        // Déclaration des contrôles
        private TextBox txtNom;
        private TextBox txtVille;
        private TextBox txtPays;
        private Button btnUpdate;
        private Button btnCancel;

        public UpdateAirport()
        {
            InitializeComponent();
            InitializeControls();
        }

        // Initialisation des contrôles manquants
        private void InitializeControls()
        {
            // Champ pour le nom de l'aéroport
            txtNom = new TextBox
            {
                Location = new Point(30, 50),
                Size = new Size(200, 30),
                Font = new Font("Segoe UI", 10)
            };
            this.Controls.Add(txtNom);

            // Champ pour la ville
            txtVille = new TextBox
            {
                Location = new Point(30, 100),
                Size = new Size(200, 30),
                Font = new Font("Segoe UI", 10)
            };
            this.Controls.Add(txtVille);

            // Champ pour le pays
            txtPays = new TextBox
            {
                Location = new Point(30, 150),
                Size = new Size(200, 30),
                Font = new Font("Segoe UI", 10)
            };
            this.Controls.Add(txtPays);

            // Bouton pour mettre à jour
            btnUpdate = new Button
            {
                Text = "Mettre à jour",
                Location = new Point(30, 200),
                Size = new Size(100, 30)
            };
            btnUpdate.Click += BtnUpdate_Click;
            this.Controls.Add(btnUpdate);

            // Bouton pour annuler
            btnCancel = new Button
            {
                Text = "Annuler",
                Location = new Point(150, 200),
                Size = new Size(100, 30)
            };
            btnCancel.Click += BtnCancel_Click;
            this.Controls.Add(btnCancel);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Afficher les valeurs dans les champs correspondants
            txtNom.Text = NomAeroport;
            txtVille.Text = Ville;
            txtPays.Text = Pays;
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            // Récupérer les valeurs mises à jour
            string updatedNom = txtNom.Text.Trim();
            string updatedVille = txtVille.Text.Trim();
            string updatedPays = txtPays.Text.Trim();

            // Vérifier si des modifications ont été faites
            if (updatedNom == NomAeroport && updatedVille == Ville && updatedPays == Pays)
            {
                MessageBox.Show("Aucune modification détectée.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "UPDATE aeroport SET nom_aeroport = @nom, ville = @ville, pays = @pays WHERE id_aeroport = @id";
                    MySqlCommand command = new MySqlCommand(query, connection);

                    // Ajouter les paramètres à la commande
                    command.Parameters.AddWithValue("@nom", updatedNom);
                    command.Parameters.AddWithValue("@ville", updatedVille);
                    command.Parameters.AddWithValue("@pays", updatedPays);
                    command.Parameters.AddWithValue("@id", AirportId);

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Aéroport mis à jour avec succès.", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Aucune mise à jour effectuée.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la mise à jour de l'aéroport: {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Fermer le formulaire et retourner un résultat OK
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            // Fermer le formulaire sans appliquer de modifications
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}