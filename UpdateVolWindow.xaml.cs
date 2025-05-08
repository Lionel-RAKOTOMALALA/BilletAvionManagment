using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using MySql.Data.MySqlClient;
using System.Windows.Controls;

namespace AvionManagment
{
    /// <summary>
    /// Logique d'interaction pour UpdateVolWindow.xaml
    /// </summary>
    public partial class UpdateVolWindow : Window
    {
        // Propriétés pour stocker les données du vol
        public int VolId { get; set; }
        public DateTime DateDepart { get; set; }
        public DateTime DateArrive { get; set; }
        public int AvionId { get; set; }
        public int AeroportDepartId { get; set; }
        public int AeroportArriveId { get; set; }

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

        public UpdateVolWindow()
        {
            InitializeComponent();

            // S'abonner à l'événement Loaded pour préremplir les champs
            this.Loaded += UpdateVolWindow_Loaded;
        }

        private void UpdateVolWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Charger les données des listes déroulantes
            LoadAvions();
            LoadAeroports();

            // Préremplir les champs avec les données du vol
            txtID.Text = VolId.ToString();

            // Définir les dates
            dpDateTimeDepart.SelectedDate = DateDepart.Date;
            dpDateTimeArrive.SelectedDate = DateArrive.Date;

            // Sélectionner l'avion
            SelectItemInComboBox(cmbAvion, AvionId);

            // Sélectionner les aéroports
            SelectItemInComboBox(cmbAeroportDepart, AeroportDepartId);
            SelectItemInComboBox(cmbAeroportArrive, AeroportArriveId);
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

        private void SelectItemInComboBox(ComboBox comboBox, int id)
        {
            if (comboBox.ItemsSource is IEnumerable<dynamic> items)
            {
                foreach (var item in items)
                {
                    if (item.Id == id)
                    {
                        comboBox.SelectedItem = item;
                        break;
                    }
                }
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

                // Définir les heures pour les dates (conserver les heures existantes ou utiliser des valeurs par défaut)
                DateTime dateTimeDepart = new DateTime(
                    dateDepart.Value.Year,
                    dateDepart.Value.Month,
                    dateDepart.Value.Day,
                    DateDepart.Hour,
                    DateDepart.Minute,
                    0);

                DateTime dateTimeArrive = new DateTime(
                    dateArrive.Value.Year,
                    dateArrive.Value.Month,
                    dateArrive.Value.Day,
                    DateArrive.Hour,
                    DateArrive.Minute,
                    0);

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

                    // Requête SQL pour mettre à jour un vol
                    string query = "UPDATE vol SET date_depart = @dateDepart, date_arrive = @dateArrive, " +
                                  "id_avion = @idAvion, id_aeroport_depart = @idAeroportDepart, id_aeroport_arrive = @idAeroportArrive " +
                                  "WHERE id_vol = @idVol";

                    MySqlCommand command = new MySqlCommand(query, connection);

                    // Ajouter les paramètres à la commande
                    command.Parameters.AddWithValue("@dateDepart", dateTimeDepart);
                    command.Parameters.AddWithValue("@dateArrive", dateTimeArrive);
                    command.Parameters.AddWithValue("@idAvion", avion.Id);
                    command.Parameters.AddWithValue("@idAeroportDepart", aeroportDepart.Id);
                    command.Parameters.AddWithValue("@idAeroportArrive", aeroportArrive.Id);
                    command.Parameters.AddWithValue("@idVol", VolId);

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Vol mis à jour avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                        this.DialogResult = true;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Aucune mise à jour effectuée.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Erreur MySQL lors de la mise à jour du vol: {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur inattendue: {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
