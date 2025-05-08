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
using MySql.Data.MySqlClient;

namespace AvionManagment
{
    public partial class CardVolDispo : UserControl
    {
        // Propriétés pour stocker les informations du vol - rendues publiques pour l'accès externe
        public int IdVol { get; private set; }
        public DateTime DateDepart { get; private set; }
        public DateTime DateArrive { get; private set; }
        public string AvionModele { get; private set; }
        public int AvionCapacite { get; private set; }
        public string AeroportDepartVille { get; private set; }
        public string AeroportArriveVille { get; private set; }
        public int Prix { get; private set; } = 150; // Prix par défaut

        // Événements
        public event EventHandler ReserverClick;
        public event EventHandler DetailsClick;

        // Contrôles UI
        private Panel pnlHeader;
        private Panel pnlBody;
        private PictureBox picAvion;
        private Label lblTrajet;
        private Label lblDetails;
        private Label lblCapacite;
        private Label lblDuree;
        private Button btnReserver;
        private Button btnDetails;
        private Panel pnlDots;

        // Couleurs disponibles pour les en-têtes
        private static Color[] headerColors = new Color[]
        {
            Color.FromArgb(59, 130, 246),  // Bleu
            Color.FromArgb(220, 38, 38),   // Rouge
            Color.FromArgb(16, 185, 129),  // Vert
            Color.FromArgb(139, 92, 246),  // Violet
            Color.FromArgb(217, 119, 6)    // Or/Jaune
        };

        // Index de couleur actuel
        private int colorIndex = 0;

        public CardVolDispo(int colorIdx = 0)
        {
            InitializeComponent();

            // Définir l'index de couleur
            colorIndex = colorIdx % headerColors.Length;

            // Définir les propriétés de base du contrôle
            this.Size = new Size(220, 300);
            this.Margin = new Padding(10);
            this.BackColor = Color.White;
            this.Padding = new Padding(0);

            // Créer les contrôles
            SetupControls();
        }

        private void SetupControls()
        {
            // Panneau principal avec ombre et coins arrondis
            this.Paint += (s, e) =>
            {
                // Dessiner l'ombre
                Rectangle rect = new Rectangle(0, 0, this.Width, this.Height);
                using (GraphicsPath path = RoundedRectangle(rect, 10))
                {
                    using (SolidBrush shadowBrush = new SolidBrush(Color.FromArgb(20, 0, 0, 0)))
                    {
                        for (int i = 1; i <= 5; i++)
                        {
                            Rectangle shadowRect = new Rectangle(i, i, rect.Width, rect.Height);
                            e.Graphics.FillPath(shadowBrush, RoundedRectangle(shadowRect, 10));
                        }
                    }

                    // Dessiner le fond blanc
                    using (SolidBrush brush = new SolidBrush(Color.White))
                    {
                        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                        e.Graphics.FillPath(brush, path);
                    }

                    // Dessiner la bordure
                    using (Pen pen = new Pen(Color.FromArgb(229, 231, 235), 1))
                    {
                        e.Graphics.DrawPath(pen, path);
                    }
                }
            };

            // En-tête coloré
            pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = headerColors[colorIndex]
            };
            this.Controls.Add(pnlHeader);

            // Corps de la carte
            pnlBody = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(15, 40, 15, 15)
            };
            this.Controls.Add(pnlBody);

            // Image d'avion (cercle qui chevauche l'en-tête et le corps)
            picAvion = new PictureBox
            {
                Size = new Size(60, 60),
                Location = new Point((this.Width - 60) / 2, pnlHeader.Height - 30),
                BackColor = Color.White
            };
            this.Controls.Add(picAvion);
            picAvion.BringToFront();

            // Dessiner l'image d'avion circulaire
            picAvion.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                // Dessiner le cercle blanc
                using (SolidBrush brush = new SolidBrush(Color.White))
                {
                    e.Graphics.FillEllipse(brush, 0, 0, picAvion.Width, picAvion.Height);
                }

                // Dessiner la bordure
                using (Pen pen = new Pen(Color.FromArgb(229, 231, 235), 1))
                {
                    e.Graphics.DrawEllipse(pen, 0, 0, picAvion.Width - 1, picAvion.Height - 1);
                }

                // Dessiner l'icône d'avion
                using (Pen pen = new Pen(headerColors[colorIndex], 2))
                {
                    int centerX = picAvion.Width / 2;
                    int centerY = picAvion.Height / 2;

                    // Corps de l'avion
                    e.Graphics.DrawLine(pen, centerX - 15, centerY, centerX + 15, centerY);

                    // Ailes
                    e.Graphics.DrawLine(pen, centerX, centerY - 12, centerX, centerY + 12);

                    // Queue
                    e.Graphics.DrawLine(pen, centerX - 15, centerY - 8, centerX - 8, centerY);
                    e.Graphics.DrawLine(pen, centerX - 15, centerY + 8, centerX - 8, centerY);
                }
            };

            // Trajet (titre)
            lblTrajet = new Label
            {
                Text = "Paris → New York",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(31, 41, 55), // Gris foncé
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = false,
                Dock = DockStyle.Top,
                Height = 25
            };
            pnlBody.Controls.Add(lblTrajet);

            // Détails du vol (sous-titre)
            lblDetails = new Label
            {
                Text = "Vol #123 • Boeing 737",
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.FromArgb(107, 114, 128), // Gris moyen
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = false,
                Dock = DockStyle.Top,
                Height = 20
            };
            pnlBody.Controls.Add(lblDetails);

            // Panneau pour les statistiques
            Panel pnlStats = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                Margin = new Padding(0, 15, 0, 15),
                BackColor = Color.Transparent
            };
            pnlBody.Controls.Add(pnlStats);

            // Statistiques (divisées en deux colonnes)
            TableLayoutPanel tblStats = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2,
                BackColor = Color.Transparent
            };
            tblStats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tblStats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tblStats.RowStyles.Add(new RowStyle(SizeType.Percent, 60F));
            tblStats.RowStyles.Add(new RowStyle(SizeType.Percent, 40F));
            pnlStats.Controls.Add(tblStats);

            // Capacité (nombre de places)
            lblCapacite = new Label
            {
                Text = "180",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(31, 41, 55),
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.BottomCenter,
                Dock = DockStyle.Fill
            };
            tblStats.Controls.Add(lblCapacite, 0, 0);

            // Durée
            lblDuree = new Label
            {
                Text = "2h 30min",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(31, 41, 55),
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.BottomCenter,
                Dock = DockStyle.Fill
            };
            tblStats.Controls.Add(lblDuree, 1, 0);

            // Libellé "Places"
            Label lblCapaciteCaption = new Label
            {
                Text = "Places",
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.FromArgb(107, 114, 128),
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.TopCenter,
                Dock = DockStyle.Fill
            };
            tblStats.Controls.Add(lblCapaciteCaption, 0, 1);

            // Libellé "Durée"
            Label lblDureeCaption = new Label
            {
                Text = "Durée",
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.FromArgb(107, 114, 128),
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.TopCenter,
                Dock = DockStyle.Fill
            };
            tblStats.Controls.Add(lblDureeCaption, 1, 1);

            // Panneau pour les boutons
            Panel pnlButtons = new Panel
            {
                Dock = DockStyle.Top,
                Height = 40,
                BackColor = Color.Transparent
            };
            pnlBody.Controls.Add(pnlButtons);

            // Bouton "Réserver"
            btnReserver = new Button
            {
                Text = "Réserver",
                FlatStyle = FlatStyle.Flat,
                BackColor = headerColors[colorIndex],
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Size = new Size(80, 30),
                Location = new Point(0, 5),
                Cursor = Cursors.Hand
            };
            btnReserver.FlatAppearance.BorderSize = 0;
            btnReserver.Click += (s, e) => ReserverClick?.Invoke(this, EventArgs.Empty);
            pnlButtons.Controls.Add(btnReserver);

            // Bouton "Détails"
            btnDetails = new Button
            {
                Text = "Détails",
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = headerColors[colorIndex],
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Size = new Size(80, 30),
                Location = new Point(btnReserver.Right + 10, 5),
                Cursor = Cursors.Hand
            };
            btnDetails.FlatAppearance.BorderColor = headerColors[colorIndex];
            btnDetails.FlatAppearance.BorderSize = 1;
            btnDetails.Click += (s, e) => DetailsClick?.Invoke(this, EventArgs.Empty);
            pnlButtons.Controls.Add(btnDetails);

            // Panneau pour les points colorés
            pnlDots = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 20,
                BackColor = Color.Transparent
            };
            pnlBody.Controls.Add(pnlDots);

            // Ajouter les points colorés
            int dotSize = 8;
            int spacing = 5;
            int totalWidth = (dotSize * 5) + (spacing * 4);
            int startX = (pnlDots.Width - totalWidth) / 2;

            for (int i = 0; i < 5; i++)
            {
                Panel dot = new Panel
                {
                    Size = new Size(dotSize, dotSize),
                    Location = new Point(startX + (i * (dotSize + spacing)), (pnlDots.Height - dotSize) / 2),
                    BackColor = headerColors[i]
                };

                dot.Paint += (s, e) =>
                {
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    using (SolidBrush brush = new SolidBrush(((Panel)s).BackColor))
                    {
                        e.Graphics.FillEllipse(brush, 0, 0, dotSize, dotSize);
                    }
                };

                pnlDots.Controls.Add(dot);
            }

            // Ajuster les contrôles lors du redimensionnement
            this.Resize += (s, e) =>
            {
                picAvion.Location = new Point((this.Width - 60) / 2, pnlHeader.Height - 30);

                // Repositionner les boutons
                int buttonsTotalWidth = btnReserver.Width + btnDetails.Width + 10;
                int buttonsStartX = (pnlButtons.Width - buttonsTotalWidth) / 2;
                btnReserver.Location = new Point(buttonsStartX, 5);
                btnDetails.Location = new Point(btnReserver.Right + 10, 5);

                // Repositionner les points
                int dotsStartX = (pnlDots.Width - totalWidth) / 2;
                for (int i = 0; i < pnlDots.Controls.Count; i++)
                {
                    pnlDots.Controls[i].Location = new Point(dotsStartX + (i * (dotSize + spacing)), (pnlDots.Height - dotSize) / 2);
                }
            };
        }

        // Méthode pour créer un rectangle arrondi
        private GraphicsPath RoundedRectangle(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();

            // Coins arrondis
            path.AddArc(rect.X, rect.Y, radius * 2, radius * 2, 180, 90);
            path.AddArc(rect.Width - radius * 2, rect.Y, radius * 2, radius * 2, 270, 90);
            path.AddArc(rect.Width - radius * 2, rect.Height - radius * 2, radius * 2, radius * 2, 0, 90);
            path.AddArc(rect.X, rect.Height - radius * 2, radius * 2, radius * 2, 90, 90);

            path.CloseAllFigures();
            return path;
        }

        // Méthode pour charger les données d'un vol
        public void LoadVolData(int idVol, DateTime dateDepart, DateTime dateArrive,
                               string avionModele, int avionCapacite,
                               string aeroportDepartVille, string aeroportArriveVille)
        {
            // Stocker les données dans les propriétés publiques
            IdVol = idVol;
            DateDepart = dateDepart;
            DateArrive = dateArrive;
            AvionModele = avionModele;
            AvionCapacite = avionCapacite;
            AeroportDepartVille = aeroportDepartVille;
            AeroportArriveVille = aeroportArriveVille;

            // Mettre à jour l'interface utilisateur
            lblTrajet.Text = $"{aeroportDepartVille} → {aeroportArriveVille}";
            lblDetails.Text = $"Vol #{idVol} • {avionModele}";

            // Calculer la durée du vol
            TimeSpan duree = dateArrive - dateDepart;
            lblDuree.Text = $"{(int)duree.TotalHours}h {duree.Minutes}min";

            // Afficher la capacité
            lblCapacite.Text = avionCapacite.ToString();

            // Calculer un prix fictif basé sur la durée du vol
            Prix = 100 + (int)(duree.TotalHours * 50);

            // Ajouter un tooltip pour afficher plus d'informations
            ToolTip tooltip = new ToolTip();
            tooltip.SetToolTip(this, $"Vol #{idVol}\nDe: {aeroportDepartVille}\nVers: {aeroportArriveVille}\nDépart: {dateDepart:dd/MM/yyyy HH:mm}\nArrivée: {dateArrive:dd/MM/yyyy HH:mm}\nPrix: {Prix} €");
        }
    }

    // Classe pour gérer l'affichage des vols disponibles
    public class VolsDisponiblesManager
    {
        private readonly string connectionString = "Server=localhost;Database=avionDb;Uid=root;Pwd=;";
        private FlowLayoutPanel flowLayoutPanel;

        public VolsDisponiblesManager(FlowLayoutPanel container)
        {
            flowLayoutPanel = container;
            flowLayoutPanel.AutoScroll = true;
            flowLayoutPanel.FlowDirection = FlowDirection.LeftToRight;
            flowLayoutPanel.WrapContents = true;
            flowLayoutPanel.Padding = new Padding(20);
        }

        public void ChargerVolsDisponibles()
        {
            try
            {
                // Effacer les cartes existantes
                flowLayoutPanel.Controls.Clear();

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

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

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        int colorIndex = 0;

                        while (reader.Read())
                        {
                            // Créer une nouvelle carte pour chaque vol
                            CardVolDispo card = new CardVolDispo(colorIndex);
                            colorIndex = (colorIndex + 1) % 5; // Alterner entre les 5 couleurs

                            // Charger les données du vol
                            card.LoadVolData(
                                Convert.ToInt32(reader["id_vol"]),
                                Convert.ToDateTime(reader["date_depart"]),
                                Convert.ToDateTime(reader["date_arrive"]),
                                reader["avion_modele"].ToString(),
                                Convert.ToInt32(reader["avion_capacite"]),
                                reader["aeroport_depart_ville"].ToString(),
                                reader["aeroport_arrive_ville"].ToString()
                            );

                            // Ajouter des gestionnaires d'événements pour les boutons
                            card.ReserverClick += (sender, e) =>
                            {
                                CardVolDispo selectedCard = (CardVolDispo)sender;
                                MessageBox.Show($"Réservation du vol #{selectedCard.IdVol} de {selectedCard.AeroportDepartVille} à {selectedCard.AeroportArriveVille} pour {selectedCard.Prix} €",
                                    "Réservation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            };

                            card.DetailsClick += (sender, e) =>
                            {
                                CardVolDispo selectedCard = (CardVolDispo)sender;
                                MessageBox.Show($"Détails du vol #{selectedCard.IdVol}\nDe: {selectedCard.AeroportDepartVille}\nVers: {selectedCard.AeroportArriveVille}\nDépart: {selectedCard.DateDepart:dd/MM/yyyy HH:mm}\nArrivée: {selectedCard.DateArrive:dd/MM/yyyy HH:mm}\nAvion: {selectedCard.AvionModele}\nCapacité: {selectedCard.AvionCapacite} places\nPrix: {selectedCard.Prix} €",
                                    "Détails du vol", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            };

                            // Ajouter la carte au conteneur
                            flowLayoutPanel.Controls.Add(card);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des vols disponibles: {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    // Formulaire pour afficher les vols disponibles
    public class FormVolsDisponibles : Form
    {
        private VolsDisponiblesManager volsManager;

        public FormVolsDisponibles()
        {
            // Configurer le formulaire
            this.Text = "Vols Disponibles";
            this.Size = new System.Drawing.Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Créer le FlowLayoutPanel pour les cartes de vol
            FlowLayoutPanel flpVols = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                Padding = new Padding(20),
                BackColor = System.Drawing.Color.FromArgb(249, 250, 251) // Fond gris très clair
            };
            this.Controls.Add(flpVols);

            // Ajouter un titre
            Label lblTitle = new Label
            {
                Text = "Vols Disponibles",
                Font = new System.Drawing.Font("Segoe UI", 24, System.Drawing.FontStyle.Bold),
                ForeColor = System.Drawing.Color.FromArgb(31, 41, 55),
                Dock = DockStyle.Top,
                Height = 80,
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(lblTitle);
            lblTitle.BringToFront();

            // Initialiser le gestionnaire de vols
            volsManager = new VolsDisponiblesManager(flpVols);

            // Charger les vols disponibles lors du chargement du formulaire
            this.Load += (s, e) => volsManager.ChargerVolsDisponibles();
        }
    }
}
