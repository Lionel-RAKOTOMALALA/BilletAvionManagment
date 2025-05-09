using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.IO;
using System.Diagnostics;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace AvionManagment
{
    public partial class Tableau_billet : UserControl
    {
        // Liste pour stocker les données des billets
        private List<BilletData> billetsList = new List<BilletData>();

        // Classe pour représenter les données d'un billet
        public class BilletData
        {
            public int IdBillet { get; set; }
            public int IdReservation { get; set; }
            public string Username { get; set; }
            public string PaysDepart { get; set; }
            public string NomAeroportDepart { get; set; }
            public string VilleDepart { get; set; }
            public string VilleArrivee { get; set; }
            public DateTime DateReservation { get; set; }
            public DateTime DateDepart { get; set; }
            public DateTime DateArrivee { get; set; }
            public string ModeleAvion { get; set; }
            public decimal Montant { get; set; }
        }

        // Variables pour la pagination
        private int currentPage = 1;
        private int itemsPerPage = 10;
        private int totalItems = 0;
        private int totalPages = 0;

        // Chaîne de connexion à la base de données
        private string connectionString = "Server=localhost;Database=avionDb;Uid=root;Pwd=;";

        // Déclaration des contrôles UI
        private Panel panel1;
        private Label lblTitle;
        private Button btnPrintAll;
        private Button btnPrintSelected;
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
        private TextBox txtSearch;
        private Button btnSearch;
        private ComboBox cmbFilter;

        // Couleurs pour le thème
        private Color primaryColor = Color.FromArgb(0, 123, 255); // Bleu vif
        private Color secondaryColor = Color.FromArgb(108, 117, 125);
        private Color successColor = Color.FromArgb(40, 167, 69);
        private Color dangerColor = Color.FromArgb(220, 53, 69);
        private Color warningColor = Color.FromArgb(255, 193, 7);
        private Color lightGrayColor = Color.FromArgb(240, 240, 240);
        private Color darkBlueColor = Color.FromArgb(26, 35, 126);

        // Propriétés pour l'utilisateur connecté
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserRole { get; set; }

        // Constructeur par défaut
        public Tableau_billet()
        {
            // Configurer l'apparence du contrôle utilisateur
            this.BackColor = Color.White;
            this.Dock = DockStyle.Fill;
            this.Size = new Size(800, 600);

            // Initialiser les contrôles
            InitializeControls();

            // Initialiser le tableau
            SetupDataGridView();

            // Charger les données depuis la base de données
            LoadDataFromDatabase();
        }

        // Constructeur avec paramètres pour les informations utilisateur
        public Tableau_billet(int userId, string userName, string userRole)
        {
            // Stocker les informations utilisateur
            this.UserId = userId;
            this.UserName = userName;
            this.UserRole = userRole;

            // Configurer l'apparence du contrôle utilisateur
            this.BackColor = Color.White;
            this.Dock = DockStyle.Fill;
            this.Size = new Size(800, 600);

            // Initialiser les contrôles
            InitializeControls();

            // Initialiser le tableau
            SetupDataGridView();

            // Charger les données depuis la base de données
            LoadDataFromDatabase();
        }

        private void InitializeControls()
        {
            // Initialiser le panel principal
            panel1 = new Panel
            {
                Dock = DockStyle.Top,
                Height = 100,
                BackColor = primaryColor // Bleu vif
            };
            this.Controls.Add(panel1);

            // Initialiser le titre
            lblTitle = new Label
            {
                Text = "Gestion des Billets",
                Font = new System.Drawing.Font("Segoe UI", 16, System.Drawing.FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(20, 15)
            };
            panel1.Controls.Add(lblTitle);

            // Initialiser la zone de recherche
            txtSearch = new TextBox
            {
                Size = new Size(200, 30),
                Location = new Point(20, 55),
                Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Regular)
            };
            // Définir le texte d'indication au lieu d'utiliser PlaceholderText
            txtSearch.Text = "Rechercher...";
            txtSearch.ForeColor = Color.Gray;
            txtSearch.GotFocus += (sender, e) => {
                if (txtSearch.Text == "Rechercher...")
                {
                    txtSearch.Text = "";
                    txtSearch.ForeColor = Color.Black;
                }
            };
            txtSearch.LostFocus += (sender, e) => {
                if (string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    txtSearch.Text = "Rechercher...";
                    txtSearch.ForeColor = Color.Gray;
                }
            };
            panel1.Controls.Add(txtSearch);

            // Initialiser le filtre
            cmbFilter = new ComboBox
            {
                Size = new Size(150, 30),
                Location = new Point(230, 55),
                Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Regular),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbFilter.Items.AddRange(new object[] { "Tous", "Aujourd'hui", "Cette semaine", "Ce mois" });
            cmbFilter.SelectedIndex = 0;
            panel1.Controls.Add(cmbFilter);

            // Initialiser le bouton de recherche
            btnSearch = new Button
            {
                Text = "Rechercher",
                Size = new Size(100, 30),
                Location = new Point(390, 55),
                FlatStyle = FlatStyle.Flat,
                BackColor = secondaryColor,
                ForeColor = Color.White,
                Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Regular),
                Cursor = Cursors.Hand
            };
            btnSearch.FlatAppearance.BorderColor = secondaryColor;
            btnSearch.FlatAppearance.BorderSize = 0;
            btnSearch.Click += btnSearch_Click;
            panel1.Controls.Add(btnSearch);

            // Initialiser le bouton Imprimer Tous
            btnPrintAll = new Button
            {
                Text = "Imprimer Tous",
                Size = new Size(120, 30),
                Location = new Point(this.Width - 260, 55),
                FlatStyle = FlatStyle.Flat,
                BackColor = successColor,
                ForeColor = Color.White,
                Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Regular),
                Cursor = Cursors.Hand
            };
            btnPrintAll.FlatAppearance.BorderColor = successColor;
            btnPrintAll.FlatAppearance.BorderSize = 0;
            btnPrintAll.Click += btnPrintAll_Click;
            panel1.Controls.Add(btnPrintAll);

            // Initialiser le bouton Imprimer Sélection
            btnPrintSelected = new Button
            {
                Text = "Imprimer Sélection",
                Size = new Size(130, 30),
                Location = new Point(this.Width - 130, 55),
                FlatStyle = FlatStyle.Flat,
                BackColor = warningColor,
                ForeColor = Color.White,
                Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Regular),
                Cursor = Cursors.Hand
            };
            btnPrintSelected.FlatAppearance.BorderColor = warningColor;
            btnPrintSelected.FlatAppearance.BorderSize = 0;
            btnPrintSelected.Click += btnPrintSelected_Click;
            panel1.Controls.Add(btnPrintSelected);

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
                MultiSelect = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                EnableHeadersVisualStyles = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowTemplate = { Height = 40 }
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
                Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Regular),
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
            btnPrevious = CreatePaginationButton("Précédent", 0, 10, 80);
            btnPrevious.Click += btnPrevious_Click;
            panelPagination.Controls.Add(btnPrevious);

            // Boutons de page
            btnPage1 = CreatePaginationButton("1", 90, 10, 30);
            btnPage1.Click += btnPage_Click;
            panelPagination.Controls.Add(btnPage1);

            btnPage2 = CreatePaginationButton("2", 130, 10, 30);
            btnPage2.Click += btnPage_Click;
            panelPagination.Controls.Add(btnPage2);

            btnPage3 = CreatePaginationButton("3", 170, 10, 30);
            btnPage3.Click += btnPage_Click;
            panelPagination.Controls.Add(btnPage3);

            btnPage4 = CreatePaginationButton("4", 210, 10, 30);
            btnPage4.Click += btnPage_Click;
            panelPagination.Controls.Add(btnPage4);

            btnPage5 = CreatePaginationButton("5", 250, 10, 30);
            btnPage5.Click += btnPage_Click;
            panelPagination.Controls.Add(btnPage5);

            // Bouton Suivant
            btnNext = CreatePaginationButton("Suivant", 290, 10, 80);
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
                Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Regular),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderColor = Color.LightGray;
            return btn;
        }

        private void SetupDataGridView()
        {
            // Configurer les colonnes du DataGridView
            dataGridView.Columns.Clear();

            // Colonne de case à cocher pour la sélection
            DataGridViewCheckBoxColumn checkColumn = new DataGridViewCheckBoxColumn
            {
                HeaderText = "",
                Width = 30,
                Name = "checkColumn"
            };
            dataGridView.Columns.Add(checkColumn);

            // Colonne ID Billet
            DataGridViewTextBoxColumn idBilletColumn = new DataGridViewTextBoxColumn
            {
                HeaderText = "ID Billet",
                Name = "IdBillet",
                Width = 60,
                ReadOnly = true
            };
            dataGridView.Columns.Add(idBilletColumn);

            // Colonne Utilisateur
            DataGridViewTextBoxColumn usernameColumn = new DataGridViewTextBoxColumn
            {
                HeaderText = "Utilisateur",
                Name = "Username",
                Width = 100,
                ReadOnly = true
            };
            dataGridView.Columns.Add(usernameColumn);

            // Colonne Départ
            DataGridViewTextBoxColumn departColumn = new DataGridViewTextBoxColumn
            {
                HeaderText = "Départ",
                Name = "Depart",
                Width = 120,
                ReadOnly = true
            };
            dataGridView.Columns.Add(departColumn);

            // Colonne Arrivée
            DataGridViewTextBoxColumn arriveeColumn = new DataGridViewTextBoxColumn
            {
                HeaderText = "Arrivée",
                Name = "Arrivee",
                Width = 120,
                ReadOnly = true
            };
            dataGridView.Columns.Add(arriveeColumn);

            // Colonne Date de réservation
            DataGridViewTextBoxColumn dateReservationColumn = new DataGridViewTextBoxColumn
            {
                HeaderText = "Date de réservation",
                Name = "DateReservation",
                Width = 120,
                ReadOnly = true
            };
            dataGridView.Columns.Add(dateReservationColumn);

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

            // Colonne Modèle d'avion
            DataGridViewTextBoxColumn modeleAvionColumn = new DataGridViewTextBoxColumn
            {
                HeaderText = "Modèle d'avion",
                Name = "ModeleAvion",
                Width = 100,
                ReadOnly = true
            };
            dataGridView.Columns.Add(modeleAvionColumn);

            // Colonne Imprimer
            DataGridViewButtonColumn printColumn = new DataGridViewButtonColumn
            {
                HeaderText = "Action",
                Name = "Print",
                Width = 80,
                Text = "Imprimer",
                UseColumnTextForButtonValue = true,
                FlatStyle = FlatStyle.Flat
            };
            dataGridView.Columns.Add(printColumn);

            // Configurer l'apparence du DataGridView
            dataGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.White;
            dataGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dataGridView.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            dataGridView.ColumnHeadersDefaultCellStyle.Padding = new Padding(5, 0, 0, 0);
            dataGridView.ColumnHeadersHeight = 40;

            dataGridView.DefaultCellStyle.BackColor = Color.White;
            dataGridView.DefaultCellStyle.ForeColor = Color.Black;
            dataGridView.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Regular);
            dataGridView.DefaultCellStyle.Padding = new Padding(5, 0, 0, 0);
            dataGridView.RowTemplate.Height = 40;

            // Alternance de couleurs pour les lignes
            dataGridView.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(249, 249, 249);

            // Sélection de ligne
            dataGridView.DefaultCellStyle.SelectionBackColor = Color.FromArgb(230, 244, 255);
            dataGridView.DefaultCellStyle.SelectionForeColor = Color.Black;

            // Ajouter l'événement pour gérer les clics sur les cellules
            dataGridView.CellClick += DataGridView_CellClick;
            dataGridView.CellFormatting += DataGridView_CellFormatting;

            // Désactiver le tri des colonnes pour éviter les problèmes d'affichage
            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            // Ajuster automatiquement la largeur des colonnes
            AdjustColumnWidths();
        }

        // Méthode pour ajuster automatiquement la largeur des colonnes
        private void AdjustColumnWidths()
        {
            if (dataGridView != null && dataGridView.Columns.Count > 0)
            {
                // Calculer la largeur totale disponible
                int availableWidth = dataGridView.Width - SystemInformation.VerticalScrollBarWidth;

                // Définir des pourcentages pour chaque colonne
                double[] columnPercentages = new double[]
                {
                    0.03, // Checkbox
                    0.05, // ID Billet
                    0.10, // Utilisateur
                    0.12, // Départ
                    0.12, // Arrivée
                    0.12, // Date de réservation
                    0.12, // Date de départ
                    0.12, // Date d'arrivée
                    0.12, // Modèle d'avion
                    0.10  // Imprimer
                };

                // Appliquer les pourcentages
                for (int i = 0; i < dataGridView.Columns.Count && i < columnPercentages.Length; i++)
                {
                    dataGridView.Columns[i].Width = (int)(availableWidth * columnPercentages[i]);
                }
            }
        }

        private void LoadDataFromDatabase(string searchTerm = "", string filterOption = "Tous")
        {
            try
            {
                // Effacer la liste existante
                billetsList.Clear();

                // Créer une connexion à la base de données
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    // Ouvrir la connexion
                    connection.Open();

                    // Construire la requête SQL avec les jointures nécessaires
                    string query = @"
                        SELECT 
                            b.id_billet,
                            b.id_reservation,
                            u.username,
                            aerroDep.pays, 
                            aerroDep.nom_aeroport, 
                            aerroDep.ville AS ville_depart, 
                            aerroArr.ville AS ville_arrivee,
                            r.date_reservation,
                            v.date_depart,
                            v.date_arrive, 
                            av.modele,
                            p.montant
                        FROM 
                            billet b
                        INNER JOIN 
                            reservation r ON b.id_reservation = r.id_reservation 
                        INNER JOIN 
                            paiement p ON b.id_reservation = p.id_reservation
                        INNER JOIN 
                            utilisateur u ON r.id_user = u.id_user 
                        INNER JOIN 
                            vol v ON r.id_vol = v.id_vol 
                        INNER JOIN 
                            aeroport aerroDep ON v.id_aeroport_depart = aerroDep.id_aeroport 
                        INNER JOIN 
                            aeroport aerroArr ON v.id_aeroport_arrive = aerroArr.id_aeroport 
                        INNER JOIN 
                            avion av ON v.id_avion = av.id_avion";

                    // Ajouter des conditions de recherche si nécessaire
                    if (!string.IsNullOrEmpty(searchTerm) && searchTerm != "Rechercher...")
                    {
                        query += @" WHERE 
                            u.username LIKE @searchTerm OR 
                            aerroDep.ville LIKE @searchTerm OR 
                            aerroArr.ville LIKE @searchTerm OR 
                            av.modele LIKE @searchTerm";
                    }

                    // Ajouter des filtres de date si nécessaire
                    if (filterOption != "Tous")
                    {
                        string dateCondition = "";
                        DateTime now = DateTime.Now;

                        switch (filterOption)
                        {
                            case "Aujourd'hui":
                                dateCondition = " DATE(r.date_reservation) = DATE(@today)";
                                break;
                            case "Cette semaine":
                                // Calculer le début de la semaine (lundi)
                                DateTime startOfWeek = now.AddDays(-(int)now.DayOfWeek + (int)DayOfWeek.Monday);
                                dateCondition = " r.date_reservation >= @startDate AND r.date_reservation < @endDate";
                                break;
                            case "Ce mois":
                                // Calculer le début du mois
                                DateTime startOfMonth = new DateTime(now.Year, now.Month, 1);
                                dateCondition = " r.date_reservation >= @startDate AND r.date_reservation < @endDate";
                                break;
                        }

                        if (!string.IsNullOrEmpty(dateCondition))
                        {
                            if (query.Contains("WHERE"))
                                query += " AND " + dateCondition;
                            else
                                query += " WHERE " + dateCondition;
                        }
                    }

                    // Ajouter l'ordre de tri
                    query += " ORDER BY r.date_reservation DESC";

                    MySqlCommand command = new MySqlCommand(query, connection);

                    // Ajouter les paramètres si nécessaire
                    if (!string.IsNullOrEmpty(searchTerm) && searchTerm != "Rechercher...")
                    {
                        command.Parameters.AddWithValue("@searchTerm", "%" + searchTerm + "%");
                    }

                    if (filterOption != "Tous")
                    {
                        DateTime now = DateTime.Now;

                        switch (filterOption)
                        {
                            case "Aujourd'hui":
                                command.Parameters.AddWithValue("@today", now.Date);
                                break;
                            case "Cette semaine":
                                DateTime startOfWeek = now.AddDays(-(int)now.DayOfWeek + (int)DayOfWeek.Monday);
                                DateTime endOfWeek = startOfWeek.AddDays(7);
                                command.Parameters.AddWithValue("@startDate", startOfWeek);
                                command.Parameters.AddWithValue("@endDate", endOfWeek);
                                break;
                            case "Ce mois":
                                DateTime startOfMonth = new DateTime(now.Year, now.Month, 1);
                                DateTime endOfMonth = startOfMonth.AddMonths(1);
                                command.Parameters.AddWithValue("@startDate", startOfMonth);
                                command.Parameters.AddWithValue("@endDate", endOfMonth);
                                break;
                        }
                    }

                    // Exécuter la commande et lire les résultats
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Créer un nouvel objet BilletData pour chaque ligne
                            BilletData billet = new BilletData
                            {
                                IdBillet = Convert.ToInt32(reader["id_billet"]),
                                IdReservation = Convert.ToInt32(reader["id_reservation"]),
                                Username = reader["username"].ToString(),
                                PaysDepart = reader["pays"].ToString(),
                                NomAeroportDepart = reader["nom_aeroport"].ToString(),
                                VilleDepart = reader["ville_depart"].ToString(),
                                VilleArrivee = reader["ville_arrivee"].ToString(),
                                DateReservation = Convert.ToDateTime(reader["date_reservation"]),
                                DateDepart = Convert.ToDateTime(reader["date_depart"]),
                                DateArrivee = Convert.ToDateTime(reader["date_arrive"]),
                                ModeleAvion = reader["modele"].ToString(),
                                Montant = reader["montant"] != DBNull.Value ? Convert.ToDecimal(reader["montant"]) : 0
                            };

                            // Ajouter le billet à la liste
                            billetsList.Add(billet);
                        }
                    }
                }

                // Mettre à jour le total des éléments
                totalItems = billetsList.Count;
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

        private void DataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // Mettre en forme les cellules de bouton
            if (e.ColumnIndex == dataGridView.Columns["Print"].Index && e.RowIndex >= 0)
            {
                e.CellStyle.BackColor = primaryColor;
                e.CellStyle.ForeColor = Color.White;
                e.CellStyle.SelectionBackColor = darkBlueColor;
                e.CellStyle.SelectionForeColor = Color.White;
            }

            // Mettre en forme les dates
            if ((e.ColumnIndex == dataGridView.Columns["DateReservation"].Index ||
                 e.ColumnIndex == dataGridView.Columns["DateDepart"].Index ||
                 e.ColumnIndex == dataGridView.Columns["DateArrivee"].Index) &&
                e.RowIndex >= 0 && e.Value != null)
            {
                if (e.Value is DateTime dateValue)
                {
                    e.Value = dateValue.ToString("dd/MM/yyyy HH:mm");
                    e.FormattingApplied = true;
                }
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
            for (int i = startIndex; i < endIndex && i < billetsList.Count; i++)
            {
                BilletData billet = billetsList[i];

                // Ajouter une nouvelle ligne
                int rowIndex = dataGridView.Rows.Add();
                DataGridViewRow row = dataGridView.Rows[rowIndex];

                // Remplir les cellules
                row.Cells[0].Value = false; // Case à cocher
                row.Cells[1].Value = billet.IdBillet;
                row.Cells[2].Value = billet.Username;
                row.Cells[3].Value = $"{billet.VilleDepart} ({billet.PaysDepart})";
                row.Cells[4].Value = billet.VilleArrivee;
                row.Cells[5].Value = billet.DateReservation;
                row.Cells[6].Value = billet.DateDepart;
                row.Cells[7].Value = billet.DateArrivee;
                row.Cells[8].Value = billet.ModeleAvion;
                // La colonne 9 est le bouton d'impression

                // Stocker l'objet billet dans le Tag de la ligne pour un accès facile
                row.Tag = billet;
            }

            // Mettre à jour le texte de pagination
            UpdatePaginationText();

            // Mettre à jour l'apparence des boutons de pagination
            UpdatePaginationButtons();

            // Ajuster la largeur des colonnes
            AdjustColumnWidths();
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

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchTerm = txtSearch.Text.Trim();
            if (searchTerm == "Rechercher...") searchTerm = "";

            string filterOption = cmbFilter.SelectedItem.ToString();

            // Recharger les données avec les critères de recherche
            LoadDataFromDatabase(searchTerm, filterOption);
        }

        private void DataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Vérifier si le clic est sur une ligne valide et dans la colonne d'impression
            if (e.RowIndex >= 0 && e.ColumnIndex == dataGridView.Columns["Print"].Index)
            {
                // Récupérer l'ID du billet
                int billetId = Convert.ToInt32(dataGridView.Rows[e.RowIndex].Cells["IdBillet"].Value);

                // Récupérer l'objet billet complet depuis le Tag de la ligne
                BilletData billet = dataGridView.Rows[e.RowIndex].Tag as BilletData;

                if (billet != null)
                {
                    // Imprimer le billet en PDF
                    PrintBilletToPdf(billet);
                }
            }
        }

        private void btnPrintAll_Click(object sender, EventArgs e)
        {
            if (billetsList.Count == 0)
            {
                MessageBox.Show("Aucun billet à imprimer.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Demander confirmation
            DialogResult result = MessageBox.Show($"Voulez-vous imprimer tous les billets ({billetsList.Count}) ?",
                                                "Confirmation",
                                                MessageBoxButtons.YesNo,
                                                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // Imprimer tous les billets
                PrintAllBilletsToPdf(billetsList);
            }
        }

        private void btnPrintSelected_Click(object sender, EventArgs e)
        {
            // Récupérer les billets sélectionnés
            List<BilletData> selectedBillets = new List<BilletData>();

            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (row.Cells[0].Value != null && (bool)row.Cells[0].Value)
                {
                    BilletData billet = row.Tag as BilletData;
                    if (billet != null)
                    {
                        selectedBillets.Add(billet);
                    }
                }
            }

            if (selectedBillets.Count == 0)
            {
                MessageBox.Show("Veuillez sélectionner au moins un billet à imprimer.",
                              "Information",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Information);
                return;
            }

            // Imprimer les billets sélectionnés
            PrintAllBilletsToPdf(selectedBillets);
        }

        private void PrintBilletToPdf(BilletData billet)
        {
            try
            {
                // Créer un dossier pour les billets s'il n'existe pas
                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string billetsFolder = Path.Combine(documentsPath, "Billets");

                if (!Directory.Exists(billetsFolder))
                {
                    Directory.CreateDirectory(billetsFolder);
                }

                // Créer un nom de fichier unique basé sur l'ID du billet et la date
                string fileName = $"Billet_{billet.IdBillet}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                string filePath = Path.Combine(billetsFolder, fileName);

                // Créer un nouveau document PDF
                using (FileStream fs = new FileStream(filePath, FileMode.Create))
                {
                    // Configurer le document
                    Document document = new Document(PageSize.A4, 50, 50, 50, 50);
                    PdfWriter writer = PdfWriter.GetInstance(document, fs);
                    document.Open();

                    // Ajouter un titre
                    iTextSharp.text.Font titleFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 20, iTextSharp.text.Font.BOLD);
                    iTextSharp.text.Paragraph title = new iTextSharp.text.Paragraph("BILLET DE VOL", titleFont);
                    title.Alignment = Element.ALIGN_CENTER;
                    title.SpacingAfter = 20;
                    document.Add(title);

                    // Ajouter un logo ou une image (si disponible)
                    try
                    {
                        string logoPath = Path.Combine(Application.StartupPath, "logo.png");
                        if (File.Exists(logoPath))
                        {
                            iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(logoPath);
                            logo.ScaleToFit(100, 100);
                            logo.Alignment = Element.ALIGN_CENTER;
                            document.Add(logo);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Erreur lors du chargement du logo: {ex.Message}");
                    }

                    // Ajouter une ligne de séparation
                    iTextSharp.text.Paragraph separator = new iTextSharp.text.Paragraph("_______________________________________________________");
                    document.Add(separator);

                    // Ajouter les informations du billet
                    iTextSharp.text.Font boldFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, iTextSharp.text.Font.BOLD);
                    iTextSharp.text.Font normalFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12);
                    iTextSharp.text.Font smallFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10);

                    document.Add(new iTextSharp.text.Paragraph($"Billet N°: {billet.IdBillet}", boldFont));
                    document.Add(new iTextSharp.text.Paragraph($"Réservation N°: {billet.IdReservation}", normalFont));
                    document.Add(new iTextSharp.text.Paragraph($"Passager: {billet.Username}", normalFont));
                    document.Add(new iTextSharp.text.Paragraph("\n"));

                    // Ajouter les informations du vol
                    PdfPTable flightTable = new PdfPTable(2);
                    flightTable.WidthPercentage = 100;
                    flightTable.SpacingBefore = 10f;
                    flightTable.SpacingAfter = 10f;

                    // En-têtes
                    flightTable.AddCell(CreateCell("DÉPART", true));
                    flightTable.AddCell(CreateCell("ARRIVÉE", true));

                    // Villes
                    flightTable.AddCell(CreateCell($"{billet.VilleDepart} ({billet.PaysDepart})", false));
                    flightTable.AddCell(CreateCell(billet.VilleArrivee, false));

                    // Aéroport
                    flightTable.AddCell(CreateCell($"Aéroport: {billet.NomAeroportDepart}", false));
                    flightTable.AddCell(CreateCell("", false));

                    // Dates
                    flightTable.AddCell(CreateCell($"Date: {billet.DateDepart:dd/MM/yyyy}", false));
                    flightTable.AddCell(CreateCell($"Date: {billet.DateArrivee:dd/MM/yyyy}", false));

                    // Heures
                    flightTable.AddCell(CreateCell($"Heure: {billet.DateDepart:HH:mm}", false));
                    flightTable.AddCell(CreateCell($"Heure: {billet.DateArrivee:HH:mm}", false));

                    document.Add(flightTable);

                    // Ajouter les informations de l'avion
                    document.Add(new iTextSharp.text.Paragraph($"Avion: {billet.ModeleAvion}", normalFont));
                    document.Add(new iTextSharp.text.Paragraph($"Prix: {billet.Montant:C2}", boldFont));
                    document.Add(new iTextSharp.text.Paragraph($"Date de réservation: {billet.DateReservation:dd/MM/yyyy HH:mm}", smallFont));

                    // Ajouter un code-barres ou QR code (simulé)
                    document.Add(new iTextSharp.text.Paragraph("\n"));
                    document.Add(new iTextSharp.text.Paragraph("Code de réservation", boldFont));

                    //// Créer un code-barres simulé
                    //PdfContentByte cb = writer.DirectContent;
                    //Rctangle rect = new Rectangle(100, 100, 500, 150);
                    //rect.BackgroundColor = BaseColor.BLACK;
                    //cb.Rectangle(rect);

                    // Ajouter des informations légales
                    document.Add(new iTextSharp.text.Paragraph("\n\n"));
                    iTextSharp.text.Font legalFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8, iTextSharp.text.Font.BOLD);
                    iTextSharp.text.Font legalTextFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8);
                    document.Add(new iTextSharp.text.Paragraph("Conditions générales:", legalFont));
                    document.Add(new iTextSharp.text.Paragraph("Ce billet est soumis aux conditions générales de transport. Veuillez vous présenter à l'aéroport au moins 2 heures avant le départ. Une pièce d'identité valide est requise.", legalTextFont));

                    // Ajouter un pied de page
                    document.Add(new iTextSharp.text.Paragraph("\n\n"));
                    iTextSharp.text.Paragraph footer = new iTextSharp.text.Paragraph("Billet généré le " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), legalTextFont);
                    footer.Alignment = Element.ALIGN_CENTER;
                    document.Add(footer);

                    // Fermer le document
                    document.Close();
                }

                // Ouvrir le PDF avec l'application par défaut
                Process.Start(filePath);

                MessageBox.Show($"Le billet a été imprimé avec succès et enregistré dans :\n{filePath}",
                              "Impression réussie",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'impression du billet: {ex.Message}",
                              "Erreur",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
            }
        }

        private PdfPCell CreateCell(string text, bool isHeader)
        {
            iTextSharp.text.Font cellFont = isHeader
                ? new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, iTextSharp.text.Font.BOLD)
                : new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10);

            if (isHeader)
            {
                cellFont.Color = BaseColor.WHITE;
            }

            PdfPCell cell = new PdfPCell(new Phrase(text, cellFont));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Padding = 8;

            if (isHeader)
            {
                cell.BackgroundColor = new BaseColor(0, 123, 255); // primaryColor
                cell.BorderColor = new BaseColor(0, 123, 255);
                cell.BorderWidth = 1.5f;
                cell.PaddingBottom = 10f;
                cell.PaddingTop = 10f;
            }

            return cell;
        }

        private void PrintAllBilletsToPdf(List<BilletData> billets)
        {
            try
            {
                // Créer un dossier pour les billets s'il n'existe pas
                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string billetsFolder = Path.Combine(documentsPath, "Billets");

                if (!Directory.Exists(billetsFolder))
                {
                    Directory.CreateDirectory(billetsFolder);
                }

                // Créer un nom de fichier unique pour le document contenant tous les billets
                string fileName = $"Billets_Multiples_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                string filePath = Path.Combine(billetsFolder, fileName);

                // Créer un nouveau document PDF
                using (FileStream fs = new FileStream(filePath, FileMode.Create))
                {
                    // Configurer le document
                    Document document = new Document(PageSize.A4, 50, 50, 50, 50);
                    PdfWriter writer = PdfWriter.GetInstance(document, fs);
                    document.Open();

                    // Ajouter un titre
                    iTextSharp.text.Font titleFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 20, iTextSharp.text.Font.BOLD);
                    iTextSharp.text.Paragraph title = new iTextSharp.text.Paragraph("BILLETS DE VOL", titleFont);
                    title.Alignment = Element.ALIGN_CENTER;
                    title.SpacingAfter = 20;
                    document.Add(title);

                    // Ajouter la date d'impression
                    iTextSharp.text.Font smallFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10);
                    iTextSharp.text.Paragraph dateImpression = new iTextSharp.text.Paragraph($"Date d'impression: {DateTime.Now:dd/MM/yyyy HH:mm:ss}", smallFont);
                    dateImpression.Alignment = Element.ALIGN_RIGHT;
                    dateImpression.SpacingAfter = 20;
                    document.Add(dateImpression);

                    // Préparer les polices
                    iTextSharp.text.Font boldFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, iTextSharp.text.Font.BOLD);
                    iTextSharp.text.Font normalFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12);
                    iTextSharp.text.Font legalFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8);

                    // Pour chaque billet
                    for (int i = 0; i < billets.Count; i++)
                    {
                        BilletData billet = billets[i];

                        // Ajouter une ligne de séparation
                        iTextSharp.text.Paragraph separator = new iTextSharp.text.Paragraph("_______________________________________________________");
                        document.Add(separator);

                        // Ajouter les informations du billet
                        document.Add(new iTextSharp.text.Paragraph($"Billet N°: {billet.IdBillet}", boldFont));
                        document.Add(new iTextSharp.text.Paragraph($"Réservation N°: {billet.IdReservation}", normalFont));
                        document.Add(new iTextSharp.text.Paragraph($"Passager: {billet.Username}", normalFont));
                        document.Add(new iTextSharp.text.Paragraph("\n"));

                        // Ajouter les informations du vol
                        PdfPTable flightTable = new PdfPTable(2);
                        flightTable.WidthPercentage = 100;
                        flightTable.SpacingBefore = 10f;
                        flightTable.SpacingAfter = 10f;

                        // En-têtes
                        flightTable.AddCell(CreateCell("DÉPART", true));
                        flightTable.AddCell(CreateCell("ARRIVÉE", true));

                        // Villes
                        flightTable.AddCell(CreateCell($"{billet.VilleDepart} ({billet.PaysDepart})", false));
                        flightTable.AddCell(CreateCell(billet.VilleArrivee, false));

                        // Aéroport
                        flightTable.AddCell(CreateCell($"Aéroport: {billet.NomAeroportDepart}", false));
                        flightTable.AddCell(CreateCell("", false));

                        // Dates
                        flightTable.AddCell(CreateCell($"Date: {billet.DateDepart:dd/MM/yyyy}", false));
                        flightTable.AddCell(CreateCell($"Date: {billet.DateArrivee:dd/MM/yyyy}", false));

                        // Heures
                        flightTable.AddCell(CreateCell($"Heure: {billet.DateDepart:HH:mm}", false));
                        flightTable.AddCell(CreateCell($"Heure: {billet.DateArrivee:HH:mm}", false));

                        document.Add(flightTable);

                        // Ajouter les informations de l'avion
                        document.Add(new iTextSharp.text.Paragraph($"Avion: {billet.ModeleAvion}", normalFont));
                        document.Add(new iTextSharp.text.Paragraph($"Prix: {billet.Montant:C2}", boldFont));
                        document.Add(new iTextSharp.text.Paragraph($"Date de réservation: {billet.DateReservation:dd/MM/yyyy HH:mm}", smallFont));

                        // Ajouter un espace entre les billets
                        document.Add(new iTextSharp.text.Paragraph("\n\n"));

                        // Ajouter un saut de page sauf pour le dernier billet
                        if (i < billets.Count - 1)
                        {
                            document.NewPage();
                        }
                    }

                    // Ajouter un pied de page
                    document.Add(new iTextSharp.text.Paragraph("\n\n"));
                    iTextSharp.text.Paragraph footer = new iTextSharp.text.Paragraph($"Document contenant {billets.Count} billet(s) - Généré le " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), legalFont);
                    footer.Alignment = Element.ALIGN_CENTER;
                    document.Add(footer);

                    // Fermer le document
                    document.Close();
                }

                // Ouvrir le PDF avec l'application par défaut
                Process.Start(filePath);

                MessageBox.Show($"{billets.Count} billet(s) ont été imprimés avec succès et enregistrés dans :\n{filePath}",
                              "Impression réussie",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'impression des billets: {ex.Message}",
                              "Erreur",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
            }
        }

        // Gérer le redimensionnement du contrôle
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            // Repositionner les boutons dans le panel supérieur
            if (panel1 != null && btnPrintAll != null && btnPrintSelected != null)
            {
                btnPrintAll.Location = new Point(this.Width - 260, 55);
                btnPrintSelected.Location = new Point(this.Width - 130, 55);
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

            // Ajuster la largeur des colonnes du DataGridView
            AdjustColumnWidths();
        }

        // Méthode pour rafraîchir les données
        public void RefreshData()
        {
            LoadDataFromDatabase();
        }
    }
}