using System;
using System.Windows;
using System.Windows.Input;
using MySql.Data.MySqlClient;

namespace AvionManagment
{
    /// <summary>
    /// Logique d'interaction pour UpdateAvionWindow.xaml
    /// </summary>
    public partial class UpdateAvionWindow : Window
    {
        // Propriétés pour stocker les données de l'avion
        public int AvionId { get; set; }
        public string Modele { get; set; }
        public int Capacite { get; set; }

        // Chaîne de connexion à la base de données
        private readonly string connectionString = "Server=localhost;Database=avionDb;Uid=root;Pwd=;";

        public UpdateAvionWindow()
        {
            InitializeComponent();

            // S'abonner à l'événement Loaded pour préremplir les champs
            this.Loaded += UpdateAvionWindow_Loaded;
        }

        private void UpdateAvionWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Préremplir les champs avec les données de l'avion
            txtID.Text = AvionId.ToString();
            txtModele.Text = Modele;
            txtCapacite.Text = Capacite.ToString();
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
            // Fermer la fenêtre sans appliquer de modifications
            this.DialogResult = false;
            this.Close();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Récupérer les valeurs mises à jour
            string updatedModele = txtModele.Text.Trim();
            string updatedCapaciteText = txtCapacite.Text.Trim();

            // Vérifier que les champs ne sont pas vides
            if (string.IsNullOrWhiteSpace(updatedModele) || string.IsNullOrWhiteSpace(updatedCapaciteText))
            {
                MessageBox.Show("Tous les champs sont obligatoires.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Vérifier que la capacité est un nombre valide
            if (!int.TryParse(updatedCapaciteText, out int updatedCapacite) || updatedCapacite <= 0)
            {
                MessageBox.Show("La capacité doit être un nombre entier positif.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Vérifier si des modifications ont été faites
            if (updatedModele == Modele && updatedCapacite == Capacite)
            {
                MessageBox.Show("Aucune modification détectée.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "UPDATE avion SET modele = @modele, capacite = @capacite WHERE id_avion = @id";
                    MySqlCommand command = new MySqlCommand(query, connection);

                    // Ajouter les paramètres à la commande
                    command.Parameters.AddWithValue("@modele", updatedModele);
                    command.Parameters.AddWithValue("@capacite", updatedCapacite);
                    command.Parameters.AddWithValue("@id", AvionId);

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Avion mis à jour avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Aucune mise à jour effectuée.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la mise à jour de l'avion: {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            // Fermer le formulaire et retourner un résultat OK
            this.DialogResult = true;
            this.Close();
        }
    }
}