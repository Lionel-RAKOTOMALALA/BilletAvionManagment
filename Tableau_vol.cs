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

        private void ReserverVol(int volId)
        {
            // Vérifier si le vol existe dans la liste
            VolData vol = volsList.FirstOrDefault(v => v.Id == volId);
            if (vol != null)
            {
                // Vérifier s'il reste des places disponibles
                if (vol.AvionCapacite <= 0)
                {
                    MessageBox.Show("Désolé, il n'y a plus de places disponibles pour ce vol.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Demander confirmation
                DialogResult result = MessageBox.Show($"Voulez-vous réserver une place pour le vol de {vol.AeroportDepartVille} à {vol.AeroportArriveVille} le {vol.DateDepart.ToString("dd/MM/yyyy HH:mm")}?",
                    "Confirmation de réservation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // Appeler la méthode pour ajouter la réservation dans la base de données
                    if (AjouterReservationEnBaseDeDonnees(volId, UserId))
                    {
                        MessageBox.Show("Réservation effectuée avec succès !", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Recharger les données pour mettre à jour les places disponibles
                        LoadDataFromDatabase();
                    }
                }
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
