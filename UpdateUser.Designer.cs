namespace AvionManagment
{
    partial class UpdateUser
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.showPassword = new System.Windows.Forms.CheckBox();
            this.loginBtn = new System.Windows.Forms.Button();
            this.passwordUpdate = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.usernameUpdate = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.fermer = new System.Windows.Forms.Label();
            this.UpdateUserBtn = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(14)))), ((int)(((byte)(28)))));
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(285, 450);
            this.panel1.TabIndex = 7;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Arial Rounded MT Bold", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(12, 187);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(258, 15);
            this.label5.TabIndex = 1;
            this.label5.Text = "Gestion de reservation de billet d\'avion";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::AvionManagment.Properties.Resources.logoo;
            this.pictureBox1.Location = new System.Drawing.Point(44, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(193, 168);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // showPassword
            // 
            this.showPassword.AutoSize = true;
            this.showPassword.Location = new System.Drawing.Point(305, 307);
            this.showPassword.Name = "showPassword";
            this.showPassword.Size = new System.Drawing.Size(139, 17);
            this.showPassword.TabIndex = 15;
            this.showPassword.Text = "Afficher le mot de passe";
            this.showPassword.UseVisualStyleBackColor = true;
            // 
            // loginBtn
            // 
            this.loginBtn.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.loginBtn.BackColor = System.Drawing.Color.LightCoral;
            this.loginBtn.FlatAppearance.BorderSize = 0;
            this.loginBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.loginBtn.Font = new System.Drawing.Font("Arial Rounded MT Bold", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.loginBtn.ForeColor = System.Drawing.Color.White;
            this.loginBtn.Location = new System.Drawing.Point(301, 372);
            this.loginBtn.Name = "loginBtn";
            this.loginBtn.Size = new System.Drawing.Size(143, 28);
            this.loginBtn.TabIndex = 14;
            this.loginBtn.Text = "Annuler";
            this.loginBtn.UseVisualStyleBackColor = false;
            // 
            // passwordUpdate
            // 
            this.passwordUpdate.Location = new System.Drawing.Point(305, 281);
            this.passwordUpdate.Name = "passwordUpdate";
            this.passwordUpdate.PasswordChar = '*';
            this.passwordUpdate.Size = new System.Drawing.Size(308, 20);
            this.passwordUpdate.TabIndex = 13;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Arial Rounded MT Bold", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(302, 263);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(91, 15);
            this.label4.TabIndex = 12;
            this.label4.Text = "Mot de passe";
            // 
            // usernameUpdate
            // 
            this.usernameUpdate.Location = new System.Drawing.Point(305, 217);
            this.usernameUpdate.Name = "usernameUpdate";
            this.usernameUpdate.Size = new System.Drawing.Size(308, 20);
            this.usernameUpdate.TabIndex = 11;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.label2.Font = new System.Drawing.Font("Arial Rounded MT Bold", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(334, 91);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(255, 22);
            this.label2.TabIndex = 9;
            this.label2.Text = "Modification de l\'utilisateur";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial Rounded MT Bold", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(302, 199);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(118, 15);
            this.label3.TabIndex = 10;
            this.label3.Text = "Nom d\'utilisateur";
            // 
            // fermer
            // 
            this.fermer.AutoSize = true;
            this.fermer.Cursor = System.Windows.Forms.Cursors.Hand;
            this.fermer.Font = new System.Drawing.Font("Arial Rounded MT Bold", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fermer.Location = new System.Drawing.Point(609, 9);
            this.fermer.Name = "fermer";
            this.fermer.Size = new System.Drawing.Size(15, 15);
            this.fermer.TabIndex = 8;
            this.fermer.Text = "X";
            // 
            // UpdateUserBtn
            // 
            this.UpdateUserBtn.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.UpdateUserBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(14)))), ((int)(((byte)(28)))));
            this.UpdateUserBtn.FlatAppearance.BorderSize = 0;
            this.UpdateUserBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.UpdateUserBtn.Font = new System.Drawing.Font("Arial Rounded MT Bold", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UpdateUserBtn.ForeColor = System.Drawing.Color.White;
            this.UpdateUserBtn.Location = new System.Drawing.Point(470, 372);
            this.UpdateUserBtn.Name = "UpdateUserBtn";
            this.UpdateUserBtn.Size = new System.Drawing.Size(143, 28);
            this.UpdateUserBtn.TabIndex = 14;
            this.UpdateUserBtn.Text = "Modifier";
            this.UpdateUserBtn.UseVisualStyleBackColor = false;
            this.UpdateUserBtn.Click += new System.EventHandler(this.UpdateUserBtn_Click);
            // 
            // UpdateUser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(639, 450);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.showPassword);
            this.Controls.Add(this.UpdateUserBtn);
            this.Controls.Add(this.loginBtn);
            this.Controls.Add(this.passwordUpdate);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.usernameUpdate);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.fermer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "UpdateUser";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "UpdateUser";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.CheckBox showPassword;
        private System.Windows.Forms.Button loginBtn;
        private System.Windows.Forms.TextBox passwordUpdate;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox usernameUpdate;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label fermer;
        private System.Windows.Forms.Button UpdateUserBtn;
    }
}