using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient; // Référence à MySQL

namespace AvionManagment
{
    public partial class Tableau_reservation : UserControl
    {
        // Liste pour stocker les données des réservations
        private List<ReservationData> reservationsList = new List<ReservationData>();

        // Classe pour représenter les données d'une réservation
        public class ReservationData
        {
            public int Id { get; set; }
            public string Statut { get; set; }
            public DateTime DateReservation { get; set; }
            public string Username { get; set; }
            public DateTime DateDepart { get; set; }
            public DateTime DateArrivee { get; set; }
            public string AeroportDepart { get; set; }
            public string AeroportArrivee { get; set; }
        }

        // Variables pour la pagination
        private int currentPage = 1;
        private int itemsPerPage = 5;
        private int totalItems = 0;
        private int totalPages = 0;

        // Chaîne de connexion à la base de données
        private string connectionString = "Server=localhost;Database=avionDb;Uid=root;Pwd=;";

        // Déclaration des contrôles UI
        private Panel panel1;
        private Label lblTitle;
        private Button btnDelete;
        private DataGridView dataGridView;
        private Panel panelPagination;
        private Label lblPagination;
        private Button btnPrevious;
        private Button btnPage1;
        private Button btnPage2;
        private Button btnPage3;
        private Button btnPage4;
        private Button btnPage5;
        private Button btnNext;

        // Couleurs pour le thème
        private Color primaryColor = Color.FromArgb(0, 123, 255);
        private Color secondaryColor = Color.FromArgb(108, 117, 125);
        private Color successColor = Color.FromArgb(40, 167, 69);
        private Color dangerColor = Color.FromArgb(220, 53, 69);
        private Color warningColor = Color.FromArgb(255, 193, 7);
        private Color lightGrayColor = Color.FromArgb(240, 240, 240);
        private Color darkBlueColor = Color.FromArgb(13, 17, 23);

        public Tableau_reservation()
        {
            InitializeComponent();

            // Configurer l'apparence du contrôle utilisateur
            this.BackColor = Color.White;
            this.Dock = DockStyle.Fill;

            // Initialiser les contrôles
            InitializeControls();

            // Initialiser le tableau
            SetupDataGridView();

            // Charger les données depuis la base de données
            LoadDataFromDatabase();
        }

        // Remarque: La méthode InitializeComponent est générée par le designer
        // Ne pas créer une deuxième méthode avec le même nom
        // partial void InitializeComponent()
        // {
        //     this.SuspendLayout();
        //     // 
        //     // Tableau_reservation
        //     // 
        //     this.Name = "Tableau_reservation";
        //     this.Size = new System.Drawing.Size(800, 600);
        //     this.ResumeLayout(false);
        // }

        private void InitializeControls()
        {
            // Initialiser le panel principal
            panel1 = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(224, 224, 224) // Gris clair
            };
            this.Controls.Add(panel1);

            // Initialiser le titre
            lblTitle = new Label
            {
                Text = "Gestion des réservations",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(20, 15)
            };
            panel1.Controls.Add(lblTitle);

            // Initialiser le bouton Supprimer
            btnDelete = new Button
            {
                Text = "Supprimer",
                Size = new Size(100, 30),
                Location = new Point(panel1.Width - 120, 15),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = dangerColor,
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                Cursor = Cursors.Hand
            };
            btnDelete.FlatAppearance.BorderColor = dangerColor;
            btnDelete.Click += btnDelete_Click;
            panel1.Controls.Add(btnDelete);

            // Initialiser le DataGridView
            dataGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.None,
                BackgroundColor = Color.White,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                MultiSelect = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                EnableHeadersVisualStyles = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            this.Controls.Add(dataGridView);

            // Initialiser le panel de pagination
            panelPagination = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                BackColor = Color.FromArgb(224, 224, 224) // Gris clair
            };
            this.Controls.Add(panelPagination);

            // Initialiser le label de pagination
            lblPagination = new Label
            {
                AutoSize = true,
                Location = new Point(20, 15),
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.Black
            };
            panelPagination.Controls.Add(lblPagination);

            // Initialiser les boutons de pagination
            InitializePaginationButtons();

            // Ajuster l'ordre des contrôles
            this.Controls.SetChildIndex(panelPagination, 0);
            this.Controls.SetChildIndex(dataGridView, 1);
            this.Controls.SetChildIndex(panel1, 2);
        }

        private void InitializePaginationButtons()
        {
            // Bouton Précédent
            btnPrevious = CreatePaginationButton("Précédent", panelPagination.Width - 500, 10, 80);
            btnPrevious.Click += btnPrevious_Click;
            panelPagination.Controls.Add(btnPrevious);

            // Boutons de page
            btnPage1 = CreatePaginationButton("1", panelPagination.Width - 410, 10, 30);
            btnPage1.Click += btnPage_Click;
            panelPagination.Controls.Add(btnPage1);

            btnPage2 = CreatePaginationButton("2", panelPagination.Width - 370, 10, 30);
            btnPage2.Click += btnPage_Click;
            panelPagination.Controls.Add(btnPage2);

            btnPage3 = CreatePaginationButton("3", panelPagination.Width - 330, 10, 30);
            btnPage3.Click += btnPage_Click;
            panelPagination.Controls.Add(btnPage3);

            btnPage4 = CreatePaginationButton("4", panelPagination.Width - 290, 10, 30);
            btnPage4.Click += btnPage_Click;
            panelPagination.Controls.Add(btnPage4);

            btnPage5 = CreatePaginationButton("5", panelPagination.Width - 250, 10, 30);
            btnPage5.Click += btnPage_Click;
            panelPagination.Controls.Add(btnPage5);

            // Bouton Suivant
            btnNext = CreatePaginationButton("Suivant", panelPagination.Width - 210, 10, 80);
            btnNext.Click += btnNext_Click;
            panelPagination.Controls.Add(btnNext);
        }

        private Button CreatePaginationButton(string text, int x, int y, int width)
        {
            Button btn = new Button
            {
                Text = text,
                Size = new Size(width, 30),
                Location = new Point(x, y),
                FlatStyle = FlatStyle.Flat,
                BackColor = lightGrayColor,
                ForeColor = Color.Black,
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderColor = Color.LightGray;
            return btn;
        }

        private void SetupDataGridView()
        {
            // Configurer les colonnes du DataGridView
            dataGridView.Columns.Clear();

            // Colonne de case à cocher
            DataGridViewCheckBoxColumn checkColumn = new DataGridViewCheckBoxColumn
            {
                HeaderText = "",
                Width = 40,
                Name = "checkColumn"
            };
            dataGridView.Columns.Add(checkColumn);

            // Colonne ID
            DataGridViewTextBoxColumn idColumn = new DataGridViewTextBoxColumn
            {
                HeaderText = "ID",
                Name = "Id",
                Width = 60,
                ReadOnly = true
            };
            dataGridView.Columns.Add(idColumn);

            // Colonne Statut
            DataGridViewTextBoxColumn statutColumn = new DataGridViewTextBoxColumn
            {
                HeaderText = "Statut",
                Name = "Statut",
                Width = 80,
                ReadOnly = true
            };
            dataGridView.Columns.Add(statutColumn);

            // Colonne Date de réservation
            DataGridViewTextBoxColumn dateReservationColumn = new DataGridViewTextBoxColumn
            {
                HeaderText = "Date de réservation",
                Name = "DateReservation",
                Width = 120,
                ReadOnly = true
            };
            dataGridView.Columns.Add(dateReservationColumn);

            // Colonne Utilisateur
            DataGridViewTextBoxColumn usernameColumn = new DataGridViewTextBoxColumn
            {
                HeaderText = "Utilisateur",
                Name = "Username",
                Width = 100,
                ReadOnly = true
            };
            dataGridView.Columns.Add(usernameColumn);

            // Colonne Date de départ
            DataGridViewTextBoxColumn dateDepartColumn = new DataGridViewTextBoxColumn
            {
                HeaderText = "Date de départ",
                Name = "DateDepart",
                Width = 120,
                ReadOnly = true
            };
            dataGridView.Columns.Add(dateDepartColumn);

            // Colonne Date d'arrivée
            DataGridViewTextBoxColumn dateArriveeColumn = new DataGridViewTextBoxColumn
            {
                HeaderText = "Date d'arrivée",
                Name = "DateArrivee",
                Width = 120,
                ReadOnly = true
            };
            dataGridView.Columns.Add(dateArriveeColumn);

            // Colonne Aéroport de départ
            DataGridViewTextBoxColumn aeroportDepartColumn = new DataGridViewTextBoxColumn
            {
                HeaderText = "Aéroport de départ",
                Name = "AeroportDepart",
                Width = 120,
                ReadOnly = true
            };
            dataGridView.Columns.Add(aeroportDepartColumn);

            // Colonne Aéroport d'arrivée
            DataGridViewTextBoxColumn aeroportArriveeColumn = new DataGridViewTextBoxColumn
            {
                HeaderText = "Aéroport d'arrivée",
                Name = "AeroportArrivee",
                Width = 120,
                ReadOnly = true
            };
            dataGridView.Columns.Add(aeroportArriveeColumn);

            // Colonne Actions
            DataGridViewTextBoxColumn actionsColumn = new DataGridViewTextBoxColumn
            {
                HeaderText = "Actions",
                Name = "Actions",
                Width = 60,
                ReadOnly = true
            };
            dataGridView.Columns.Add(actionsColumn);

            // Configurer l'apparence du DataGridView
            dataGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.White;
            dataGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dataGridView.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dataGridView.ColumnHeadersDefaultCellStyle.Padding = new Padding(10, 0, 0, 0);
            dataGridView.ColumnHeadersHeight = 40;

            dataGridView.DefaultCellStyle.BackColor = Color.White;
            dataGridView.DefaultCellStyle.ForeColor = Color.Black;
            dataGridView.DefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Regular);
            dataGridView.DefaultCellStyle.Padding = new Padding(10, 0, 0, 0);
            dataGridView.RowTemplate.Height = 40;

            // Alternance de couleurs pour les lignes
            dataGridView.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(249, 249, 249);

            // Sélection de ligne
            dataGridView.DefaultCellStyle.SelectionBackColor = Color.FromArgb(230, 244, 255);
            dataGridView.DefaultCellStyle.SelectionForeColor = Color.Black;

            // Gérer l'événement CellPainting pour dessiner les icônes d'action
            dataGridView.CellPainting += DataGridView_CellPainting;

            // Ajouter l'événement pour gérer les clics sur les cellules
            dataGridView.CellClick += DataGridView_CellClick;

            // Ajuster la largeur des colonnes
            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dataGridView.Columns[0].Width = 40;  // Checkbox
            dataGridView.Columns[1].Width = 60;  // ID
            dataGridView.Columns[2].Width = 80;  // Statut
            dataGridView.Columns[3].Width = 120; // Date de réservation
            dataGridView.Columns[4].Width = 100; // Utilisateur
            dataGridView.Columns[5].Width = 120; // Date de départ
            dataGridView.Columns[6].Width = 120; // Date d'arrivée
            dataGridView.Columns[7].Width = 120; // Aéroport de départ
            dataGridView.Columns[8].Width = 120; // Aéroport d'arrivée
            dataGridView.Columns[9].Width = 60;  // Actions
        }

        private void LoadDataFromDatabase()
        {
            try
            {
                // Effacer la liste existante
                reservationsList.Clear();

                // Créer une connexion à la base de données
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    // Ouvrir la connexion
                    connection.Open();

                    // Créer une commande SQL pour récupérer les réservations
                    string query = @"
                        SELECT r.id_reservation, r.statut, r.date_reservation, u.username, 
                               v.date_depart, v.date_arrive, ae.ville as aeropot_depart, 
                               aero.ville as aeroport_arrive 
                        FROM reservation r 
                        INNER JOIN vol v ON r.id_vol=v.id_vol 
                        INNER JOIN utilisateur u ON r.id_user = u.id_user 
                        INNER JOIN aeroport ae ON v.id_aeroport_depart = ae.id_aeroport 
                        INNER JOIN aeroport aero ON aero.id_aeroport = v.id_aeroport_arrive";

                    MySqlCommand command = new MySqlCommand(query, connection);

                    // Exécuter la commande et lire les résultats
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Créer un nouvel objet ReservationData pour chaque ligne
                            ReservationData reservation = new ReservationData
                            {
                                Id = Convert.ToInt32(reader["id_reservation"]),
                                Statut = reader["statut"].ToString(),
                                DateReservation = Convert.ToDateTime(reader["date_reservation"]),
                                Username = reader["username"].ToString(),
                                DateDepart = Convert.ToDateTime(reader["date_depart"]),
                                DateArrivee = Convert.ToDateTime(reader["date_arrive"]),
                                AeroportDepart = reader["aeropot_depart"].ToString(),
                                AeroportArrivee = reader["aeroport_arrive"].ToString()
                            };

                            // Ajouter la réservation à la liste
                            reservationsList.Add(reservation);
                        }
                    }
                }

                // Mettre à jour le total des éléments
                totalItems = reservationsList.Count;
                totalPages = (int)Math.Ceiling((double)totalItems / itemsPerPage);

                // Réinitialiser la page courante si nécessaire
                if (currentPage > totalPages && totalPages > 0)
                {
                    currentPage = 1;
                }

                // Charger les données dans le DataGridView
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des données: {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DataGridView_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            // Dessiner l'icône de suppression dans la colonne Actions
            if (e.ColumnIndex == 9 && e.RowIndex >= 0)
            {
                e.Paint(e.CellBounds, DataGridViewPaintParts.All);

                // Dessiner l'icône de suppression (poubelle)
                var deleteRect = new Rectangle(
                    e.CellBounds.Left + e.CellBounds.Width / 2 - 8,
                    e.CellBounds.Top + e.CellBounds.Height / 2 - 8,
                    16, 16);

                using (Pen pen = new Pen(dangerColor, 2))
                {
                    // Dessiner une poubelle
                    e.Graphics.DrawRectangle(pen, deleteRect.Left, deleteRect.Top + 3, deleteRect.Width, deleteRect.Height - 3);
                    e.Graphics.DrawLine(pen, deleteRect.Left + 3, deleteRect.Top, deleteRect.Right - 3, deleteRect.Top);
                    e.Graphics.DrawLine(pen, deleteRect.Left + deleteRect.Width / 2, deleteRect.Top, deleteRect.Left + deleteRect.Width / 2, deleteRect.Bottom);
                }

                e.Handled = true;
            }

            // Personnaliser l'apparence des en-têtes de colonne
            if (e.RowIndex == -1)
            {
                e.CellStyle.BackColor = Color.White;
                e.CellStyle.ForeColor = Color.Black;
                e.CellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            }
        }

        private void LoadData()
        {
            // Effacer les données existantes
            dataGridView.Rows.Clear();

            // Calculer les indices de début et de fin pour la pagination
            int startIndex = (currentPage - 1) * itemsPerPage;
            int endIndex = Math.Min(startIndex + itemsPerPage, totalItems);

            // Ajouter les données à la grille
            for (int i = startIndex; i < endIndex && i < reservationsList.Count; i++)
            {
                ReservationData reservation = reservationsList[i];

                // Ajouter une nouvelle ligne
                int rowIndex = dataGridView.Rows.Add();
                DataGridViewRow row = dataGridView.Rows[rowIndex];

                // Remplir les cellules
                row.Cells[0].Value = false; // Case à cocher
                row.Cells[1].Value = reservation.Id;
                row.Cells[2].Value = reservation.Statut;
                row.Cells[3].Value = reservation.DateReservation.ToString("dd/MM/yyyy HH:mm");
                row.Cells[4].Value = reservation.Username;
                row.Cells[5].Value = reservation.DateDepart.ToString("dd/MM/yyyy HH:mm");
                row.Cells[6].Value = reservation.DateArrivee.ToString("dd/MM/yyyy HH:mm");
                row.Cells[7].Value = reservation.AeroportDepart;
                row.Cells[8].Value = reservation.AeroportArrivee;
                row.Cells[9].Value = ""; // La colonne Actions est gérée par l'événement CellPainting
            }

            // Mettre à jour le texte de pagination
            UpdatePaginationText();

            // Mettre à jour l'apparence des boutons de pagination
            UpdatePaginationButtons();
        }

        private void UpdatePaginationText()
        {
            int startItem = totalItems == 0 ? 0 : (currentPage - 1) * itemsPerPage + 1;
            int endItem = Math.Min(currentPage * itemsPerPage, totalItems);

            lblPagination.Text = $"Affichage de {startItem} à {endItem} sur {totalItems}";
        }

        private void UpdatePaginationButtons()
        {
            // Mettre à jour l'apparence des boutons de pagination
            UpdatePaginationButtonStyle(btnPage1, currentPage == 1);
            UpdatePaginationButtonStyle(btnPage2, currentPage == 2);
            UpdatePaginationButtonStyle(btnPage3, currentPage == 3);
            UpdatePaginationButtonStyle(btnPage4, currentPage == 4);
            UpdatePaginationButtonStyle(btnPage5, currentPage == 5);

            // Activer/désactiver les boutons Précédent/Suivant
            btnPrevious.Enabled = (currentPage > 1);
            btnNext.Enabled = (currentPage < totalPages);

            // Ajuster l'apparence des boutons désactivés
            btnPrevious.BackColor = btnPrevious.Enabled ? lightGrayColor : Color.FromArgb(220, 220, 220);
            btnNext.BackColor = btnNext.Enabled ? lightGrayColor : Color.FromArgb(220, 220, 220);
        }

        private void UpdatePaginationButtonStyle(Button btn, bool isActive)
        {
            if (isActive)
            {
                btn.BackColor = primaryColor;
                btn.ForeColor = Color.White;
                btn.FlatAppearance.BorderColor = primaryColor;
            }
            else
            {
                btn.BackColor = lightGrayColor;
                btn.ForeColor = Color.Black;
                btn.FlatAppearance.BorderColor = Color.LightGray;
            }
        }

        private void btnPage_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int page = int.Parse(btn.Text);

            if (page != currentPage && page <= totalPages)
            {
                currentPage = page;
                LoadData();
            }
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                LoadData();
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (currentPage < totalPages)
            {
                currentPage++;
                LoadData();
            }
        }

        private void DataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Gérer les clics sur les cellules de la colonne Actions
            if (e.RowIndex >= 0 && e.ColumnIndex == 9) // Colonne Actions
            {
                int reservationId = Convert.ToInt32(dataGridView.Rows[e.RowIndex].Cells[1].Value);
                DeleteReservation(reservationId);
            }
        }

        private void DeleteReservation(int reservationId)
        {
            // Demander confirmation
            DialogResult result = MessageBox.Show($"Voulez-vous vraiment supprimer cette réservation?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // Supprimer la réservation de la base de données
                if (DeleteReservationFromDatabase(reservationId))
                {
                    // Recharger les données
                    LoadDataFromDatabase();
                }
            }
        }

        private bool DeleteReservationFromDatabase(int reservationId)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "DELETE FROM reservation WHERE id_reservation = @id";
                    MySqlCommand command = new MySqlCommand(query, connection);

                    command.Parameters.AddWithValue("@id", reservationId);

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Réservation supprimée avec succès.", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return true;
                    }
                    else
                    {
                        MessageBox.Show("Aucune réservation n'a été supprimée.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la suppression de la réservation: {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            // Supprimer les réservations sélectionnées
            List<int> selectedIds = new List<int>();

            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (row.Cells[0].Value != null && (bool)row.Cells[0].Value)
                {
                    int reservationId = Convert.ToInt32(row.Cells[1].Value);
                    selectedIds.Add(reservationId);
                }
            }

            if (selectedIds.Count > 0)
            {
                DialogResult result = MessageBox.Show($"Voulez-vous vraiment supprimer {selectedIds.Count} réservation(s)?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    bool anyDeleted = false;

                    foreach (int id in selectedIds)
                    {
                        if (DeleteReservationFromDatabase(id))
                        {
                            anyDeleted = true;
                        }
                    }

                    if (anyDeleted)
                    {
                        // Recharger les données
                        LoadDataFromDatabase();
                    }
                }
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner au moins une réservation à supprimer.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // Gérer le redimensionnement du contrôle
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            // Repositionner les boutons dans le panel supérieur
            if (panel1 != null && btnDelete != null)
            {
                btnDelete.Location = new Point(this.Width - 120, 15);
            }

            // Repositionner les boutons de pagination
            if (panelPagination != null && btnPrevious != null)
            {
                int centerX = this.Width / 2;
                btnPrevious.Location = new Point(centerX - 200, 10);

                if (btnPage1 != null) btnPage1.Location = new Point(centerX - 110, 10);
                if (btnPage2 != null) btnPage2.Location = new Point(centerX - 70, 10);
                if (btnPage3 != null) btnPage3.Location = new Point(centerX - 30, 10);
                if (btnPage4 != null) btnPage4.Location = new Point(centerX + 10, 10);
                if (btnPage5 != null) btnPage5.Location = new Point(centerX + 50, 10);
                if (btnNext != null) btnNext.Location = new Point(centerX + 90, 10);
            }
        }
    }
}