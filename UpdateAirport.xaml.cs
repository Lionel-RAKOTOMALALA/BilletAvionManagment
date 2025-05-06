using System;
using System.Windows;
using System.Windows.Input;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace AvionManagment
{
    /// <summary>
    /// Logique d'interaction pour UpdateAirportWindow.xaml
    /// </summary>
    public partial class UpdateAirportWindow : Window
    {
        // Propriétés pour stocker les données de l'aéroport
        public int AirportId { get; set; }
        public string NomAeroport { get; set; }
        public string Ville { get; set; }
        public string Pays { get; set; }

        // Chaîne de connexion à la base de données
        private readonly string connectionString = "Server=localhost;Database=avionDb;Uid=root;Pwd=;";

        public UpdateAirportWindow()
        {
            InitializeComponent();

            // S'abonner à l'événement Loaded pour préremplir les champs
            this.Loaded += UpdateAirportWindow_Loaded;
        }

        private void UpdateAirportWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Préremplir les champs avec les données de l'aéroport
            txtID.Text = AirportId.ToString();
            txtNom.Text = NomAeroport;
            txtVille.Text = Ville;
            txtPays.Text = Pays;
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
            string updatedNom = txtNom.Text.Trim();
            string updatedVille = txtVille.Text.Trim();
            string updatedPays = txtPays.Text.Trim();

            // Vérifier si des modifications ont été faites
            if (updatedNom == NomAeroport && updatedVille == Ville && updatedPays == Pays)
            {
                MessageBox.Show("Aucune modification détectée.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Vérifier que les champs ne sont pas vides
            if (string.IsNullOrWhiteSpace(updatedNom) || string.IsNullOrWhiteSpace(updatedVille) || string.IsNullOrWhiteSpace(updatedPays))
            {
                MessageBox.Show("Tous les champs sont obligatoires.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
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
                        MessageBox.Show("Aéroport mis à jour avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Aucune mise à jour effectuée.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la mise à jour de l'aéroport: {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            // Fermer le formulaire et retourner un résultat OK
            this.DialogResult = true;
            this.Close();
        }
    }
}
