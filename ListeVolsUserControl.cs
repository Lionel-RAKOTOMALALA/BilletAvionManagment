using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace AvionManagment
{
    public partial class ListeVolsUserControl : UserControl
    {
        private readonly string connectionString = "Server=localhost;Database=avionDb;Uid=root;Pwd=;";

        public ListeVolsUserControl()
        {
            InitializeComponent();
            this.AutoScroll = true;
        }

        public void LoadVols()
        {
            try
            {
                List<VolInfo> vols = new List<VolInfo>();

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"SELECT v.id_vol, v.date_depart, v.date_arrive, v.id_avion, 
                                  a.modele AS avion_modele, a.capacite AS avion_capacite, 
                                  v.id_aeroport_depart, ad.ville AS aeroport_depart_ville, 
                                  v.id_aeroport_arrive, aa.ville AS aeroport_arrive_ville 
                                  FROM vol v 
                                  JOIN avion a ON v.id_avion = a.id_avion 
                                  JOIN aeroport ad ON v.id_aeroport_depart = ad.id_aeroport 
                                  JOIN aeroport aa ON v.id_aeroport_arrive = aa.id_aeroport 
                                  ORDER BY v.date_depart DESC";

                    MySqlCommand command = new MySqlCommand(query, connection);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DateTime dateDepart = Convert.ToDateTime(reader["date_depart"]);
                            DateTime dateArrive = Convert.ToDateTime(reader["date_arrive"]);
                            TimeSpan duree = dateArrive - dateDepart;

                            vols.Add(new VolInfo
                            {
                                IdVol = Convert.ToInt32(reader["id_vol"]),
                                DateDepart = dateDepart,
                                DateArrivee = dateArrive,
                                Duree = duree,
                                AvionModele = reader["avion_modele"].ToString(),
                                AeroportDepart = reader["aeroport_depart_ville"].ToString(),
                                AeroportArrivee = reader["aeroport_arrive_ville"].ToString(),
                                Prix = 150 // Vous pouvez calculer ce prix dynamiquement
                            });
                        }
                    }
                }

                DisplayVols(vols);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des vols: {ex.Message}", "Erreur",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DisplayVols(List<VolInfo> vols)
        {
            this.Controls.Clear();

            int yPos = 10;
            foreach (var vol in vols)
            {
                var card = new Panel
                {
                    Width = 300,
                    Height = 200,
                    Location = new Point(10, yPos),
                    BorderStyle = BorderStyle.FixedSingle,
                    BackColor = Color.White
                };

                // Ajouter les contrôles pour afficher les informations du vol
                var lblDepart = new Label
                {
                    Text = $"Départ: {vol.DateDepart:dd/MM/yyyy HH:mm} - {vol.AeroportDepart}",
                    Location = new Point(10, 10),
                    AutoSize = true
                };

                var lblArrivee = new Label
                {
                    Text = $"Arrivée: {vol.DateArrivee:dd/MM/yyyy HH:mm} - {vol.AeroportArrivee}",
                    Location = new Point(10, 30),
                    AutoSize = true
                };

                var lblDuree = new Label
                {
                    Text = $"Durée: {(int)vol.Duree.TotalHours}h{vol.Duree.Minutes:00}",
                    Location = new Point(10, 50),
                    AutoSize = true
                };

                var lblAvion = new Label
                {
                    Text = $"Avion: {vol.AvionModele}",
                    Location = new Point(10, 70),
                    AutoSize = true
                };

                var lblPrix = new Label
                {
                    Text = $"Prix: {vol.Prix} €",
                    Location = new Point(10, 90),
                    AutoSize = true,
                    Font = new Font(Font, FontStyle.Bold),
                    ForeColor = Color.Blue
                };

                var btnReserver = new Button
                {
                    Text = "Réserver",
                    Location = new Point(10, 120),
                    Size = new Size(100, 30),
                    BackColor = Color.FromArgb(0, 120, 215),
                    ForeColor = Color.White
                };
                btnReserver.Click += (s, e) => ReserverVol(vol.IdVol);

                // Ajouter les contrôles à la carte
                card.Controls.Add(lblDepart);
                card.Controls.Add(lblArrivee);
                card.Controls.Add(lblDuree);
                card.Controls.Add(lblAvion);
                card.Controls.Add(lblPrix);
                card.Controls.Add(btnReserver);

                // Ajouter la carte au UserControl
                this.Controls.Add(card);

                yPos += card.Height + 10;
            }
        }

        private void ReserverVol(int idVol)
        {
            MessageBox.Show($"Réservation du vol {idVol} confirmée!", "Réservation",
                          MessageBoxButtons.OK, MessageBoxIcon.Information);
            // Ici vous pouvez ouvrir un formulaire de réservation
        }
    }

    public class VolInfo
    {
        public int IdVol { get; set; }
        public DateTime DateDepart { get; set; }
        public DateTime DateArrivee { get; set; }
        public TimeSpan Duree { get; set; }
        public string AvionModele { get; set; }
        public string AeroportDepart { get; set; }
        public string AeroportArrivee { get; set; }
        public decimal Prix { get; set; }
    }
}