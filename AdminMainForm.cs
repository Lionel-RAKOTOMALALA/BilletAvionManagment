using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;
using System.IO;

namespace AvionManagment
{
    public partial class AdminMainForm : Form
    {
        // Champs pour stocker les données utilisateur
        private int id_user;
        private string username;
        private string role;

        // Référence aux panneaux que nous allons afficher
        private adminPanel adminPanelControl;
        private Tableau_aeroport tableauAeroportControl;
        private Tableau_avion tableauAvionControl;
        private Tableau_vol tableauVolControl;
        private Tableau_reservation tableauReservationControl;

        // Couleurs pour le thème moderne sombre
        private Color darkBackground = Color.FromArgb(18, 18, 30);      // Fond principal #12121E
        private Color darkSecondary = Color.FromArgb(30, 30, 45);       // Éléments secondaires #1E1E2D
        private Color accentPurple = Color.FromArgb(124, 58, 237);      // Accent principal #7C3AED
        private Color textColor = Color.White;                          // Texte principal
        private Color textColorDimmed = Color.FromArgb(180, 180, 180);  // Texte secondaire
        private Color borderColor = Color.FromArgb(40, 40, 60);         // Bordures #28283C

        // Dictionnaire pour stocker les icônes des boutons
        private Dictionary<Button, Image> buttonIcons = new Dictionary<Button, Image>();

        // Pour les animations fluides
        private Timer animationTimer = new Timer();
        private Button currentHoverButton = null;
        private float currentAlpha = 0.0f;
        private const float animationSpeed = 0.1f;
        private Button currentSelectedButton = null;

        // Constructeur modifié pour accepter les données utilisateur
        public AdminMainForm(int id_user, string username, string role)
        {
            // Stocker les données utilisateur dans des champs privés
            this.id_user = id_user;
            this.username = username;
            this.role = role;

            InitializeComponent();

            // Configurer le timer pour les animations
            animationTimer.Interval = 20;
            animationTimer.Tick += AnimationTimer_Tick;

            // Télécharger les icônes
            TelechargerIcones();

            // Configurer l'apparence de l'application
            ConfigurerApparence();

     

            // Ajouter le toggle de mode clair/sombre
            AjouterToggleModeNuit();

            // Configurer l'affichage des boutons du menu selon le rôle
            ConfigurerMenuSelonRole();

            // Ajouter des tooltips aux boutons
            AjouterTooltips();

            // Afficher les informations utilisateur dans l'interface
            AfficherInfosUtilisateurDansInterface();

            // Initialiser les contrôles
            InitializePanels();

            // Afficher le panneau Dashboard par défaut
            ShowDashboard();
        }

        private void TelechargerIcones()
        {
            try
            {
                // Créer des icônes modernes pour chaque bouton du menu
                Image dashboardIcon = CreerIconeModerne("dashboard");
                Image userIcon = CreerIconeModerne("users");
                Image airportIcon = CreerIconeModerne("airport");
                Image planeIcon = CreerIconeModerne("plane");
                Image flightIcon = CreerIconeModerne("flight");
                Image reservationIcon = CreerIconeModerne("reservation");
                Image paymentIcon = CreerIconeModerne("payment");
                Image ticketIcon = CreerIconeModerne("ticket");
                Image logoutIcon = CreerIconeModerne("logout");

                // Stocker les icônes dans le dictionnaire
                buttonIcons[dashboardBtn] = dashboardIcon;
                buttonIcons[UserMenuBtn] = userIcon;
                buttonIcons[AeroportMenuBtn] = airportIcon;
                buttonIcons[AvionMenuBtn] = planeIcon;
                buttonIcons[VolMenuBtn] = flightIcon;
                buttonIcons[ReservationMenuBtn] = reservationIcon;
                buttonIcons[PaiementMenuBtn] = paymentIcon;
                buttonIcons[BilletMenuBtn] = ticketIcon;
                buttonIcons[DeconnectionBtn] = logoutIcon;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du téléchargement des icônes: {ex.Message}");
            }
        }

        private Image CreerIconeModerne(string type)
        {
            // Créer une image pour l'icône avec un style moderne
            Bitmap bmp = new Bitmap(24, 24);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);
                using (Pen pen = new Pen(textColorDimmed, 1.5f))
                {
                    // Dessiner des icônes plus modernes et minimalistes
                    switch (type.ToLower())
                    {
                        case "dashboard":
                            // Icône de tableau de bord simplifiée
                            g.DrawRectangle(pen, 4, 4, 7, 7);
                            g.DrawRectangle(pen, 13, 4, 7, 7);
                            g.DrawRectangle(pen, 4, 13, 7, 7);
                            g.DrawRectangle(pen, 13, 13, 7, 7);
                            break;
                        case "users":
                            // Icône d'utilisateur simplifiée
                            g.DrawEllipse(pen, 8, 4, 8, 8);
                            g.DrawArc(pen, 4, 14, 16, 8, 0, 180);
                            break;
                        case "airport":
                            // Tour de contrôle simplifiée
                            g.DrawLine(pen, 8, 20, 16, 20);
                            g.DrawLine(pen, 12, 20, 12, 4);
                            g.DrawLine(pen, 8, 8, 16, 8);
                            g.DrawLine(pen, 10, 4, 14, 4);
                            break;
                        case "plane":
                            // Avion simplifié
                            g.DrawLine(pen, 4, 12, 20, 12);
                            g.DrawLine(pen, 12, 6, 12, 18);
                            g.DrawLine(pen, 8, 8, 16, 8);
                            g.DrawLine(pen, 8, 16, 16, 16);
                            break;
                        case "flight":
                            // Vol simplifié
                            g.DrawLine(pen, 4, 18, 20, 18);
                            g.DrawLine(pen, 12, 4, 12, 18);
                            g.DrawLine(pen, 8, 8, 16, 8);
                            break;
                        case "reservation":
                            // Calendrier simplifié
                            g.DrawRectangle(pen, 4, 6, 16, 14);
                            g.DrawLine(pen, 4, 10, 20, 10);
                            g.DrawLine(pen, 8, 6, 8, 4);
                            g.DrawLine(pen, 16, 6, 16, 4);
                            break;
                        case "payment":
                            // Carte de paiement simplifiée
                            g.DrawRectangle(pen, 4, 8, 16, 10);
                            g.DrawLine(pen, 4, 12, 20, 12);
                            g.DrawLine(pen, 16, 16, 18, 16);
                            break;
                        case "ticket":
                            // Billet simplifié
                            g.DrawRectangle(pen, 4, 6, 16, 12);
                            g.DrawLine(pen, 8, 6, 8, 18);
                            g.DrawLine(pen, 4, 10, 20, 10);
                            g.DrawLine(pen, 4, 14, 20, 14);
                            break;
                        case "logout":
                            // Déconnexion simplifiée
                            g.DrawArc(pen, 8, 4, 12, 12, -30, 210);
                            g.DrawLine(pen, 14, 10, 20, 10);
                            g.DrawLine(pen, 17, 7, 20, 10);
                            g.DrawLine(pen, 17, 13, 20, 10);
                            break;
                        default:
                            // Icône par défaut
                            g.DrawEllipse(pen, 4, 4, 16, 16);
                            break;
                    }
                }
            }
            return bmp;
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            if (currentHoverButton != null)
            {
                // Animation d'entrée
                currentAlpha += animationSpeed;
                if (currentAlpha >= 1.0f)
                {
                    currentAlpha = 1.0f;
                    animationTimer.Stop();
                }

                // Appliquer l'effet d'animation
                float factor = EaseInOut(currentAlpha);
                Color targetColor = Color.FromArgb(
                    (int)(darkBackground.R + (darkSecondary.R - darkBackground.R) * factor),
                    (int)(darkBackground.G + (darkSecondary.G - darkBackground.G) * factor),
                    (int)(darkBackground.B + (darkSecondary.B - darkBackground.B) * factor)
                );

                currentHoverButton.BackColor = targetColor;

                // Effet de déplacement subtil
                int padding = (int)(3 * factor);
                currentHoverButton.Padding = new Padding(20 + padding, 0, 0, 0);
            }
        }

        private float EaseInOut(float t)
        {
            // Fonction d'accélération/décélération pour des animations plus fluides
            return t < 0.5f ? 2 * t * t : -1 + (4 - 2 * t) * t;
        }

        private void ConfigurerApparence()
        {
            // Configurer l'apparence de la barre supérieure
            TopBar.BackColor = darkBackground;

            // Configurer l'apparence de la barre latérale
            Sidebar.BackColor = darkBackground;
            Sidebar.Width = 220; // Largeur fixe pour la sidebar

            // Configurer l'apparence des boutons du menu
            ConfigurerApparenceBoutons();

            // Configurer le titre de l'application
            if (this.Controls["lblTitre"] is Label lblTitre)
            {
                lblTitre.ForeColor = textColor;
                lblTitre.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            }

            // Configurer l'apparence du panneau de contenu
            contentPanel.BackColor = Color.White;
            contentPanel.BorderStyle = BorderStyle.None;

            // Arrondir les coins de la barre latérale
            ApplyRoundedCorners(Sidebar, 0); // Coins non arrondis pour un look plus moderne
        }

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect,
            int nTopRect,
            int nRightRect,
            int nBottomRect,
            int nWidthEllipse,
            int nHeightEllipse
        );

        private void ApplyRoundedCorners(Control control, int radius)
        {
            control.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, control.Width, control.Height, radius, radius));
        }

        private void ConfigurerApparenceBoutons()
        {
            // Configurer l'apparence des boutons du menu
            foreach (Control ctrl in Sidebar.Controls)
            {
                if (ctrl is Button btn)
                {
                    btn.BackColor = darkBackground;
                    btn.ForeColor = textColorDimmed;
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderSize = 0;
                    btn.Font = new Font("Segoe UI", 10, FontStyle.Regular);
                    btn.Height = 45; // Hauteur uniforme
                    btn.TextAlign = ContentAlignment.MiddleLeft;
                    btn.Padding = new Padding(20, 0, 0, 0);
                    btn.Cursor = Cursors.Hand;
                    btn.Width = Sidebar.Width; // Largeur complète
                    btn.Dock = DockStyle.None; // Désactiver le dock pour permettre le positionnement manuel

                    // Ajouter l'icône au bouton s'il en existe une
                    if (buttonIcons.ContainsKey(btn))
                    {
                        btn.Image = buttonIcons[btn];
                        btn.ImageAlign = ContentAlignment.MiddleLeft;
                        btn.TextAlign = ContentAlignment.MiddleLeft;
                        btn.TextImageRelation = TextImageRelation.ImageBeforeText;
                        btn.Padding = new Padding(20, 0, 0, 0);
                        btn.ImageList = new ImageList();
                        btn.ImageList.ImageSize = new Size(20, 20);
                    }

                    // Ajouter des effets de survol modernes
                    btn.MouseEnter += (s, e) => {
                        if (btn != currentSelectedButton) // Ne pas changer si déjà sélectionné
                        {
                            // Démarrer l'animation
                            currentHoverButton = btn;
                            currentAlpha = 0.0f;
                            animationTimer.Start();
                        }
                    };

                    btn.MouseLeave += (s, e) => {
                        if (btn != currentSelectedButton) // Ne pas changer si déjà sélectionné
                        {
                            btn.BackColor = darkBackground;
                            btn.ForeColor = textColorDimmed;
                            btn.Padding = new Padding(20, 0, 0, 0);
                            currentHoverButton = null;
                        }
                    };
                }
            }
        }


        private Image CreerIconeRecherche()
        {
            // Créer une icône de recherche
            Bitmap bmp = new Bitmap(16, 16);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);
                using (Pen pen = new Pen(textColorDimmed, 1.5f))
                {
                    // Dessiner une loupe
                    g.DrawEllipse(pen, 2, 2, 8, 8);
                    g.DrawLine(pen, 9, 9, 14, 14);
                }
            }
            return bmp;
        }

        private void AjouterToggleModeNuit()
        {
            // Créer un panneau pour le toggle de mode nuit
            Panel nightModePanel = new Panel();
            nightModePanel.Height = 40;
            nightModePanel.Width = Sidebar.Width;
            nightModePanel.BackColor = darkBackground;
            nightModePanel.Dock = DockStyle.Bottom;
            Sidebar.Controls.Add(nightModePanel);

            // Ajouter le texte "Light Mode"
            Label lblNightMode = new Label();
            lblNightMode.Text = "Light Mode";
            lblNightMode.ForeColor = textColorDimmed;
            lblNightMode.Font = new Font("Segoe UI", 10);
            lblNightMode.AutoSize = true;
            lblNightMode.Location = new Point(20, 10);
            nightModePanel.Controls.Add(lblNightMode);

            // Créer un toggle switch personnalisé
            PictureBox toggleSwitch = new PictureBox();
            toggleSwitch.Size = new Size(40, 20);
            toggleSwitch.Location = new Point(Sidebar.Width - 60, 10);
            toggleSwitch.BackColor = Color.FromArgb(30, 30, 45);
            toggleSwitch.Image = CreateToggleSwitchImage(false);
            toggleSwitch.SizeMode = PictureBoxSizeMode.StretchImage;
            toggleSwitch.Cursor = Cursors.Hand;

            // Arrondir les coins du toggle
            ApplyRoundedCorners(toggleSwitch, 10);

            nightModePanel.Controls.Add(toggleSwitch);

            // Ajouter l'événement de clic pour le toggle
            bool isLightMode = false;
            toggleSwitch.Click += (s, e) => {
                isLightMode = !isLightMode;
                toggleSwitch.Image = CreateToggleSwitchImage(isLightMode);
                // Ici, vous pouvez ajouter le code pour changer le thème de l'application
            };
        }

        private Image CreateToggleSwitchImage(bool isOn)
        {
            // Créer une image pour le toggle switch
            Bitmap bmp = new Bitmap(40, 20);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;

                // Dessiner le fond
                using (SolidBrush bgBrush = new SolidBrush(isOn ? accentPurple : Color.FromArgb(80, 80, 80)))
                {
                    DrawRoundedRectangle(g, new Rectangle(0, 0, 40, 20), 10, bgBrush);
                }

                // Dessiner le cercle
                using (SolidBrush circleBrush = new SolidBrush(Color.White))
                {
                    int circleX = isOn ? 22 : 2;
                    g.FillEllipse(circleBrush, circleX, 2, 16, 16);
                }
            }
            return bmp;
        }

        // Méthode pour dessiner un rectangle arrondi
        private void DrawRoundedRectangle(Graphics g, Rectangle bounds, int cornerRadius, Brush brush)
        {
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddArc(bounds.X, bounds.Y, cornerRadius * 2, cornerRadius * 2, 180, 90);
                path.AddArc(bounds.X + bounds.Width - cornerRadius * 2, bounds.Y, cornerRadius * 2, cornerRadius * 2, 270, 90);
                path.AddArc(bounds.X + bounds.Width - cornerRadius * 2, bounds.Y + bounds.Height - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 0, 90);
                path.AddArc(bounds.X, bounds.Y + bounds.Height - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 90, 90);
                path.CloseFigure();
                g.FillPath(brush, path);
            }
        }

        private void AjouterTooltips()
        {
            // Créer un contrôle ToolTip
            ToolTip toolTip = new ToolTip();
            toolTip.AutoPopDelay = 5000;
            toolTip.InitialDelay = 500;
            toolTip.ReshowDelay = 500;
            toolTip.ShowAlways = true;

            // Ajouter des tooltips aux boutons
            toolTip.SetToolTip(dashboardBtn, "Tableau de bord principal");
            toolTip.SetToolTip(UserMenuBtn, "Gestion des utilisateurs");
            toolTip.SetToolTip(AeroportMenuBtn, "Gestion des aéroports");
            toolTip.SetToolTip(AvionMenuBtn, "Gestion des avions");
            toolTip.SetToolTip(VolMenuBtn, "Gestion des vols");
            toolTip.SetToolTip(ReservationMenuBtn, "Gestion des réservations");
            toolTip.SetToolTip(PaiementMenuBtn, "Gestion des paiements");
            toolTip.SetToolTip(BilletMenuBtn, "Gestion des billets");
            toolTip.SetToolTip(DeconnectionBtn, "Se déconnecter de l'application");
        }

        private void AfficherInfosUtilisateurDansInterface()
        {
            // Créer un panneau pour afficher les informations utilisateur
            Panel userInfoPanel = new Panel();
            userInfoPanel.Dock = DockStyle.Top;
            userInfoPanel.Height = 80;
            userInfoPanel.BackColor = darkBackground;
            Sidebar.Controls.Add(userInfoPanel);
            Sidebar.Controls.SetChildIndex(userInfoPanel, 0); // Mettre en haut

            // Ajouter une image d'utilisateur (carrée avec coins arrondis)
            PictureBox userImage = new PictureBox();
            userImage.Size = new Size(40, 40);
            userImage.Location = new Point(15, 20);
            userImage.BackColor = accentPurple;
            userImage.Image = CreateDefaultUserImage();
            userImage.SizeMode = PictureBoxSizeMode.StretchImage;
            userInfoPanel.Controls.Add(userImage);

            // Rendre l'image avec coins arrondis
            ApplyRoundedCorners(userImage, 8);

            // Ajouter le nom d'utilisateur
            Label lblUsername = new Label();
            lblUsername.Text = username;
            lblUsername.ForeColor = Color.White;
            lblUsername.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblUsername.AutoSize = true;
            lblUsername.Location = new Point(65, 20);
            userInfoPanel.Controls.Add(lblUsername);

            // Ajouter le rôle
            Label lblRole = new Label();
            lblRole.Text = role.ToUpper();
            lblRole.ForeColor = textColorDimmed;
            lblRole.Font = new Font("Segoe UI", 8, FontStyle.Regular);
            lblRole.AutoSize = true;
            lblRole.Location = new Point(65, 40);
            userInfoPanel.Controls.Add(lblRole);
        }

        private Image CreateDefaultUserImage()
        {
            // Créer une image par défaut pour l'utilisateur
            Bitmap bmp = new Bitmap(40, 40);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(accentPurple);
                using (Font font = new Font("Segoe UI", 18, FontStyle.Bold))
                using (Brush brush = new SolidBrush(Color.White))
                {
                    string initial = username.Length > 0 ? username.Substring(0, 1).ToUpper() : "C";
                    SizeF size = g.MeasureString(initial, font);
                    g.DrawString(initial, font, brush,
                        (bmp.Width - size.Width) / 2,
                        (bmp.Height - size.Height) / 2);
                }
            }
            return bmp;
        }

        private void ConfigurerMenuSelonRole()
        {
            // Vérifier si l'utilisateur est un administrateur
            bool isAdmin = role != null && role.ToLower() == "admin";

            // Configurer les boutons visibles selon le rôle
            if (isAdmin)
            {
                // Pour les administrateurs, afficher tous les boutons de gestion
                dashboardBtn.Visible = true;
                UserMenuBtn.Visible = true;
                AeroportMenuBtn.Visible = true;
                AvionMenuBtn.Visible = true;
                VolMenuBtn.Visible = true;
                ReservationMenuBtn.Visible = true;
                PaiementMenuBtn.Visible = true;
                BilletMenuBtn.Visible = true;
            }
            else
            {
                // Pour les utilisateurs simples, afficher uniquement les boutons pertinents
                dashboardBtn.Visible = true;
                UserMenuBtn.Visible = false;
                AeroportMenuBtn.Visible = false;
                AvionMenuBtn.Visible = false;
                VolMenuBtn.Visible = true;
                ReservationMenuBtn.Visible = true;
                PaiementMenuBtn.Visible = true;
                BilletMenuBtn.Visible = true;

            }

            // Le bouton de déconnexion est toujours visible
            DeconnectionBtn.Visible = true;

            // Réorganiser les boutons pour éviter les espaces vides
            ReorganiserBoutons();
        }

        private void ReorganiserBoutons()
        {
            // Liste pour stocker les boutons visibles (sauf Dashboard et Déconnexion)
            List<Button> boutonsVisibles = new List<Button>();

            // Collecter tous les boutons visibles (sauf Dashboard et Déconnexion)
            foreach (Control ctrl in Sidebar.Controls)
            {
                if (ctrl is Button btn && btn.Visible && btn != dashboardBtn && btn != DeconnectionBtn)
                {
                    boutonsVisibles.Add(btn);
                }
            }

            // Trier les boutons par leur position Y d'origine pour maintenir l'ordre
            boutonsVisibles.Sort((a, b) => a.TabIndex.CompareTo(b.TabIndex));

            // Positionner le bouton Dashboard en haut (juste après la barre de recherche)
            if (dashboardBtn != null && dashboardBtn.Visible)
            {
                dashboardBtn.Location = new Point(0, 140);
            }

            // Repositionner les autres boutons sans espaces
            int startY = 185; // Position Y de départ après le Dashboard
            int buttonHeight = 45; // Hauteur de chaque bouton
            int spacing = 0; // Espacement minimal entre les boutons

            foreach (Button btn in boutonsVisibles)
            {
                btn.Location = new Point(0, startY);
                startY += buttonHeight + spacing;
            }

            // Positionner le bouton de déconnexion en bas
            if (DeconnectionBtn != null && DeconnectionBtn.Visible)
            {
                // Calculer la position Y pour le bouton de déconnexion
                // (juste au-dessus du toggle de mode nuit)
                int disconnectY = Sidebar.Height - DeconnectionBtn.Height - 50;
                DeconnectionBtn.Location = new Point(0, disconnectY);
            }
        }

        private void InitializePanels()
        {
            // Créer les instances des contrôles
            adminPanelControl = new adminPanel();
            tableauAeroportControl = new Tableau_aeroport();
            tableauAvionControl = new Tableau_avion();
            tableauVolControl = new Tableau_vol(id_user, username, role);
            tableauReservationControl = new Tableau_reservation();

            // Configurer les contrôles pour qu'ils remplissent le contentPanel
            adminPanelControl.Dock = DockStyle.Fill;
            tableauAeroportControl.Dock = DockStyle.Fill;
            tableauAvionControl.Dock = DockStyle.Fill;
            tableauVolControl.Dock = DockStyle.Fill;
            tableauReservationControl.Dock = DockStyle.Fill;

            // Ne pas les ajouter tout de suite au contentPanel
        }

        private void ClearContentPanel()
        {
            // Supprimer tous les contrôles du contentPanel
            contentPanel.Controls.Clear();
        }

        private void ShowDashboard()
        {
            // Mettre en évidence le bouton sélectionné
            ResetButtonColors();
            dashboardBtn.BackColor = darkSecondary;
            dashboardBtn.ForeColor = textColor;
            currentSelectedButton = dashboardBtn;

            // Afficher le panneau d'administration
            ClearContentPanel();
            contentPanel.Controls.Add(adminPanelControl);
            adminPanelControl.BringToFront();

            // Mettre à jour le titre de la page
            UpdatePageTitle("Tableau de bord");
        }

        private void ShowReservationPanel()
        {
            // Mettre en évidence le bouton sélectionné
            ResetButtonColors();
            AvionMenuBtn.BackColor = darkSecondary;
            AvionMenuBtn.ForeColor = textColor;
            currentSelectedButton = ReservationMenuBtn;

            // Afficher le tableau des avions
            ClearContentPanel();
            contentPanel.Controls.Add(tableauReservationControl);
            tableauReservationControl.BringToFront();

            // Mettre à jour le titre de la page
            UpdatePageTitle("Gestion de réservations");
        }

        private void ShowAvionPanel()
        {
            // Mettre en évidence le bouton sélectionné
            ResetButtonColors();
            AvionMenuBtn.BackColor = darkSecondary;
            AvionMenuBtn.ForeColor = textColor;
            currentSelectedButton = AvionMenuBtn;

            // Afficher le tableau des avions
            ClearContentPanel();
            contentPanel.Controls.Add(tableauAvionControl);
            tableauAvionControl.BringToFront();

            // Mettre à jour le titre de la page
            UpdatePageTitle("Gestion des avions");
        }

        private void ShowAeroportPanel()
        {
            // Mettre en évidence le bouton sélectionné
            ResetButtonColors();
            AeroportMenuBtn.BackColor = darkSecondary;
            AeroportMenuBtn.ForeColor = textColor;
            currentSelectedButton = AeroportMenuBtn;

            // Afficher le tableau des aéroports
            ClearContentPanel();
            contentPanel.Controls.Add(tableauAeroportControl);
            tableauAeroportControl.BringToFront();

            // Mettre à jour le titre de la page
            UpdatePageTitle("Gestion des aéroports");
        }

        private void ShowVolPanel()
        {
            // Mettre en évidence le bouton sélectionné
            ResetButtonColors();
            VolMenuBtn.BackColor = darkSecondary;
            VolMenuBtn.ForeColor = textColor;
            currentSelectedButton = VolMenuBtn;

            // Afficher le tableau des vols avec les informations utilisateur
            ClearContentPanel();
            tableauVolControl = new Tableau_vol(id_user, username, role); // Passer les informations utilisateur
            tableauVolControl.Dock = DockStyle.Fill;
            contentPanel.Controls.Add(tableauVolControl);
            tableauVolControl.BringToFront();

            // Mettre à jour le titre de la page
            UpdatePageTitle("Gestion des vols");
        }

        private void UpdatePageTitle(string title)
        {
            // Mettre à jour le titre de la page si le contrôle existe
            if (this.Controls["lblPageTitle"] is Label lblPageTitle)
            {
                lblPageTitle.Text = title;
            }
        }

        private void ResetButtonColors()
        {
            // Réinitialiser la couleur de tous les boutons du menu
            foreach (Control ctrl in Sidebar.Controls)
            {
                if (ctrl is Button btn)
                {
                    btn.BackColor = darkBackground;
                    btn.ForeColor = textColorDimmed;
                    btn.Padding = new Padding(20, 0, 0, 0);
                }
            }
            currentSelectedButton = null;
        }

        private void fermer_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void AfficherInformationsUtilisateur()
        {
            // Afficher les infos dans un MessageBox
            MessageBox.Show($"ID: {id_user}\nNom d'utilisateur: {username}\nRôle: {role}", "Informations Utilisateur", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void DeconnectionBtn_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Voulez-vous vraiment vous déconnecter?", "Déconnexion", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                this.Hide();
                Form1 loginForm = new Form1();
                loginForm.Show();
            }
        }

        // Gestionnaires d'événements des boutons du menu
        private void dashboardBtn_Click(object sender, EventArgs e)
        {
            ShowDashboard();
        }

        private void AvionMenuBtn_Click(object sender, EventArgs e)
        {
            ShowAvionPanel();
        }

        private void AeroportMenuBtn_Click(object sender, EventArgs e)
        {
            ShowAeroportPanel();
        }

        private void VolMenuBtn_Click(object sender, EventArgs e)
        {
            ShowVolPanel();
        }

        private void ReservationMenuBtn_Click(object sender, EventArgs e)
        {
            // Mettre en évidence le bouton sélectionné
            ResetButtonColors();
            ReservationMenuBtn.BackColor = darkSecondary;
            ReservationMenuBtn.ForeColor = textColor;
            currentSelectedButton = ReservationMenuBtn;

            // Mettre à jour le titre de la page
            UpdatePageTitle("Gestion des réservations");

            // Ici, vous pouvez ajouter le code pour afficher le panneau de réservation
            // Exemple: ShowReservationPanel();
        }

        private void PaiementMenuBtn_Click(object sender, EventArgs e)
        {
            // Mettre en évidence le bouton sélectionné
            ResetButtonColors();
            PaiementMenuBtn.BackColor = darkSecondary;
            PaiementMenuBtn.ForeColor = textColor;
            currentSelectedButton = PaiementMenuBtn;

            // Mettre à jour le titre de la page
            UpdatePageTitle("Gestion des paiements");

            // Ici, vous pouvez ajouter le code pour afficher le panneau de paiement
            // Exemple: ShowPaiementPanel();
        }

        private void BilletMenuBtn_Click(object sender, EventArgs e)
        {
            // Mettre en évidence le bouton sélectionné
            ResetButtonColors();
            BilletMenuBtn.BackColor = darkSecondary;
            BilletMenuBtn.ForeColor = textColor;
            currentSelectedButton = BilletMenuBtn;

            // Mettre à jour le titre de la page
            UpdatePageTitle("Gestion des billets");

            // Ici, vous pouvez ajouter le code pour afficher le panneau des billets
            // Exemple: ShowBilletPanel();
        }

        private void UserMenuBtn_Click(object sender, EventArgs e)
        {
            // Mettre en évidence le bouton sélectionné
            ResetButtonColors();
            UserMenuBtn.BackColor = darkSecondary;
            UserMenuBtn.ForeColor = textColor;
            currentSelectedButton = UserMenuBtn;

            // Mettre à jour le titre de la page
            UpdatePageTitle("Gestion des utilisateurs");

            // Ici, vous pouvez ajouter le code pour afficher le panneau de gestion des utilisateurs
            // Exemple: ShowUserPanel();
        }

        // Méthode pour gérer le redimensionnement du formulaire
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            // Réorganiser les boutons lors du redimensionnement
            ReorganiserBoutons();
        }

        private void dashboardBtn_Click_1(object sender, EventArgs e)
        {
            ShowDashboard();
        }

        private void UserMenuBtn_Click_1(object sender, EventArgs e)
        {
            ShowDashboard();
        }

        private void AeroportMenuBtn_Click_1(object sender, EventArgs e)
        {
            ShowAeroportPanel();
        }

        private void ReservationMenuBtn_Click_1(object sender, EventArgs e)
        {
            ShowReservationPanel();
        }
    }
}   