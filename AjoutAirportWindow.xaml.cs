using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using MySql.Data.MySqlClient;

namespace AvionManagment
{
    public partial class AjoutAirportWindow : Window
    {
        // Chaîne de connexion à la base de données
        private string connectionString = "Server=localhost;Database=avionDb;Uid=root;Pwd=;";

        public AjoutAirportWindow()
        {
            InitializeComponent();

            // Animation d'ouverture
            this.Opacity = 0;

            // Configurer la transformation initiale
            WindowScaleTransform.ScaleX = 0.8;
            WindowScaleTransform.ScaleY = 0.8;

            // Créer et configurer l'animation d'opacité
            DoubleAnimation fadeIn = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            // Créer et configurer l'animation d'échelle
            DoubleAnimation scaleX = new DoubleAnimation
            {
                From = 0.8,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(350),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            DoubleAnimation scaleY = new DoubleAnimation
            {
                From = 0.8,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(350),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            // Appliquer les animations
            this.BeginAnimation(UIElement.OpacityProperty, fadeIn);
            
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            CloseWithAnimation();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            CloseWithAnimation();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Vérifier que tous les champs sont remplis
            if (string.IsNullOrWhiteSpace(txtNom.Text) ||
                string.IsNullOrWhiteSpace(txtVille.Text) ||
                string.IsNullOrWhiteSpace(txtPays.Text))
            {
                MessageBox.Show("Veuillez remplir tous les champs.", "Champs manquants", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                        MessageBox.Show("Aéroport ajouté avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                        this.DialogResult = true;
                        CloseWithAnimation();
                    }
                    else
                    {
                        MessageBox.Show("Erreur lors de l'ajout de l'aéroport.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur: {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CloseWithAnimation()
        {
            // Animation de fermeture
            DoubleAnimation fadeOut = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(250),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            };

            DoubleAnimation scaleX = new DoubleAnimation
            {
                From = 1,
                To = 0.8,
                Duration = TimeSpan.FromMilliseconds(200),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            };

            DoubleAnimation scaleY = new DoubleAnimation
            {
                From = 1,
                To = 0.8,
                Duration = TimeSpan.FromMilliseconds(200),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            };

            // Fermer la fenêtre après l'animation
            fadeOut.Completed += (s, _) => this.Close();

            // Appliquer les animations
            this.BeginAnimation(UIElement.OpacityProperty, fadeOut);
            
        }
    }
}