using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace AvionManagment
{
    public partial class UpdateUser : Form
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public int UserId { get; set; }

        // Chaîne de connexion à la base de données
        private readonly string connectionString = "Server=localhost;Database=avionDb;Uid=root;Pwd=;";

        public UpdateUser()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Afficher les valeurs dans les champs correspondants
            usernameUpdate.Text = Username;
            passwordUpdate.Text = Password;
        }

        private void UpdateUserBtn_Click(object sender, EventArgs e)
        {
            // Récupérer les valeurs mises à jour
            string updatedUsername = usernameUpdate.Text;
            string updatedPassword = passwordUpdate.Text;

            // Vérifier si des modifications ont été faites
            if (updatedUsername == Username && updatedPassword == Password)
            {
                MessageBox.Show("Aucune modification détectée.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "UPDATE utilisateur SET username = @username, password = @password WHERE id_user = @id";
                    MySqlCommand command = new MySqlCommand(query, connection);

                    // Ajouter les paramètres à la commande
                    command.Parameters.AddWithValue("@username", updatedUsername);
                    command.Parameters.AddWithValue("@password", updatedPassword);
                    command.Parameters.AddWithValue("@id", UserId);

                    // Debug : Afficher les paramètres
                    MessageBox.Show($"ID: {UserId}, Username: {updatedUsername}, Password: {updatedPassword}");

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Utilisateur mis à jour avec succès.", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Aucune mise à jour effectuée.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la mise à jour de l'utilisateur: {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Fermer le formulaire et retourner un résultat OK
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}