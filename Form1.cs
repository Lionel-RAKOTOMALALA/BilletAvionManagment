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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            panel1.BackColor = Color.FromArgb(3, 14, 28);
            loginBtn.BackColor = Color.FromArgb(3, 14, 28);
            registerBtn.BackColor = Color.FromArgb(151, 191, 244);

        }

        private void label1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void registerBtn_Click(object sender, EventArgs e)
        {
       

            // Créer une instance du formulaire RegistrationForm
            RegistrationForm registrationForm = new RegistrationForm();

            // Montrer le formulaire RegistrationForm
            registrationForm.Show();
            this.Hide();
            
        }

        private void loginBtn_Click(object sender, EventArgs e)
        {
            // Récupération des données saisies par l'utilisateur
            string usernameValue = username.Text; // Contenu de la TextBox username
            string passwordValue = password.Text; // Contenu de la TextBox password

            // Vérification des champs vides
            if (string.IsNullOrEmpty(usernameValue) || string.IsNullOrEmpty(passwordValue))
            {
                MessageBox.Show("Veuillez remplir tous les champs.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Chaîne de connexion à la base de données MySQL
            string connectionString = "Server=localhost;Database=aviondb;Uid=root;Pwd=;";

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    // Ouvrir la connexion à la base de données
                    connection.Open();

                    // Étape 1 : Vérifier si le nom d'utilisateur existe
                    string usernameQuery = "SELECT COUNT(*) FROM utilisateur WHERE username = @username";
                    using (MySqlCommand usernameCommand = new MySqlCommand(usernameQuery, connection))
                    {
                        usernameCommand.Parameters.AddWithValue("@username", usernameValue);

                        int usernameExists = Convert.ToInt32(usernameCommand.ExecuteScalar());

                        if (usernameExists == 0)
                        {
                            // Nom d'utilisateur incorrect
                            MessageBox.Show("Nom d'utilisateur incorrect.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }

                    // Étape 2 : Vérifier le mot de passe pour ce nom d'utilisateur
                    string passwordQuery = "SELECT COUNT(*) FROM utilisateur WHERE username = @username AND password = @password";
                    using (MySqlCommand passwordCommand = new MySqlCommand(passwordQuery, connection))
                    {
                        passwordCommand.Parameters.AddWithValue("@username", usernameValue);
                        passwordCommand.Parameters.AddWithValue("@password", passwordValue);

                        int passwordMatches = Convert.ToInt32(passwordCommand.ExecuteScalar());

                        if (passwordMatches == 0)
                        {
                            // Mot de passe incorrect
                            MessageBox.Show("Mot de passe incorrect.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }

                    // Étape 3 : Connexion réussie
                    MessageBox.Show("Connexion réussie !", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                   
                    // Afficher Form1 et fermer RegistrationForm
                    AdminMainForm form = new AdminMainForm();
                    form.Show();
                    this.Hide();
          
                }
            }
            catch (Exception ex)
            {
                // Gestion des erreurs
                MessageBox.Show($"Erreur : {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void showPassword_CheckedChanged(object sender, EventArgs e)
        {
            password.PasswordChar = showPassword.Checked ? '\0' : '*'; // Afficher ou masquer le mot de passe
        }
    }
}
