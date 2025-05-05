using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace AvionManagment
{
    public partial class AjoutUser : Form
    {
        public string NewUsername { get; private set; } // Propriété pour récupérer le nom d'utilisateur
        public string NewPassword { get; private set; } // Propriété pour récupérer le mot de passe

        public AjoutUser()
        {
            InitializeComponent();
        }

        private void ajoutBtn_Click(object sender, EventArgs e)
        {
            // Récupérer les valeurs des champs de texte
            string usernameValue = username.Text;
            string passwordValue = password.Text;
            string passwordConfirmValue = passwordConfirm.Text;

            // Vérifier si les champs sont vides
            if (string.IsNullOrEmpty(usernameValue) || string.IsNullOrEmpty(passwordValue) || string.IsNullOrEmpty(passwordConfirmValue))
            {
                MessageBox.Show("Veuillez remplir tous les champs.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Vérifier si les mots de passe correspondent
            if (passwordValue != passwordConfirmValue)
            {
                MessageBox.Show("Les mots de passe ne correspondent pas.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Chaîne de connexion à la base de données
            string connectionString = "Server=localhost;Database=aviondb;Uid=root;Pwd=;";

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    // Ouvrir la connexion
                    connection.Open();

                    // Requête SQL pour insérer les données
                    string query = "INSERT INTO utilisateur (username, password) VALUES (@username, @password)";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        // Ajouter les paramètres pour éviter les injections SQL
                        command.Parameters.AddWithValue("@username", usernameValue);
                        command.Parameters.AddWithValue("@password", passwordValue);

                        // Exécuter la commande
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Utilisateur ajouté avec succès !", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Stocker les valeurs ajoutées dans les propriétés
                            NewUsername = usernameValue;
                            NewPassword = passwordValue;

                            // Indiquer que l'ajout a réussi et fermer le formulaire
                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Échec de l'ajout de l'utilisateur.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur : {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}