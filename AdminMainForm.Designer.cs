namespace AvionManagment
{
    partial class AdminMainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.TopBar = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.fermer = new System.Windows.Forms.Label();
            this.Sidebar = new System.Windows.Forms.Panel();
            this.PaiementMenuBtn = new System.Windows.Forms.Button();
            this.DeconnectionBtn = new System.Windows.Forms.Button();
            this.BilletMenuBtn = new System.Windows.Forms.Button();
            this.ReservationMenuBtn = new System.Windows.Forms.Button();
            this.VolMenuBtn = new System.Windows.Forms.Button();
            this.AvionMenuBtn = new System.Windows.Forms.Button();
            this.AeroportMenuBtn = new System.Windows.Forms.Button();
            this.UserMenuBtn = new System.Windows.Forms.Button();
            this.dashboardBtn = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.contentPanel = new System.Windows.Forms.Panel();
            this.TopBar.SuspendLayout();
            this.Sidebar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // TopBar
            // 
            this.TopBar.BackColor = System.Drawing.Color.Black;
            this.TopBar.Controls.Add(this.label1);
            this.TopBar.Controls.Add(this.fermer);
            this.TopBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.TopBar.Location = new System.Drawing.Point(0, 0);
            this.TopBar.Name = "TopBar";
            this.TopBar.Size = new System.Drawing.Size(1067, 39);
            this.TopBar.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial Rounded MT Bold", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(3, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(291, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "Gestion de réservation de billet d\'avion";
            // 
            // fermer
            // 
            this.fermer.AutoSize = true;
            this.fermer.Cursor = System.Windows.Forms.Cursors.Hand;
            this.fermer.Font = new System.Drawing.Font("Arial Rounded MT Bold", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fermer.ForeColor = System.Drawing.Color.White;
            this.fermer.Location = new System.Drawing.Point(1037, 9);
            this.fermer.Name = "fermer";
            this.fermer.Size = new System.Drawing.Size(18, 18);
            this.fermer.TabIndex = 1;
            this.fermer.Text = "X";
            this.fermer.Click += new System.EventHandler(this.fermer_Click);
            // 
            // Sidebar
            // 
            this.Sidebar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(16)))), ((int)(((byte)(36)))));
            this.Sidebar.Controls.Add(this.PaiementMenuBtn);
            this.Sidebar.Controls.Add(this.DeconnectionBtn);
            this.Sidebar.Controls.Add(this.BilletMenuBtn);
            this.Sidebar.Controls.Add(this.ReservationMenuBtn);
            this.Sidebar.Controls.Add(this.VolMenuBtn);
            this.Sidebar.Controls.Add(this.AvionMenuBtn);
            this.Sidebar.Controls.Add(this.AeroportMenuBtn);
            this.Sidebar.Controls.Add(this.UserMenuBtn);
            this.Sidebar.Controls.Add(this.dashboardBtn);
            this.Sidebar.Controls.Add(this.label2);
            this.Sidebar.Controls.Add(this.pictureBox1);
            this.Sidebar.Dock = System.Windows.Forms.DockStyle.Left;
            this.Sidebar.Location = new System.Drawing.Point(0, 39);
            this.Sidebar.Name = "Sidebar";
            this.Sidebar.Size = new System.Drawing.Size(208, 683);
            this.Sidebar.TabIndex = 1;
            // 
            // PaiementMenuBtn
            // 
            this.PaiementMenuBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.PaiementMenuBtn.FlatAppearance.BorderSize = 0;
            this.PaiementMenuBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(121)))), ((int)(((byte)(187)))), ((int)(((byte)(255)))));
            this.PaiementMenuBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(121)))), ((int)(((byte)(187)))), ((int)(((byte)(255)))));
            this.PaiementMenuBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.PaiementMenuBtn.Font = new System.Drawing.Font("Arial Rounded MT Bold", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PaiementMenuBtn.Location = new System.Drawing.Point(0, 497);
            this.PaiementMenuBtn.Name = "PaiementMenuBtn";
            this.PaiementMenuBtn.Size = new System.Drawing.Size(208, 39);
            this.PaiementMenuBtn.TabIndex = 3;
            this.PaiementMenuBtn.Text = "Paiement";
            this.PaiementMenuBtn.UseVisualStyleBackColor = true;
            // 
            // DeconnectionBtn
            // 
            this.DeconnectionBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.DeconnectionBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.DeconnectionBtn.Font = new System.Drawing.Font("Arial Rounded MT Bold", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DeconnectionBtn.Location = new System.Drawing.Point(0, 644);
            this.DeconnectionBtn.Name = "DeconnectionBtn";
            this.DeconnectionBtn.Size = new System.Drawing.Size(208, 39);
            this.DeconnectionBtn.TabIndex = 3;
            this.DeconnectionBtn.Text = "Se déconnecter";
            this.DeconnectionBtn.UseVisualStyleBackColor = true;
            this.DeconnectionBtn.Click += new System.EventHandler(this.DeconnectionBtn_Click);
            // 
            // BilletMenuBtn
            // 
            this.BilletMenuBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BilletMenuBtn.FlatAppearance.BorderSize = 0;
            this.BilletMenuBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(121)))), ((int)(((byte)(187)))), ((int)(((byte)(255)))));
            this.BilletMenuBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(121)))), ((int)(((byte)(187)))), ((int)(((byte)(255)))));
            this.BilletMenuBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BilletMenuBtn.Font = new System.Drawing.Font("Arial Rounded MT Bold", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BilletMenuBtn.Location = new System.Drawing.Point(0, 559);
            this.BilletMenuBtn.Name = "BilletMenuBtn";
            this.BilletMenuBtn.Size = new System.Drawing.Size(208, 39);
            this.BilletMenuBtn.TabIndex = 3;
            this.BilletMenuBtn.Text = "Billet";
            this.BilletMenuBtn.UseVisualStyleBackColor = true;
            // 
            // ReservationMenuBtn
            // 
            this.ReservationMenuBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ReservationMenuBtn.FlatAppearance.BorderSize = 0;
            this.ReservationMenuBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(121)))), ((int)(((byte)(187)))), ((int)(((byte)(255)))));
            this.ReservationMenuBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(121)))), ((int)(((byte)(187)))), ((int)(((byte)(255)))));
            this.ReservationMenuBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ReservationMenuBtn.Font = new System.Drawing.Font("Arial Rounded MT Bold", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ReservationMenuBtn.Location = new System.Drawing.Point(0, 429);
            this.ReservationMenuBtn.Name = "ReservationMenuBtn";
            this.ReservationMenuBtn.Size = new System.Drawing.Size(208, 39);
            this.ReservationMenuBtn.TabIndex = 3;
            this.ReservationMenuBtn.Text = "Réservation";
            this.ReservationMenuBtn.UseVisualStyleBackColor = true;
            // 
            // VolMenuBtn
            // 
            this.VolMenuBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.VolMenuBtn.FlatAppearance.BorderSize = 0;
            this.VolMenuBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(121)))), ((int)(((byte)(187)))), ((int)(((byte)(255)))));
            this.VolMenuBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(121)))), ((int)(((byte)(187)))), ((int)(((byte)(255)))));
            this.VolMenuBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.VolMenuBtn.Font = new System.Drawing.Font("Arial Rounded MT Bold", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.VolMenuBtn.Location = new System.Drawing.Point(0, 369);
            this.VolMenuBtn.Name = "VolMenuBtn";
            this.VolMenuBtn.Size = new System.Drawing.Size(208, 39);
            this.VolMenuBtn.TabIndex = 3;
            this.VolMenuBtn.Text = "Vol";
            this.VolMenuBtn.UseVisualStyleBackColor = true;
            // 
            // AvionMenuBtn
            // 
            this.AvionMenuBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.AvionMenuBtn.FlatAppearance.BorderSize = 0;
            this.AvionMenuBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(121)))), ((int)(((byte)(187)))), ((int)(((byte)(255)))));
            this.AvionMenuBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(121)))), ((int)(((byte)(187)))), ((int)(((byte)(255)))));
            this.AvionMenuBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.AvionMenuBtn.Font = new System.Drawing.Font("Arial Rounded MT Bold", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AvionMenuBtn.Location = new System.Drawing.Point(0, 310);
            this.AvionMenuBtn.Name = "AvionMenuBtn";
            this.AvionMenuBtn.Size = new System.Drawing.Size(208, 39);
            this.AvionMenuBtn.TabIndex = 3;
            this.AvionMenuBtn.Text = "Avion";
            this.AvionMenuBtn.UseVisualStyleBackColor = true;
            // 
            // AeroportMenuBtn
            // 
            this.AeroportMenuBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.AeroportMenuBtn.FlatAppearance.BorderSize = 0;
            this.AeroportMenuBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(121)))), ((int)(((byte)(187)))), ((int)(((byte)(255)))));
            this.AeroportMenuBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(121)))), ((int)(((byte)(187)))), ((int)(((byte)(255)))));
            this.AeroportMenuBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.AeroportMenuBtn.Font = new System.Drawing.Font("Arial Rounded MT Bold", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AeroportMenuBtn.Location = new System.Drawing.Point(0, 246);
            this.AeroportMenuBtn.Name = "AeroportMenuBtn";
            this.AeroportMenuBtn.Size = new System.Drawing.Size(208, 39);
            this.AeroportMenuBtn.TabIndex = 3;
            this.AeroportMenuBtn.Text = "Aeroport";
            this.AeroportMenuBtn.UseVisualStyleBackColor = true;
            this.AeroportMenuBtn.Click += new System.EventHandler(this.AeroportMenuBtn_Click_1);
            // 
            // UserMenuBtn
            // 
            this.UserMenuBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.UserMenuBtn.FlatAppearance.BorderSize = 0;
            this.UserMenuBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(121)))), ((int)(((byte)(187)))), ((int)(((byte)(255)))));
            this.UserMenuBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(121)))), ((int)(((byte)(187)))), ((int)(((byte)(255)))));
            this.UserMenuBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.UserMenuBtn.Font = new System.Drawing.Font("Arial Rounded MT Bold", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UserMenuBtn.Location = new System.Drawing.Point(0, 184);
            this.UserMenuBtn.Name = "UserMenuBtn";
            this.UserMenuBtn.Size = new System.Drawing.Size(208, 39);
            this.UserMenuBtn.TabIndex = 3;
            this.UserMenuBtn.Text = "Utilisateur";
            this.UserMenuBtn.UseVisualStyleBackColor = true;
            this.UserMenuBtn.Click += new System.EventHandler(this.UserMenuBtn_Click_1);
            // 
            // dashboardBtn
            // 
            this.dashboardBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.dashboardBtn.FlatAppearance.BorderSize = 0;
            this.dashboardBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(121)))), ((int)(((byte)(187)))), ((int)(((byte)(255)))));
            this.dashboardBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(121)))), ((int)(((byte)(187)))), ((int)(((byte)(255)))));
            this.dashboardBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.dashboardBtn.Font = new System.Drawing.Font("Arial Rounded MT Bold", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dashboardBtn.Location = new System.Drawing.Point(0, 126);
            this.dashboardBtn.Name = "dashboardBtn";
            this.dashboardBtn.Size = new System.Drawing.Size(208, 39);
            this.dashboardBtn.TabIndex = 3;
            this.dashboardBtn.Text = "Dashboard";
            this.dashboardBtn.UseVisualStyleBackColor = true;
            this.dashboardBtn.Click += new System.EventHandler(this.dashboardBtn_Click_1);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.label2.Font = new System.Drawing.Font("Arial Rounded MT Bold", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(46, 81);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(111, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "Bienvenu à vous";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::AvionManagment.Properties.Resources.logoKely;
            this.pictureBox1.Location = new System.Drawing.Point(55, 16);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(87, 62);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // contentPanel
            // 
            this.contentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentPanel.Location = new System.Drawing.Point(208, 39);
            this.contentPanel.Name = "contentPanel";
            this.contentPanel.Size = new System.Drawing.Size(859, 683);
            this.contentPanel.TabIndex = 2;
            // 
            // AdminMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1067, 722);
            this.Controls.Add(this.contentPanel);
            this.Controls.Add(this.Sidebar);
            this.Controls.Add(this.TopBar);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "AdminMainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "AdminMainForm";
            this.TopBar.ResumeLayout(false);
            this.TopBar.PerformLayout();
            this.Sidebar.ResumeLayout(false);
            this.Sidebar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel TopBar;
        private System.Windows.Forms.Label fermer;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel Sidebar;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button dashboardBtn;
        private System.Windows.Forms.Button UserMenuBtn;
        private System.Windows.Forms.Button PaiementMenuBtn;
        private System.Windows.Forms.Button DeconnectionBtn;
        private System.Windows.Forms.Button BilletMenuBtn;
        private System.Windows.Forms.Button ReservationMenuBtn;
        private System.Windows.Forms.Button VolMenuBtn;
        private System.Windows.Forms.Button AvionMenuBtn;
        private System.Windows.Forms.Button AeroportMenuBtn;
        private System.Windows.Forms.Panel contentPanel;
    }
}