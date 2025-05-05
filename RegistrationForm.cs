using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace AvionManagment
{
    public partial class RegistrationForm : Form
    {
        public RegistrationForm()
        {
            InitializeComponent();
            panel1.BackColor = Color.FromArgb(3, 14, 28);
            registerBtn.BackColor = Color.FromArgb(3, 14, 28);
            loginBtn.BackColor = Color.FromArgb(151, 191, 244);
        }

        private void label1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void registerBtn_Click(object sender, EventArgs e)
        {
            // Récupération des données saisies par l'utilisateur
            string usernameValue = username.Text; // Récupération du texte de la TextBox username
            string passwordValue = password.Text; // Récupération du texte de la TextBox password
            string passwordConfirmValue = passwordConfirm.Text; // Récupération du texte de la TextBox de confirmation du mot de passe

            // Vérification des champs vides
            if (string.IsNullOrEmpty(usernameValue) || string.IsNullOrEmpty(passwordValue) || string.IsNullOrEmpty(passwordConfirmValue))
            {
                MessageBox.Show("Veuillez remplir tous les champs.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Vérification de la correspondance des mots de passe
            if (passwordValue != passwordConfirmValue)
            {
                MessageBox.Show("Les mots de passe ne correspondent pas.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Chaîne de connexion à la base de données MySQL
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
                        // Paramètres pour éviter les injections SQL
                        command.Parameters.AddWithValue("@username", usernameValue);
                        command.Parameters.AddWithValue("@password", passwordValue);

                        // Exécuter la commande
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Inscription réussie !", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            // Réinitialiser les champs après une inscription réussie
                            username.Clear();
                            password.Clear();
                            passwordConfirm.Clear();
                        }
                        else
                        {
                            MessageBox.Show("Échec de l'inscription.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur : {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void loginBtn_Click(object sender, EventArgs e)
        {
            // Créer une instance du formulaire Form1
            Form1 form1 = new Form1();

            // Montrer le formulaire Form1
            form1.Show();

            // Fermer le formulaire RegistrationForm
            this.Hide();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            password.PasswordChar = showPassword.Checked ? '\0' : '*'; // Afficher ou masquer le mot de passe
            passwordConfirm.PasswordChar = showPassword.Checked ? '\0' : '*';

        }
    }
}
