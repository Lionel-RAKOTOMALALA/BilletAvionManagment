using System;
using System.Drawing;
using System.Windows.Forms;

namespace AvionManagment
{
    public partial class AdminMainForm : Form
    {
        // Référence aux panneaux que nous allons afficher
        private adminPanel adminPanelControl;
        private Tableau_aeroport tableauAeroportControl;
        private Tableau_avion tableauAvionControl;

        public AdminMainForm()
        {
            InitializeComponent();
            TopBar.BackColor = Color.FromArgb(3, 14, 28);
            Sidebar.BackColor = Color.FromArgb(0, 16, 36);
            dashboardBtn.BackColor = Color.FromArgb(0, 16, 36);

            // Initialiser les contrôles
            InitializePanels();

            // Afficher le panneau Dashboard par défaut
            ShowAvionPanel();
        }

        private void InitializePanels()
        {
            // Créer les instances des contrôles
            adminPanelControl = new adminPanel();
            tableauAeroportControl = new Tableau_aeroport();
            tableauAvionControl = new Tableau_avion();

            // Configurer les contrôles pour qu'ils remplissent le contentPanel
            adminPanelControl.Dock = DockStyle.Fill;
            tableauAeroportControl.Dock = DockStyle.Fill;
            tableauAvionControl.Dock = DockStyle.Fill;  

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
            dashboardBtn.BackColor = Color.FromArgb(0, 26, 56); // Couleur plus foncée pour indiquer la sélection

            // Afficher le panneau d'administration
            ClearContentPanel();
            contentPanel.Controls.Add(adminPanelControl);
            adminPanelControl.BringToFront();
        }

        private void ShowUserPanel()
        {
            // Mettre en évidence le bouton sélectionné
            ResetButtonColors();
            UserMenuBtn.BackColor = Color.FromArgb(0, 26, 56); // Couleur plus foncée pour indiquer la sélection

            // Afficher le panneau d'administration
            ClearContentPanel();
            contentPanel.Controls.Add(adminPanelControl);
            adminPanelControl.BringToFront();
        }
        private void ShowAvionPanel()
        {
            // Mettre en évidence le bouton sélectionné
            ResetButtonColors();
            AvionMenuBtn.BackColor = Color.FromArgb(0, 26, 56); // Couleur plus foncée pour indiquer la sélection

            // Afficher le tableau des aéroports
            ClearContentPanel();
            contentPanel.Controls.Add(tableauAvionControl);
            tableauAvionControl.BringToFront();
        }
        private void ShowAeroportPanel()
        {
            // Mettre en évidence le bouton sélectionné
            ResetButtonColors();
            AeroportMenuBtn.BackColor = Color.FromArgb(0, 26, 56); // Couleur plus foncée pour indiquer la sélection

            // Afficher le tableau des aéroports
            ClearContentPanel();
            contentPanel.Controls.Add(tableauAeroportControl);
            tableauAeroportControl.BringToFront();
        }


        private void ResetButtonColors()
        {
            // Réinitialiser la couleur de tous les boutons du menu
            dashboardBtn.BackColor = Color.FromArgb(0, 16, 36);
            UserMenuBtn.BackColor = Color.FromArgb(0, 16, 36);
            AeroportMenuBtn.BackColor = Color.FromArgb(0, 16, 36);
            // Ajoutez ici les autres boutons si nécessaire
        }

        private void fermer_Click(object sender, EventArgs e)
        {
            this.Close();
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

        // Gestionnaires d'événements pour les boutons du menu
        private void dashboardBtn_Click(object sender, EventArgs e)
        {
            ShowDashboard();
        }

        private void UserMenuBtn_Click(object sender, EventArgs e)
        {
            ShowUserPanel();
        }

        private void AeroportMenuBtn_Click(object sender, EventArgs e)
        {
            ShowAeroportPanel();
        }

        private void AeroportMenuBtn_Click_1(object sender, EventArgs e)
        {
            ShowAeroportPanel();
        }

        private void UserMenuBtn_Click_1(object sender, EventArgs e)
        {
            ShowDashboard();
        }

        private void dashboardBtn_Click_1(object sender, EventArgs e)
        {
            ShowDashboard();
        }

        private void AvionMenuBtn_Click(object sender, EventArgs e)
        {
            ShowAvionPanel();
        }
    }
}