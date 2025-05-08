using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using MySql.Data.MySqlClient;
using System.Windows.Controls;

namespace AvionManagment
{
    /// <summary>
    /// Logique d'interaction pour AjoutVolWindow.xaml
    /// </summary>
    public partial class AjoutVolWindow : Window
    {
        // Chaîne de connexion à la base de données
        private readonly string connectionString = "Server=localhost;Database=avionDb;Uid=root;Pwd=;";

        // Classes pour stocker les données des listes déroulantes
        public class AvionItem
        {
            public int Id { get; set; }
            public string Display { get; set; }
            public override string ToString() => Display;
        }

        public class AeroportItem
        {
            public int Id { get; set; }
            public string Display { get; set; }
            public override string ToString() => Display;
        }

        



        public AjoutVolWindow()
        {
            InitializeComponent();

            // Charger les données des listes déroulantes
            LoadAvions();
            LoadAeroports();

            // Définir les dates par défaut
            dpDateTimeDepart.SelectedDate = DateTime.Today;
            dpDateTimeArrive.SelectedDate = DateTime.Today;
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

        private void LoadAvions()
        {
            try
            {
                List<AvionItem> avions = new List<AvionItem>();

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    // Récupérer uniquement les avions avec capacité > 0
                    string query = "SELECT id_avion, modele FROM avion WHERE capacite > 0";
                    MySqlCommand command = new MySqlCommand(query, connection);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = Convert.ToInt32(reader["id_avion"]);
                            string modele = reader["modele"].ToString();

                            avions.Add(new AvionItem
                            {
                                Id = id,
                                Display = $"{id} - {modele}"
                            });
                        }
                    }
                }

                cmbAvion.ItemsSource = avions;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des avions: {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadAeroports()
        {
            try
            {
                List<AeroportItem> aeroports = new List<AeroportItem>();

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT id_aeroport, ville FROM aeroport";
                    MySqlCommand command = new MySqlCommand(query, connection);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = Convert.ToInt32(reader["id_aeroport"]);
                            string ville = reader["ville"].ToString();

                            aeroports.Add(new AeroportItem
                            {
                                Id = id,
                                Display = $"{id} - {ville}"
                            });
                        }
                    }
                }

                // Assigner la même source aux deux ComboBox
                cmbAeroportDepart.ItemsSource = aeroports;
                cmbAeroportArrive.ItemsSource = new List<AeroportItem>(aeroports); // Copie pour éviter les problèmes de référence
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des aéroports: {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }




        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Récupérer les valeurs des champs
                DateTime? dateDepart = dpDateTimeDepart.SelectedDate;
                DateTime? dateArrive = dpDateTimeArrive.SelectedDate;
                AvionItem avion = cmbAvion.SelectedItem as AvionItem;
                AeroportItem aeroportDepart = cmbAeroportDepart.SelectedItem as AeroportItem;
                AeroportItem aeroportArrive = cmbAeroportArrive.SelectedItem as AeroportItem;

                // Vérifier que tous les champs obligatoires sont remplis
                if (!dateDepart.HasValue || !dateArrive.HasValue ||
                    avion == null || aeroportDepart == null || aeroportArrive == null)
                {
                    MessageBox.Show("Tous les champs obligatoires doivent être remplis.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Définir les heures pour les dates (par défaut à midi si seulement la date est sélectionnée)
                DateTime dateTimeDepart = dateDepart.Value.Date.AddHours(12);
                DateTime dateTimeArrive = dateArrive.Value.Date.AddHours(14); // Par défaut 2 heures plus tard

                // Vérifier que les aéroports de départ et d'arrivée sont différents
                if (aeroportDepart.Id == aeroportArrive.Id)
                {
                    MessageBox.Show("Les aéroports de départ et d'arrivée doivent être différents.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Vérifier que la date d'arrivée est après la date de départ
                if (dateTimeArrive <= dateTimeDepart)
                {
                    MessageBox.Show("La date et l'heure d'arrivée doivent être après la date et l'heure de départ.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    // Requête SQL de base pour insérer un vol
                    string query = "INSERT INTO vol (date_depart, date_arrive, id_avion, id_aeroport_depart, id_aeroport_arrive) " +
                                  "VALUES (@dateDepart, @dateArrive, @idAvion, @idAeroportDepart, @idAeroportArrive)";

                    MySqlCommand command = new MySqlCommand(query, connection);

                    // Ajouter les paramètres à la commande
                    command.Parameters.AddWithValue("@dateDepart", dateTimeDepart);
                    command.Parameters.AddWithValue("@dateArrive", dateTimeArrive);
                    command.Parameters.AddWithValue("@idAvion", avion.Id);
                    command.Parameters.AddWithValue("@idAeroportDepart", aeroportDepart.Id);
                    command.Parameters.AddWithValue("@idAeroportArrive", aeroportArrive.Id);

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Vol ajouté avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                        this.DialogResult = true;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Aucun vol n'a été ajouté.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Erreur MySQL lors de l'ajout du vol: {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur inattendue: {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Méthode pour ajouter la fonctionnalité de sélection d'heure aux DatePicker
        private DateTime CombineDateAndTime(DateTime date, TimeSpan time)
        {
            return new DateTime(date.Year, date.Month, date.Day, time.Hours, time.Minutes, 0);
        }
    }
}