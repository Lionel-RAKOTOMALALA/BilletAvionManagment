using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AvionManagment
{
    public partial class AdminMainForm : Form
    {
        public AdminMainForm()
        {
            InitializeComponent();
            TopBar.BackColor = Color.FromArgb(3, 14, 28);
            Sidebar.BackColor = Color.FromArgb(0, 16, 36);
            dashboardBtn.BackColor = Color.FromArgb(0, 16, 36);
        }

        private void fermer_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void DeconnectionBtn_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("Voulez-vous vraiment vous déconnecter?", "Déconnexion", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                this.Hide();
                Form1 loginForm = new Form1();
                loginForm.Show();
            }
        }
    }
}
