using System;
using System.Windows;
using System.Windows.Input;
using MySql.Data.MySqlClient;

namespace AvionManagment
{
    /// <summary>
    /// Logique d'interaction pour AjoutAvionWindow.xaml
    /// </summary>
    public partial class AjoutAvionWindow : Window
    {
        // Chaîne de connexion à la base de données
        private readonly string connectionString = "Server=localhost;Database=avionDb;Uid=root;Pwd=;";

        public AjoutAvionWindow()
        {
            InitializeComponent();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Récupérer les valeurs des champs
            string modele = txtModele.Text.Trim();
            string capaciteText = txtCapacite.Text.Trim();

            // Vérifier que les champs ne sont pas vides
            if (string.IsNullOrWhiteSpace(modele) || string.IsNullOrWhiteSpace(capaciteText))
            {
                MessageBox.Show("Tous les champs sont obligatoires.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Vérifier que la capacité est un nombre valide
            if (!int.TryParse(capaciteText, out int capacite) || capacite <= 0)
            {
                MessageBox.Show("La capacité doit être un nombre entier positif.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "INSERT INTO avion (modele, capacite) VALUES (@modele, @capacite)";
                    MySqlCommand command = new MySqlCommand(query, connection);

                    // Ajouter les paramètres à la commande
                    command.Parameters.AddWithValue("@modele", modele);
                    command.Parameters.AddWithValue("@capacite", capacite);

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Avion ajouté avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                        this.DialogResult = true;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Aucun avion n'a été ajouté.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'ajout de l'avion: {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}