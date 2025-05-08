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
using System.IO;
using System.Net;
using System.Net.Http;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;

namespace AvionManagment
{
    public partial class Tableau_vol : UserControl
    {
        // Liste pour stocker les données des vols
        private List<VolData> volsList = new List<VolData>();

        // Classe pour représenter les données d'un vol
        public class VolData
        {
            public int Id { get; set; }
            public DateTime DateDepart { get; set; }
            public DateTime DateArrive { get; set; }
            public int AvionId { get; set; }
            public string AvionModele { get; set; }
            public int AvionCapacite { get; set; } // Ajout du champ AvionCapacite
            public int AeroportDepartId { get; set; }
            public string AeroportDepartVille { get; set; }
            public int AeroportArriveId { get; set; }
            public string AeroportArriveVille { get; set; }
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
        private Button btnAddVol;
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

        // Images pour les icônes
        private Image editIcon;
        private Image deleteIcon;
        private Image reserveIcon;

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

        // WebView2 pour afficher Stripe
        private WebView2 webView2;
        private Form paymentForm;

        // Variables pour suivre l'état du paiement
        private bool paymentProcessed = false;
        private bool paymentSuccessful = false;

        // Constructeur par défaut
        public Tableau_vol()
        {
            InitializeComponent();

            // Configurer l'apparence du contrôle utilisateur
            this.BackColor = Color.White;
            this.Dock = DockStyle.Fill;

            // Charger les icônes
            LoadIcons();

            // Initialiser les contrôles
            InitializeControls();

            // Initialiser le tableau
            SetupDataGridView();

            // Charger les données depuis la base de données
            LoadDataFromDatabase();
        }

        // Constructeur avec paramètres pour les informations utilisateur
        public Tableau_vol(int userId, string userName, string userRole)
        {
            InitializeComponent();

            // Stocker les informations utilisateur
            this.UserId = userId;
            this.UserName = userName;
            this.UserRole = userRole;

            // Configurer l'apparence du contrôle utilisateur
            this.BackColor = Color.White;
            this.Dock = DockStyle.Fill;

            // Charger les icônes
            LoadIcons();

            // Initialiser les contrôles
            InitializeControls();

            // Initialiser le tableau
            SetupDataGridView();

            // Charger les données depuis la base de données
            LoadDataFromDatabase();
        }

        private void LoadIcons()
        {
            try
            {
                // Essayer de charger les icônes depuis les ressources ou des fichiers locaux
                // Si vous avez des fichiers d'icônes dans votre projet, utilisez:
                // editIcon = Image.FromFile(Path.Combine(Application.StartupPath, "Icons", "edit.png"));
                // deleteIcon = Image.FromFile(Path.Combine(Application.StartupPath, "Icons", "delete.png"));

                // Sinon, vous pouvez utiliser des icônes par défaut ou les dessiner
                editIcon = new Bitmap(24, 24);
                using (Graphics g = Graphics.FromImage(editIcon))
                {
                    g.Clear(warningColor);
                    using (Pen pen = new Pen(Color.White, 2))
                    {
                        g.DrawLine(pen, 6, 18, 18, 6);
                        g.DrawLine(pen, 8, 18, 10, 16);
                        g.DrawLine(pen, 6, 18, 10, 18);
                    }
                }

                deleteIcon = new Bitmap(24, 24);
                using (Graphics g = Graphics.FromImage(deleteIcon))
                {
                    g.Clear(dangerColor);
                    using (Pen pen = new Pen(Color.White, 2))
                    {
                        g.DrawRectangle(pen, 6, 8, 12, 12);
                        g.DrawLine(pen, 4, 8, 20, 8);
                        g.DrawLine(pen, 12, 8, 12, 20);
                    }
                }

                // Créer une icône pour la réservation
                reserveIcon = new Bitmap(24, 24);
                using (Graphics g = Graphics.FromImage(reserveIcon))
                {
                    g.Clear(successColor);
                    using (Pen pen = new Pen(Color.White, 2))
                    {
                        g.DrawEllipse(pen, 4, 4, 16, 16);
                        g.DrawLine(pen, 8, 12, 11, 16);
                        g.DrawLine(pen, 11, 16, 16, 8);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des icônes: {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeControls()
        {
            // Initialiser le panel principal
            panel1 = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = primaryColor // Bleu vif
            };
            this.Controls.Add(panel1);

            // Initialiser le titre
            lblTitle = new Label
            {
                Text = "Gestion des vols",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(20, 15)
            };
            panel1.Controls.Add(lblTitle);

            // Initialiser le bouton Supprimer
            btnDelete = new Button
            {
                Text = "Supprimer",
                Size = new Size(100, 30),
                Location = new Point(panel1.Width - 230, 15),
                FlatStyle = FlatStyle.Flat,
                BackColor = dangerColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                Cursor = Cursors.Hand
            };
            btnDelete.FlatAppearance.BorderColor = dangerColor;
            btnDelete.FlatAppearance.BorderSize = 0;
            btnDelete.Click += btnDelete_Click;
            panel1.Controls.Add(btnDelete);

            // Initialiser le bouton Ajouter
            btnAddVol = new Button
            {
                Text = "Ajouter",
                Size = new Size(100, 30),
                Location = new Point(panel1.Width - 120, 15),
                FlatStyle = FlatStyle.Flat,
                BackColor = successColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                Cursor = Cursors.Hand
            };
            btnAddVol.FlatAppearance.BorderColor = successColor;
            btnAddVol.FlatAppearance.BorderSize = 0;
            btnAddVol.Click += btnAddVol_Click;
            panel1.Controls.Add(btnAddVol);

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
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None, // Changé pour permettre un contrôle précis de la largeur
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

            // Mettre à jour l'affichage des boutons d'administration selon le rôle
            UpdateAdminButtons();
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
                Width = 50,
                ReadOnly = true
            };
            dataGridView.Columns.Add(idColumn);

            // Colonne Date de départ
            DataGridViewTextBoxColumn dateDepartColumn = new DataGridViewTextBoxColumn
            {
                HeaderText = "Départ",
                Name = "DateDepart",
                Width = 130,
                ReadOnly = true
            };
            dataGridView.Columns.Add(dateDepartColumn);

            // Colonne Date d'arrivée
            DataGridViewTextBoxColumn dateArriveColumn = new DataGridViewTextBoxColumn
            {
                HeaderText = "Arrivée",
                Name = "DateArrive",
                Width = 130,
                ReadOnly = true
            };
            dataGridView.Columns.Add(dateArriveColumn);

            // Colonne Avion
            DataGridViewTextBoxColumn avionColumn = new DataGridViewTextBoxColumn
            {
                HeaderText = "Avion",
                Name = "Avion",
                Width = 100,
                ReadOnly = true
            };
            dataGridView.Columns.Add(avionColumn);

            // Colonne Places disponibles
            DataGridViewTextBoxColumn avionCapacite = new DataGridViewTextBoxColumn
            {
                HeaderText = "Places disponibles",
                Name = "AvionCapacite",
                Width = 120,
                ReadOnly = true
            };
            dataGridView.Columns.Add(avionCapacite);

            // Colonne Aéroport de départ
            DataGridViewTextBoxColumn aeroportDepartColumn = new DataGridViewTextBoxColumn
            {
                HeaderText = "De",
                Name = "AeroportDepart",
                Width = 80,
                ReadOnly = true
            };
            dataGridView.Columns.Add(aeroportDepartColumn);

            // Colonne Aéroport d'arrivée
            DataGridViewTextBoxColumn aeroportArriveColumn = new DataGridViewTextBoxColumn
            {
                HeaderText = "À",
                Name = "AeroportArrive",
                Width = 80,
                ReadOnly = true
            };
            dataGridView.Columns.Add(aeroportArriveColumn);

            // Colonne Actions
            DataGridViewImageColumn actionsColumn = new DataGridViewImageColumn
            {
                HeaderText = "Actions",
                Name = "Actions",
                Width = 100,
                ReadOnly = true,
                ImageLayout = DataGridViewImageCellLayout.Zoom
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

            // Ajouter l'événement pour gérer les clics sur les cellules
            dataGridView.CellClick += DataGridView_CellClick;
            dataGridView.CellFormatting += DataGridView_CellFormatting;

            // Désactiver le tri des colonnes pour éviter les problèmes d'affichage
            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            // Activer le défilement horizontal
            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dataGridView.ScrollBars = ScrollBars.Both;
        }

        private void LoadDataFromDatabase()
        {
            try
            {
                // Effacer la liste existante
                volsList.Clear();

                // Créer une connexion à la base de données
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    // Ouvrir la connexion
                    connection.Open();

                    // Créer une commande SQL pour récupérer les vols avec jointures
                    string query = @"
    SELECT v.id_vol, v.date_depart, v.date_arrive, 
           v.id_avion, a.modele AS avion_modele, a.capacite AS avion_capacite,
           v.id_aeroport_depart, ad.ville AS aeroport_depart_ville,
           v.id_aeroport_arrive, aa.ville AS aeroport_arrive_ville
    FROM vol v
    JOIN avion a ON v.id_avion = a.id_avion
    JOIN aeroport ad ON v.id_aeroport_depart = ad.id_aeroport
    JOIN aeroport aa ON v.id_aeroport_arrive = aa.id_aeroport
    ORDER BY v.date_depart DESC";

                    MySqlCommand command = new MySqlCommand(query, connection);

                    // Exécuter la commande et lire les résultats
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Créer un nouvel objet VolData pour chaque ligne
                            VolData vol = new VolData
                            {
                                Id = Convert.ToInt32(reader["id_vol"]),
                                DateDepart = Convert.ToDateTime(reader["date_depart"]),
                                DateArrive = Convert.ToDateTime(reader["date_arrive"]),
                                AvionId = Convert.ToInt32(reader["id_avion"]),
                                AvionModele = reader["avion_modele"].ToString(),
                                AvionCapacite = Convert.ToInt32(reader["avion_capacite"]), // Lecture du champ AvionCapacite
                                AeroportDepartId = Convert.ToInt32(reader["id_aeroport_depart"]),
                                AeroportDepartVille = reader["aeroport_depart_ville"].ToString(),
                                AeroportArriveId = Convert.ToInt32(reader["id_aeroport_arrive"]),
                                AeroportArriveVille = reader["aeroport_arrive_ville"].ToString()
                            };

                            // Ajouter le vol à la liste
                            volsList.Add(vol);
                        }
                    }
                }

                // Mettre à jour le total des éléments
                totalItems = volsList.Count;
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
            // Mettre en évidence la colonne des places disponibles
            if (e.ColumnIndex == 5 && e.RowIndex >= 0)
            {
                e.CellStyle.BackColor = Color.FromArgb(232, 245, 233);
                e.CellStyle.ForeColor = Color.FromArgb(46, 125, 50);
                e.CellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
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

            // Créer les images d'actions selon le rôle de l'utilisateur
            Bitmap actionsImage;

            // Vérifier si l'utilisateur est un administrateur
            bool isAdmin = UserRole != null && UserRole.ToLower() == "admin";

            // Créer l'image appropriée selon le rôle
            if (isAdmin)
            {
                // Pour les administrateurs, afficher les icônes d'édition et de suppression
                actionsImage = new Bitmap(80, 24);
                using (Graphics g = Graphics.FromImage(actionsImage))
                {
                    g.Clear(Color.Transparent);
                    g.DrawImage(editIcon, 0, 0, 24, 24);
                    g.DrawImage(deleteIcon, 40, 0, 24, 24);
                }
            }
            else
            {
                // Pour les utilisateurs normaux, afficher uniquement l'icône de réservation
                actionsImage = new Bitmap(24, 24);
                using (Graphics g = Graphics.FromImage(actionsImage))
                {
                    g.Clear(Color.Transparent);
                    g.DrawImage(reserveIcon, 0, 0, 24, 24);
                }
            }

            // Ajouter les données à la grille
            for (int i = startIndex; i < endIndex && i < volsList.Count; i++)
            {
                VolData vol = volsList[i];

                // Formater les dates
                string dateDepart = vol.DateDepart.ToString("dd/MM/yyyy HH:mm");
                string dateArrive = vol.DateArrive.ToString("dd/MM/yyyy HH:mm");

                // Ajouter une nouvelle ligne
                int rowIndex = dataGridView.Rows.Add();
                DataGridViewRow row = dataGridView.Rows[rowIndex];

                // Remplir les cellules
                row.Cells[0].Value = false; // Case à cocher
                row.Cells[1].Value = vol.Id;
                row.Cells[2].Value = dateDepart;
                row.Cells[3].Value = dateArrive;
                row.Cells[4].Value = $"{vol.AvionId} - {vol.AvionModele}";
                row.Cells[5].Value = vol.AvionCapacite; // Affichage de la capacité
                row.Cells[6].Value = vol.AeroportDepartVille;
                row.Cells[7].Value = vol.AeroportArriveVille;
                row.Cells[8].Value = actionsImage; // Image d'actions selon le rôle
            }

            // Mettre à jour le texte de pagination
            UpdatePaginationText();

            // Mettre à jour l'apparence des boutons de pagination
            UpdatePaginationButtons();

            // Afficher/masquer les boutons d'administration selon le rôle
            UpdateAdminButtons();
        }

        private void UpdateAdminButtons()
        {
            // Vérifier si l'utilisateur est un administrateur
            bool isAdmin = UserRole != null && UserRole.ToLower() == "admin";

            // Afficher ou masquer les boutons d'administration
            if (btnDelete != null) btnDelete.Visible = isAdmin;
            if (btnAddVol != null) btnAddVol.Visible = isAdmin;
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
            // Vérifier si le clic est sur une ligne valide et dans la colonne Actions
            if (e.RowIndex >= 0 && e.ColumnIndex == 8) // Colonne Actions
            {
                int volId = Convert.ToInt32(dataGridView.Rows[e.RowIndex].Cells[1].Value);

                // Vérifier le rôle de l'utilisateur
                bool isAdmin = UserRole != null && UserRole.ToLower() == "admin";

                if (isAdmin)
                {
                    // Pour les administrateurs, déterminer si c'est un clic sur l'icône d'édition ou de suppression
                    Rectangle cellRect = dataGridView.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);
                    Point clickPoint = dataGridView.PointToClient(Cursor.Position);

                    // Calculer la position relative du clic dans la cellule
                    int relativeX = clickPoint.X - cellRect.Left;
                    int cellMiddle = cellRect.Width / 2;

                    if (relativeX < cellMiddle) // Clic sur l'icône d'édition (gauche)
                    {
                        EditVol(volId);
                    }
                    else // Clic sur l'icône de suppression (droite)
                    {
                        DeleteVol(volId);
                    }
                }
                else
                {
                    // Pour les utilisateurs normaux, effectuer une réservation
                    ReserverVol(volId);
                }
            }
        }

        private void EditVol(int volId)
        {
            // Trouver le vol dans la liste
            VolData vol = volsList.FirstOrDefault(v => v.Id == volId);
            if (vol != null)
            {
                // Créer une instance de la fenêtre UpdateVolWindow
                UpdateVolWindow updateVolWindow = new UpdateVolWindow
                {
                    VolId = vol.Id,
                    DateDepart = vol.DateDepart,
                    DateArrive = vol.DateArrive,
                    AvionId = vol.AvionId,
                    AeroportDepartId = vol.AeroportDepartId,
                    AeroportArriveId = vol.AeroportArriveId
                };

                // Afficher la fenêtre en modal
                bool? result = updateVolWindow.ShowDialog();

                // Si la mise à jour est confirmée
                if (result == true)
                {
                    // Recharger les données
                    LoadDataFromDatabase();
                }
            }
        }

        private void DeleteVol(int volId)
        {
            // Demander confirmation
            DialogResult result = MessageBox.Show($"Voulez-vous vraiment supprimer ce vol?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // Supprimer le vol de la base de données
                if (DeleteVolFromDatabase(volId))
                {
                    // Recharger les données
                    LoadDataFromDatabase();
                }
            }
        }

        private async Task<string> DemarrerPaiementStripe(int volId, decimal amount, string productName)
        {
            // Configuration spéciale pour le développement (contournement SSL)
            HttpClientHandler handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };

            using (HttpClient client = new HttpClient(handler))
            {
                try
                {
                    // 1. Préparation des données
                    var requestData = new
                    {
                        Amount = amount,
                        ProductName = productName,
                        Description = $"Réservation de vol #{volId}",
                        VolId = volId,
                        UserId = this.UserId,
                        BaseUrl = "https://localhost" // Peut être une URL factice pour une app desktop
                    };

                    // 2. Sérialisation
                    string json = JsonConvert.SerializeObject(requestData);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    // 3. Configuration du timeout
                    client.Timeout = TimeSpan.FromSeconds(30);

                    // 4. En-têtes
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));

                    // 5. URL de l'API (vérifiez le port dans votre projet API)
                    string apiUrl = "https://localhost:7286/api/Stripe/create-checkout-session";

                    // 6. Envoi de la requête avec journalisation
                    Debug.WriteLine($"Envoi à: {apiUrl}\nDonnées: {json}");

                    HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                    // 7. Vérification de la réponse
                    if (!response.IsSuccessStatusCode)
                    {
                        string errorContent = await response.Content.ReadAsStringAsync();
                        Debug.WriteLine($"Erreur API: {response.StatusCode}\n{errorContent}");
                        throw new Exception($"Erreur du serveur: {response.StatusCode}\n{errorContent}");
                    }

                    // 8. Traitement de la réponse
                    string responseContent = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"Réponse reçue: {responseContent}");

                    dynamic result = JsonConvert.DeserializeObject(responseContent);
                    return result.url;
                }
                catch (TaskCanceledException)
                {
                    MessageBox.Show("Délai d'attente dépassé. Vérifiez votre connexion.",
                                  "Erreur",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Error);
                    return null;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"ERREUR COMPLÈTE: {ex.ToString()}");
                    MessageBox.Show($"Échec de la connexion à l'API:\n{ex.Message}",
                                  "Erreur",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Error);
                    return null;
                }
            }
        }

        private async void ReserverVol(int volId)
        {
            VolData vol = volsList.FirstOrDefault(v => v.Id == volId);
            if (vol == null) return;

            if (vol.AvionCapacite <= 0)
            {
                MessageBox.Show("Désolé, il n'y a plus de places disponibles pour ce vol.",
                              "Information",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Information);
                return;
            }

            // Confirmation utilisateur
            DialogResult confirmation = MessageBox.Show(
                $"Voulez-vous réserver une place pour le vol de {vol.AeroportDepartVille} à {vol.AeroportArriveVille} le {vol.DateDepart:dd/MM/yyyy HH:mm} ?\n\nPrix: 100€",
                "Confirmation de réservation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirmation != DialogResult.Yes) return;

            try
            {
                // Réinitialiser les variables de suivi du paiement
                paymentProcessed = false;
                paymentSuccessful = false;

                // Démarrer le processus de paiement
                string checkoutUrl = await DemarrerPaiementStripe(
                    volId,
                    100.00m,
                    $"Vol {vol.AeroportDepartVille}-{vol.AeroportArriveVille}");

                if (string.IsNullOrEmpty(checkoutUrl))
                {
                    return; // L'erreur a déjà été affichée
                }

                // Afficher la fenêtre de paiement Stripe avec WebView2
                ShowStripePaymentWindow(checkoutUrl, volId);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la réservation:\n{ex.Message}",
                              "Erreur",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
            }
        }

        // Méthode pour afficher la fenêtre de paiement Stripe avec WebView2
        private void ShowStripePaymentWindow(string url, int volId)
        {
            try
            {
                // Créer une nouvelle fenêtre pour le paiement
                paymentForm = new Form
                {
                    Text = "Paiement Stripe - MODE TEST",
                    Width = 800,
                    Height = 700,
                    StartPosition = FormStartPosition.CenterParent,
                    Icon = this.ParentForm?.Icon
                };

                // Ajouter un label pour indiquer le mode test
                Label lblTestMode = new Label
                {
                    Text = "MODE TEST - Aucun paiement réel ne sera effectué",
                    Dock = DockStyle.Top,
                    Height = 30,
                    BackColor = Color.Yellow,
                    ForeColor = Color.Black,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold)
                };
                paymentForm.Controls.Add(lblTestMode);

                // Créer un panel pour les informations de test
                Panel testInfoPanel = new Panel
                {
                    Dock = DockStyle.Bottom,
                    Height = 100,
                    BackColor = Color.LightYellow,
                    BorderStyle = BorderStyle.FixedSingle
                };

                Label lblTestInfo = new Label
                {
                    Text = "Cartes de test Stripe :\n" +
                           "✅ Succès : 4242 4242 4242 4242\n" +
                           "🔐 Authentification : 4000 0025 0000 3155\n" +
                           "❌ Refusée : 4000 0000 0000 9995\n" +
                           "Pour tous : date future, CVC = 3 chiffres, code postal = 5 chiffres",
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = new Font("Segoe UI", 9, FontStyle.Regular)
                };

                testInfoPanel.Controls.Add(lblTestInfo);
                paymentForm.Controls.Add(testInfoPanel);

                // Créer un panel de chargement
                Panel loadingPanel = new Panel
                {
                    Dock = DockStyle.Fill,
                    BackColor = Color.White,
                    Visible = false
                };

                Label lblLoading = new Label
                {
                    Text = "Traitement du paiement en cours...",
                    AutoSize = false,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Fill,
                    Font = new Font("Segoe UI", 14, FontStyle.Bold)
                };

                PictureBox loadingIcon = new PictureBox
                {
                    Size = new Size(50, 50),
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    Location = new Point((loadingPanel.Width - 50) / 2, (loadingPanel.Height - 50) / 2 - 50)
                };

                // Créer une animation de chargement simple
                Bitmap loadingImage = new Bitmap(50, 50);
                using (Graphics g = Graphics.FromImage(loadingImage))
                {
                    g.Clear(Color.White);
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    using (Pen pen = new Pen(primaryColor, 5))
                    {
                        g.DrawEllipse(pen, 5, 5, 40, 40);
                    }
                    using (Pen pen = new Pen(Color.Gray, 5))
                    {
                        g.DrawArc(pen, 5, 5, 40, 40, 0, 270);
                    }
                }
                loadingIcon.Image = loadingImage;

                loadingPanel.Controls.Add(lblLoading);
                loadingPanel.Controls.Add(loadingIcon);
                paymentForm.Controls.Add(loadingPanel);

                // Créer le contrôle WebView2
                webView2 = new WebView2
                {
                    Dock = DockStyle.Fill
                };

                // Ajouter un bouton pour fermer la fenêtre
                Button btnClose = new Button
                {
                    Text = "Fermer",
                    Dock = DockStyle.Bottom,
                    Height = 40,
                    BackColor = dangerColor,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
                btnClose.FlatAppearance.BorderSize = 0;
                btnClose.Click += (s, e) =>
                {
                    // Nettoyer les ressources WebView2 avant de fermer
                    if (webView2 != null)
                    {
                        if (webView2.CoreWebView2 != null)
                        {
                            webView2.CoreWebView2.NavigationStarting -= CoreWebView2_NavigationStarting;
                            webView2.CoreWebView2.NavigationCompleted -= CoreWebView2_NavigationCompleted;
                            webView2.CoreWebView2.ProcessFailed -= CoreWebView2_ProcessFailed;
                            webView2.CoreWebView2.WebResourceResponseReceived -= CoreWebView2_WebResourceResponseReceived;
                            webView2.CoreWebView2.SourceChanged -= CoreWebView2_SourceChanged;
                            webView2.CoreWebView2.DOMContentLoaded -= CoreWebView2_DOMContentLoaded;
                        }
                    }
                    paymentForm.Close();
                };

                // Ajouter les contrôles à la fenêtre
                paymentForm.Controls.Add(webView2);
                paymentForm.Controls.Add(btnClose);

                // Gérer l'événement FormClosing pour nettoyer les ressources
                paymentForm.FormClosing += (s, e) =>
                {
                    // Traiter le résultat du paiement si la fenêtre est fermée sans que le paiement ait été traité
                    if (!paymentProcessed)
                    {
                        // Considérer comme annulé si fermé manuellement
                        paymentProcessed = true;
                        paymentSuccessful = false;
                    }
                };

                // Gérer l'événement FormClosed pour finaliser le processus
                paymentForm.FormClosed += async (s, e) =>
                {
                    // Traiter le résultat du paiement
                    if (paymentSuccessful)
                    {
                        if (AjouterReservationEnBaseDeDonnees(volId, this.UserId))
                        {
                            MessageBox.Show("Réservation confirmée ! (Mode Test)",
                                          "Succès",
                                          MessageBoxButtons.OK,
                                          MessageBoxIcon.Information);
                            LoadDataFromDatabase();
                        }
                    }
                    else if (paymentProcessed) // Si le paiement a été traité mais n'a pas réussi
                    {
                        MessageBox.Show("Paiement annulé ou échoué. (Mode Test)",
                                      "Information",
                                      MessageBoxButtons.OK,
                                      MessageBoxIcon.Information);
                    }

                    // Libérer les ressources
                    if (webView2 != null)
                    {
                        webView2.Dispose();
                        webView2 = null;
                    }
                };

                // Initialiser WebView2 de manière asynchrone
                InitializeWebView2Async(url, loadingPanel);

                // Afficher la fenêtre de paiement
                paymentForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'affichage de la fenêtre de paiement: {ex.Message}",
                              "Erreur",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
            }
        }

        // Méthode pour initialiser WebView2 de manière asynchrone
        private async void InitializeWebView2Async(string url, Panel loadingPanel)
        {
            try
            {
                // Initialiser l'environnement WebView2
                await webView2.EnsureCoreWebView2Async(null);

                // Configurer les options WebView2
                webView2.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
                webView2.CoreWebView2.Settings.AreDevToolsEnabled = false;

                // IMPORTANT: Ajouter un gestionnaire d'événements pour les changements d'URL
                webView2.CoreWebView2.SourceChanged += CoreWebView2_SourceChanged;

                // Configurer les gestionnaires d'événements existants
                webView2.CoreWebView2.NavigationStarting += CoreWebView2_NavigationStarting;
                webView2.CoreWebView2.NavigationCompleted += CoreWebView2_NavigationCompleted;
                webView2.CoreWebView2.ProcessFailed += CoreWebView2_ProcessFailed;
                webView2.CoreWebView2.WebResourceResponseReceived += CoreWebView2_WebResourceResponseReceived;

                // NOUVEAU: Ajouter un gestionnaire pour les scripts exécutés
                webView2.CoreWebView2.DOMContentLoaded += CoreWebView2_DOMContentLoaded;

                // Naviguer vers l'URL de paiement Stripe
                webView2.CoreWebView2.Navigate(url);

                // IMPORTANT: Créer un timer de secours qui fermera la fenêtre après un délai
                System.Windows.Forms.Timer backupTimer = new System.Windows.Forms.Timer();
                backupTimer.Interval = 30000; // 30 secondes maximum pour tout le processus
                backupTimer.Tick += (sender, e) => {
                    backupTimer.Stop();

                    // Vérifier si le paiement a été traité
                    if (!paymentProcessed)
                    {
                        // Injecter du JavaScript pour vérifier si la page contient des indicateurs de succès
                        CheckPaymentStatusViaJS();
                    }
                };
                backupTimer.Start();

                // Démarrer un timer pour vérifier le statut du paiement régulièrement
                System.Windows.Forms.Timer checkPaymentTimer = new System.Windows.Forms.Timer();
                checkPaymentTimer.Interval = 2000; // Vérifier toutes les 2 secondes
                checkPaymentTimer.Tick += async (sender, e) => {
                    try
                    {
                        // Vérifier le statut du paiement via l'API
                        bool paymentStatus = await CheckPaymentStatus(url);

                        // Afficher des informations de débogage
                        Debug.WriteLine($"Vérification du statut de paiement: {paymentStatus}");

                        if (paymentStatus)
                        {
                            checkPaymentTimer.Stop();
                            backupTimer.Stop();

                            paymentProcessed = true;
                            paymentSuccessful = true;

                            // Afficher le panel de chargement
                            BeginInvoke(new Action(() => {
                                if (webView2 != null && !webView2.IsDisposed)
                                    webView2.Visible = false;

                                if (loadingPanel != null)
                                    loadingPanel.Visible = true;
                            }));

                            // Attendre un peu pour montrer le message de chargement
                            await Task.Delay(1000);

                            // Fermer la fenêtre
                            BeginInvoke(new Action(() => {
                                if (paymentForm != null && !paymentForm.IsDisposed)
                                {
                                    paymentForm.Close();
                                }
                            }));
                        }
                        else
                        {
                            // Vérifier également via JavaScript
                            CheckPaymentStatusViaJS();
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Erreur lors de la vérification du statut: {ex.Message}");
                    }
                };
                checkPaymentTimer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'initialisation de WebView2: {ex.Message}\n\n" +
                              "Assurez-vous que le runtime WebView2 est installé sur votre système.",
                              "Erreur",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);

                if (paymentForm != null && !paymentForm.IsDisposed)
                {
                    paymentForm.Close();
                }
            }
        }

        // NOUVEAU: Gestionnaire pour les changements d'URL source
        private void CoreWebView2_SourceChanged(object sender, CoreWebView2SourceChangedEventArgs e)
        {
            string currentUrl = webView2.Source.ToString();
            Debug.WriteLine($"URL changée: {currentUrl}");

            // Vérifier si l'URL contient des indicateurs de succès
            if (currentUrl.Contains("paiement-reussi") ||
                currentUrl.Contains("success=true") ||
                currentUrl.Contains("payment_intent_client_secret") ||
                currentUrl.Contains("redirect_status=succeeded"))
            {
                Debug.WriteLine("URL de succès détectée!");

                if (!paymentProcessed)
                {
                    paymentProcessed = true;
                    paymentSuccessful = true;

                    // Afficher la page de succès
                    ShowSuccessPage();

                    // Fermer la fenêtre après un court délai
                    BeginInvoke(new Action(async () => {
                        await Task.Delay(2000);
                        if (paymentForm != null && !paymentForm.IsDisposed)
                        {
                            paymentForm.Close();
                        }
                    }));
                }
            }
            else if (currentUrl.Contains("paiement-annule") ||
                     currentUrl.Contains("success=false") ||
                     currentUrl.Contains("redirect_status=failed"))
            {
                Debug.WriteLine("URL d'échec détectée!");

                if (!paymentProcessed)
                {
                    paymentProcessed = true;
                    paymentSuccessful = false;

                    // Afficher la page d'annulation
                    ShowCancelPage();

                    // Fermer la fenêtre après un court délai
                    BeginInvoke(new Action(async () => {
                        await Task.Delay(2000);
                        if (paymentForm != null && !paymentForm.IsDisposed)
                        {
                            paymentForm.Close();
                        }
                    }));
                }
            }
        }

        // NOUVEAU: Gestionnaire pour le chargement du DOM
        private void CoreWebView2_DOMContentLoaded(object sender, CoreWebView2DOMContentLoadedEventArgs e)
        {
            // Vérifier si la page contient des indicateurs de succès via JavaScript
            CheckPaymentStatusViaJS();
        }

        // NOUVEAU: Méthode pour vérifier le statut du paiement via JavaScript
        private async void CheckPaymentStatusViaJS()
        {
            try
            {
                if (webView2 != null && webView2.CoreWebView2 != null)
                {
                    // Script pour vérifier les éléments de la page qui pourraient indiquer un succès
                    string script = @"
                        (function() {
                            // Vérifier le titre de la page
                            if (document.title.includes('Success') || document.title.includes('Succès')) {
                                return 'success_title';
                            }
                            
                            // Vérifier les éléments de texte qui pourraient indiquer un succès
                            const bodyText = document.body.innerText;
                            if (bodyText.includes('Payment successful') || 
                                bodyText.includes('Paiement réussi') || 
                                bodyText.includes('Thank you for your payment') ||
                                bodyText.includes('Merci pour votre paiement')) {
                                return 'success_text';
                            }
                            
                            // Vérifier les classes CSS qui pourraient indiquer un succès
                            if (document.querySelector('.success') || 
                                document.querySelector('.payment-success') ||
                                document.querySelector('[data-status=success]')) {
                                return 'success_element';
                            }
                            
                            // Vérifier l'URL
                            if (window.location.href.includes('success') || 
                                window.location.href.includes('succeeded') ||
                                window.location.href.includes('completed')) {
                                return 'success_url';
                            }
                            
                            return 'no_indicators';
                        })();
                    ";

                    string result = await webView2.CoreWebView2.ExecuteScriptAsync(script);
                    Debug.WriteLine($"Résultat de la vérification JS: {result}");

                    // Si un indicateur de succès est trouvé
                    if (result.Contains("success"))
                    {
                        if (!paymentProcessed)
                        {
                            paymentProcessed = true;
                            paymentSuccessful = true;

                            // Fermer la fenêtre
                            BeginInvoke(new Action(() => {
                                if (paymentForm != null && !paymentForm.IsDisposed)
                                {
                                    paymentForm.Close();
                                }
                            }));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erreur lors de l'exécution du script JS: {ex.Message}");
            }
        }

        // Gestionnaire d'événements pour le début de la navigation
        private void CoreWebView2_NavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e)
        {
            string url = e.Uri;
            Debug.WriteLine($"Navigation démarrée vers: {url}");

            // Vérifier si l'URL contient des indicateurs de succès ou d'échec
            if (url.Contains("/paiement-reussi") || url.Contains("success=true"))
            {
                // Marquer le paiement comme réussi et traité
                paymentProcessed = true;
                paymentSuccessful = true;

                // Annuler la navigation vers l'URL de succès qui pourrait échouer
                e.Cancel = true;

                // Afficher la page de succès
                BeginInvoke(new Action(() => {
                    ShowSuccessPage();
                }));

                // Fermer la fenêtre de paiement après un court délai
                BeginInvoke(new Action(async () => {
                    await Task.Delay(2000);
                    if (paymentForm != null && !paymentForm.IsDisposed)
                    {
                        paymentForm.Close();
                    }
                }));
            }
            else if (url.Contains("/paiement-annule") || url.Contains("success=false"))
            {
                // Marquer le paiement comme échoué et traité
                paymentProcessed = true;
                paymentSuccessful = false;

                // Annuler la navigation vers l'URL d'échec qui pourrait échouer
                e.Cancel = true;

                // Afficher la page d'annulation
                BeginInvoke(new Action(() => {
                    ShowCancelPage();
                }));

                // Fermer la fenêtre de paiement après un court délai
                BeginInvoke(new Action(async () => {
                    await Task.Delay(2000);
                    if (paymentForm != null && !paymentForm.IsDisposed)
                    {
                        paymentForm.Close();
                    }
                }));
            }
        }

        // Gestionnaire d'événements pour la fin de la navigation
        private void CoreWebView2_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            if (!e.IsSuccess)
            {
                Debug.WriteLine($"Erreur de navigation: {e.WebErrorStatus}");

                // Si la navigation échoue et que nous n'avons pas encore traité le paiement
                if (!paymentProcessed)
                {
                    // Vérifier si l'URL actuelle contient des indicateurs de succès ou d'échec
                    string currentUrl = webView2.Source.ToString();

                    if (currentUrl.Contains("/paiement-reussi") || currentUrl.Contains("success=true"))
                    {
                        // Marquer le paiement comme réussi et traité
                        paymentProcessed = true;
                        paymentSuccessful = true;

                        // Afficher la page de succès
                        BeginInvoke(new Action(() => {
                            ShowSuccessPage();
                        }));

                        // Fermer la fenêtre de paiement après un court délai
                        BeginInvoke(new Action(async () => {
                            await Task.Delay(2000);
                            if (paymentForm != null && !paymentForm.IsDisposed)
                            {
                                paymentForm.Close();
                            }
                        }));
                    }
                    else if (currentUrl.Contains("/paiement-annule") || currentUrl.Contains("success=false")
                     || currentUrl.Contains("success=false"))
                    {
                            // Marquer le paiement comme échoué et traité
                            paymentProcessed = true;
                            paymentSuccessful = false;

                            // Afficher la page d'annulation
                            BeginInvoke(new Action(() => {
                                ShowCancelPage();
                            }));

                            // Fermer la fenêtre de paiement après un court délai
                            BeginInvoke(new Action(async () => {
                                await Task.Delay(2000);
                                if (paymentForm != null && !paymentForm.IsDisposed)
                                {
                                    paymentForm.Close();
                                }
                            }));
                        }
                    else if (e.WebErrorStatus == CoreWebView2WebErrorStatus.ConnectionAborted ||
                             e.WebErrorStatus == CoreWebView2WebErrorStatus.Disconnected ||
                             e.WebErrorStatus == CoreWebView2WebErrorStatus.HostNameNotResolved ||
                             e.WebErrorStatus == CoreWebView2WebErrorStatus.ConnectionReset)
                        {
                            // Si l'erreur est liée à la connexion et que l'URL contient des indicateurs
                            string url = webView2.Source.ToString();

                            if (url.Contains("/paiement-reussi") || url.Contains("success=true"))
                            {
                                // Marquer le paiement comme réussi et traité
                                paymentProcessed = true;
                                paymentSuccessful = true;

                                // Afficher une page HTML de succès au lieu de l'erreur
                                ShowSuccessPage();
                            }
                            else if (url.Contains("/paiement-annule") || url.Contains("success=false"))
                            {
                                // Marquer le paiement comme échoué et traité
                                paymentProcessed = true;
                                paymentSuccessful = false;

                                // Afficher une page HTML d'annulation au lieu de l'erreur
                                ShowCancelPage();
                            }
                            else
                            {
                                // Afficher une page personnalisée pour les erreurs de connexion
                                ShowCustomErrorPage();
                            }
                        }
                    }
                }
            }

        // Gestionnaire d'événements pour les erreurs de processus
        private void CoreWebView2_ProcessFailed(object sender, CoreWebView2ProcessFailedEventArgs e)
        {
            Debug.WriteLine($"Erreur de processus WebView2: {e.ProcessFailedKind}");

            // Si le processus échoue, vérifier si nous avons déjà un statut de paiement
            if (!paymentProcessed)
            {
                // Afficher une page personnalisée au lieu de l'erreur
                ShowCustomErrorPage();
            }
        }

        // Gestionnaire pour les réponses de ressources web
        private void CoreWebView2_WebResourceResponseReceived(object sender, CoreWebView2WebResourceResponseReceivedEventArgs e)
        {
            // Vérifier si la réponse est un code d'erreur HTTP
            if (e.Response.StatusCode >= 400)
            {
                Debug.WriteLine($"Erreur HTTP {e.Response.StatusCode} pour {e.Request.Uri}");

                // Si l'URL contient des indicateurs de succès ou d'échec
                string url = e.Request.Uri;

                if (url.Contains("/paiement-reussi") || url.Contains("success=true"))
                {
                    // Marquer le paiement comme réussi et traité
                    paymentProcessed = true;
                    paymentSuccessful = true;

                    // Afficher une page HTML de succès au lieu de l'erreur
                    BeginInvoke(new Action(() =>
                    {
                        ShowSuccessPage();
                    }));
                }
                else if (url.Contains("/paiement-annule") || url.Contains("success=false"))
                {
                    // Marquer le paiement comme échoué et traité
                    paymentProcessed = true;
                    paymentSuccessful = false;

                    // Afficher une page HTML d'annulation au lieu de l'erreur
                    BeginInvoke(new Action(() =>
                    {
                        ShowCancelPage();
                    }));
                }
            }
        }

        // Méthode pour vérifier le statut du paiement
        private async Task<bool> CheckPaymentStatus(string checkoutUrl)
        {
            try
            {
                // Extraire l'ID de session du checkout URL
                string sessionId = ExtractSessionIdFromUrl(checkoutUrl);
                if (string.IsNullOrEmpty(sessionId))
                {
                    Debug.WriteLine("Impossible d'extraire l'ID de session de l'URL");
                    return false;
                }

                // Créer un HttpClient pour vérifier le statut
                HttpClientHandler handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                };

                using (HttpClient client = new HttpClient(handler))
                {
                    // URL de l'API pour vérifier le statut
                    string apiUrl = $"https://localhost:7286/api/Stripe/check-payment-status?sessionId={sessionId}";
                    Debug.WriteLine($"Vérification du statut à l'URL: {apiUrl}");

                    // Envoyer la requête
                    HttpResponseMessage response = await client.GetAsync(apiUrl);
                    Debug.WriteLine($"Statut de la réponse: {response.StatusCode}");

                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        Debug.WriteLine($"Contenu de la réponse: {content}");

                        try
                        {
                            dynamic result = JsonConvert.DeserializeObject(content);
                            bool success = result?.success == true;
                            Debug.WriteLine($"Succès du paiement selon l'API: {success}");
                            return success;
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Erreur lors de la désérialisation: {ex.Message}");
                            // Si la désérialisation échoue mais que la réponse est OK, considérer comme un succès
                            return response.IsSuccessStatusCode;
                        }
                    }
                    else
                    {
                        // Si l'API renvoie une erreur, vérifier si c'est parce que le paiement est déjà traité
                        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                        {
                            // La session n'existe peut-être plus car elle a été traitée avec succès
                            Debug.WriteLine("Session non trouvée, possible que le paiement soit déjà traité");

                            // Vérifier l'URL actuelle du WebView2
                            string currentUrl = webView2?.Source?.ToString() ?? "";
                            if (currentUrl.Contains("success") || currentUrl.Contains("succeeded"))
                            {
                                Debug.WriteLine("URL actuelle indique un succès");
                                return true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception lors de la vérification du statut: {ex.Message}");
            }

            return false;
        }

        // Méthode pour extraire l'ID de session de l'URL (améliorée)
        private string ExtractSessionIdFromUrl(string url)
        {
            try
            {
                Debug.WriteLine($"Extraction de l'ID de session de l'URL: {url}");

                // Différents formats possibles pour l'ID de session
                string[] sessionParams = { "session_id=", "cs_", "checkout.session=" };

                foreach (string param in sessionParams)
                {
                    if (url.Contains(param))
                    {
                        int startIndex = url.IndexOf(param) + param.Length;
                        int endIndex = url.IndexOf("&", startIndex);

                        if (endIndex == -1) // Si c'est le dernier paramètre
                        {
                            string sessionId = url.Substring(startIndex);
                            Debug.WriteLine($"ID de session extrait: {sessionId}");
                            return sessionId;
                        }
                        else
                        {
                            string sessionId = url.Substring(startIndex, endIndex - startIndex);
                            Debug.WriteLine($"ID de session extrait: {sessionId}");
                            return sessionId;
                        }
                    }
                }

                // Si aucun format standard n'est trouvé, essayer d'extraire un identifiant qui ressemble à un ID de session
                if (url.Contains("stripe.com"))
                {
                    // Rechercher des motifs comme cs_test_xxx ou cs_live_xxx
                    int csIndex = url.IndexOf("cs_");
                    if (csIndex >= 0)
                    {
                        int endIndex = url.IndexOf("&", csIndex);
                        if (endIndex == -1)
                        {
                            string sessionId = url.Substring(csIndex);
                            Debug.WriteLine($"ID de session extrait (format alternatif): {sessionId}");
                            return sessionId;
                        }
                        else
                        {
                            string sessionId = url.Substring(csIndex, endIndex - csIndex);
                            Debug.WriteLine($"ID de session extrait (format alternatif): {sessionId}");
                            return sessionId;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erreur lors de l'extraction de l'ID de session: {ex.Message}");
            }

            Debug.WriteLine("Aucun ID de session trouvé dans l'URL");
            return string.Empty;
        }

        // Afficher une page HTML de succès
        private void ShowSuccessPage()
        {
            string html = @"
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset='UTF-8'>
                <title>Paiement réussi</title>
                <style>
                    body {
                        font-family: 'Segoe UI', Arial, sans-serif;
                        background-color: #f0f8f0;
                        color: #333;
                        text-align: center;
                        padding: 50px;
                        margin: 0;
                    }
                    .container {
                        max-width: 600px;
                        margin: 0 auto;
                        background-color: white;
                        border-radius: 10px;
                        padding: 30px;
                        box-shadow: 0 4px 8px rgba(0,0,0,0.1);
                    }
                    h1 {
                        color: #28a745;
                    }
                    .icon {
                        font-size: 72px;
                        color: #28a745;
                        margin-bottom: 20px;
                    }
                    .message {
                        font-size: 18px;
                        margin-bottom: 30px;
                    }
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='icon'>✓</div>
                    <h1>Paiement réussi !</h1>
                    <div class='message'>
                        Votre paiement a été traité avec succès. Votre réservation est maintenant confirmée.
                        <br><br>
                        Cette fenêtre se fermera automatiquement dans 2 secondes.
                    </div>
                </div>
                <script>
                    // Close automatically after 2 seconds
                    setTimeout(function() {
                        window.close();
                        // Send a message to parent to force closure
                        if (window.parent) {
                            window.parent.postMessage('payment_success', '*');
                        }
                    }, 2000);
                </script>
            </body>
            </html>";

            if (webView2 != null && webView2.CoreWebView2 != null)
            {
                webView2.CoreWebView2.NavigateToString(html);
            }
        }

        // Afficher une page HTML d'annulation
        private void ShowCancelPage()
        {
            string html = @"
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset='UTF-8'>
                <title>Paiement annulé</title>
                <style>
                    body {
                        font-family: 'Segoe UI', Arial, sans-serif;
                        background-color: #f8f0f0;
                        color: #333;
                        text-align: center;
                        padding: 50px;
                        margin: 0;
                    }
                    .container {
                        max-width: 600px;
                        margin: 0 auto;
                        background-color: white;
                        border-radius: 10px;
                        padding: 30px;
                        box-shadow: 0 4px 8px rgba(0,0,0,0.1);
                    }
                    h1 {
                        color: #dc3545;
                    }
                    .icon {
                        font-size: 72px;
                        color: #dc3545;
                        margin-bottom: 20px;
                    }
                    .message {
                        font-size: 18px;
                        margin-bottom: 30px;
                    }
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='icon'>✗</div>
                    <h1>Paiement annulé</h1>
                    <div class='message'>
                        Votre paiement a été annulé. Aucun montant n'a été débité.
                        <br><br>
                        Cette fenêtre se fermera automatiquement dans 2 secondes.
                    </div>
                </div>
                <script>
                    // Fermer automatiquement après 2 secondes
                    setTimeout(function() {
                        window.close();
                    }, 2000);
                </script>
            </body>
            </html>";

            if (webView2 != null && webView2.CoreWebView2 != null)
            {
                webView2.CoreWebView2.NavigateToString(html);
            }
        }

        // Afficher une page d'erreur personnalisée
        private void ShowCustomErrorPage()
        {
            string html = @"
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset='UTF-8'>
                <title>Traitement du paiement</title>
                <style>
                    body {
                        font-family: 'Segoe UI', Arial, sans-serif;
                        background-color: #f8f9fa;
                        color: #333;
                        text-align: center;
                        padding: 50px;
                        margin: 0;
                    }
                    .container {
                        max-width: 600px;
                        margin: 0 auto;
                        background-color: white;
                        border-radius: 10px;
                        padding: 30px;
                        box-shadow: 0 4px 8px rgba(0,0,0,0.1);
                    }
                    h1 {
                        color: #0066cc;
                    }
                    .icon {
                        font-size: 72px;
                        color: #0066cc;
                        margin-bottom: 20px;
                    }
                    .message {
                        font-size: 18px;
                        margin-bottom: 30px;
                    }
                    .spinner {
                        border: 5px solid #f3f3f3;
                        border-top: 5px solid #0066cc;
                        border-radius: 50%;
                        width: 50px;
                        height: 50px;
                        animation: spin 2s linear infinite;
                        margin: 0 auto 20px auto;
                    }
                    @keyframes spin {
                        0% { transform: rotate(0deg); }
                        100% { transform: rotate(360deg); }
                    }
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='spinner'></div>
                    <h1>Traitement en cours</h1>
                    <div class='message'>
                        Votre paiement est en cours de traitement. Veuillez ne pas fermer cette fenêtre.
                        <br><br>
                        La fenêtre se fermera automatiquement une fois le paiement traité.
                    </div>
                </div>
            </body>
            </html>";

            if (webView2 != null && webView2.CoreWebView2 != null)
            {
                webView2.CoreWebView2.NavigateToString(html);
            }
        }

        private bool AjouterReservationEnBaseDeDonnees(int volId, int userId)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    // Vérifier si l'utilisateur a déjà réservé ce vol
                    string checkQuery = "SELECT COUNT(*) FROM reservation WHERE id_vol = @idVol AND id_user = @idUser";
                    MySqlCommand checkCommand = new MySqlCommand(checkQuery, connection);
                    checkCommand.Parameters.AddWithValue("@idVol", volId);
                    checkCommand.Parameters.AddWithValue("@idUser", userId);

                    int existingReservations = Convert.ToInt32(checkCommand.ExecuteScalar());
                    if (existingReservations > 0)
                    {
                        MessageBox.Show("Vous avez déjà réservé une place pour ce vol.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return false;
                    }

                    // Insérer la nouvelle réservation
                    string insertQuery = "INSERT INTO reservation (id_vol, id_user, date_reservation, statut) VALUES (@idVol, @idUser, @dateReservation, @statut)";
                    MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection);
                    insertCommand.Parameters.AddWithValue("@idVol", volId);
                    insertCommand.Parameters.AddWithValue("@idUser", userId);
                    insertCommand.Parameters.AddWithValue("@dateReservation", DateTime.Now);
                    insertCommand.Parameters.AddWithValue("@statut", "Confirmée"); // Exemple : statut par défaut

                    int rowsAffected = insertCommand.ExecuteNonQuery();

                    // Mettre à jour la capacité disponible de l'avion
                    if (rowsAffected > 0)
                    {
                        // Récupérer l'ID de l'avion associé au vol
                        string avionQuery = "SELECT id_avion FROM vol WHERE id_vol = @idVol";
                        MySqlCommand avionCommand = new MySqlCommand(avionQuery, connection);
                        avionCommand.Parameters.AddWithValue("@idVol", volId);

                        object avionIdResult = avionCommand.ExecuteScalar();
                        if (avionIdResult != null && avionIdResult != DBNull.Value)
                        {
                            int avionId = Convert.ToInt32(avionIdResult);

                            // Mettre à jour la capacité de l'avion
                            string updateQuery = "UPDATE avion SET capacite = capacite - 1 WHERE id_avion = @idAvion";
                            MySqlCommand updateCommand = new MySqlCommand(updateQuery, connection);
                            updateCommand.Parameters.AddWithValue("@idAvion", avionId);
                            updateCommand.ExecuteNonQuery();

                            return true;
                        }
                        else
                        {
                            MessageBox.Show("Impossible de trouver l'avion associé à ce vol.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'ajout de la réservation : {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private bool DeleteVolFromDatabase(int volId)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "DELETE FROM vol WHERE id_vol = @id";
                    MySqlCommand command = new MySqlCommand(query, connection);

                    command.Parameters.AddWithValue("@id", volId);

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Vol supprimé avec succès.", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return true;
                    }
                    else
                    {
                        MessageBox.Show("Aucun vol n'a été supprimé.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la suppression du vol: {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            // Supprimer les vols sélectionnés
            List<int> selectedIds = new List<int>();

            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (row.Cells[0].Value != null && (bool)row.Cells[0].Value)
                {
                    int volId = Convert.ToInt32(row.Cells[1].Value);
                    selectedIds.Add(volId);
                }
            }

            if (selectedIds.Count > 0)
            {
                DialogResult result = MessageBox.Show($"Voulez-vous vraiment supprimer {selectedIds.Count} vol(s)?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    bool anyDeleted = false;

                    foreach (int id in selectedIds)
                    {
                        if (DeleteVolFromDatabase(id))
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
                MessageBox.Show("Veuillez sélectionner au moins un vol à supprimer.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnAddVol_Click(object sender, EventArgs e)
        {
            // Créer et afficher la fenêtre WPF
            AjoutVolWindow wpfWindow = new AjoutVolWindow();

            // Convertir le Handle de la fenêtre Windows Forms en WindowInteropHelper pour définir le propriétaire
            System.Windows.Interop.WindowInteropHelper helper = new System.Windows.Interop.WindowInteropHelper(wpfWindow);
            helper.Owner = this.Handle;

            // Afficher la fenêtre WPF comme dialogue modal
            bool? result = wpfWindow.ShowDialog();

            // Si l'utilisateur a cliqué sur OK (ajout réussi)
            if (result == true)
            {
                // Recharger les données
                LoadDataFromDatabase();
            }
        }

        // Gérer le redimensionnement du contrôle
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            // Repositionner les boutons dans le panel supérieur
            if (panel1 != null && btnDelete != null && btnAddVol != null)
            {
                btnDelete.Location = new Point(this.Width - 230, 15);
                btnAddVol.Location = new Point(this.Width - 120, 15);
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

            // Ajuster la largeur du DataGridView pour s'assurer que toutes les colonnes sont visibles
            if (dataGridView != null)
            {
                // Calculer la largeur totale nécessaire pour toutes les colonnes
                int totalWidth = 0;
                foreach (DataGridViewColumn col in dataGridView.Columns)
                {
                    totalWidth += col.Width;
                }

                // Si la largeur totale est supérieure à la largeur du contrôle, activer le défilement horizontal
                if (totalWidth > this.Width)
                {
                    dataGridView.ScrollBars = ScrollBars.Both;
                }
                else
                {
                    dataGridView.ScrollBars = ScrollBars.Vertical;
                }
            }
        }
    }
}