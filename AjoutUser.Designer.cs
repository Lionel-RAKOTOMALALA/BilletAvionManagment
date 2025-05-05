namespace AvionManagment
{
    partial class AjoutUser
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
            this.label3 = new System.Windows.Forms.Label();
            this.fermer = new System.Windows.Forms.Label();
            this.showPassword = new System.Windows.Forms.CheckBox();
            this.ajoutBtn = new System.Windows.Forms.Button();
            this.passwordConfirm = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.password = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.username = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
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
            this.panel1.TabIndex = 16;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Arial Rounded MT Bold", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(12, 225);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(258, 15);
            this.label5.TabIndex = 1;
            this.label5.Text = "Gestion de reservation de billet d\'avion";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::AvionManagment.Properties.Resources.logoo;
            this.pictureBox1.Location = new System.Drawing.Point(30, 31);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(193, 168);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial Rounded MT Bold", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(312, 161);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(118, 15);
            this.label3.TabIndex = 18;
            this.label3.Text = "Nom d\'utilisateur";
            // 
            // fermer
            // 
            this.fermer.AutoSize = true;
            this.fermer.Cursor = System.Windows.Forms.Cursors.Hand;
            this.fermer.Font = new System.Drawing.Font("Arial Rounded MT Bold", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fermer.Location = new System.Drawing.Point(643, 9);
            this.fermer.Name = "fermer";
            this.fermer.Size = new System.Drawing.Size(15, 15);
            this.fermer.TabIndex = 26;
            this.fermer.Text = "X";
            // 
            // showPassword
            // 
            this.showPassword.AutoSize = true;
            this.showPassword.Location = new System.Drawing.Point(315, 334);
            this.showPassword.Name = "showPassword";
            this.showPassword.Size = new System.Drawing.Size(139, 17);
            this.showPassword.TabIndex = 25;
            this.showPassword.Text = "Afficher le mot de passe";
            this.showPassword.UseVisualStyleBackColor = true;
            
            // 
            // ajoutBtn
            // 
            this.ajoutBtn.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ajoutBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(14)))), ((int)(((byte)(28)))));
            this.ajoutBtn.FlatAppearance.BorderSize = 0;
            this.ajoutBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ajoutBtn.Font = new System.Drawing.Font("Arial Rounded MT Bold", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ajoutBtn.ForeColor = System.Drawing.Color.White;
            this.ajoutBtn.Location = new System.Drawing.Point(482, 379);
            this.ajoutBtn.Name = "ajoutBtn";
            this.ajoutBtn.Size = new System.Drawing.Size(122, 28);
            this.ajoutBtn.TabIndex = 24;
            this.ajoutBtn.Text = "Ajouter";
            this.ajoutBtn.UseVisualStyleBackColor = false;
            this.ajoutBtn.Click += new System.EventHandler(this.ajoutBtn_Click);
            // 
            // passwordConfirm
            // 
            this.passwordConfirm.Location = new System.Drawing.Point(315, 308);
            this.passwordConfirm.Name = "passwordConfirm";
            this.passwordConfirm.PasswordChar = '*';
            this.passwordConfirm.Size = new System.Drawing.Size(308, 20);
            this.passwordConfirm.TabIndex = 22;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Arial Rounded MT Bold", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(312, 290);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(176, 15);
            this.label7.TabIndex = 20;
            this.label7.Text = "Confirmer le mot de passe";
            // 
            // password
            // 
            this.password.Location = new System.Drawing.Point(315, 243);
            this.password.Name = "password";
            this.password.PasswordChar = '*';
            this.password.Size = new System.Drawing.Size(308, 20);
            this.password.TabIndex = 23;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Arial Rounded MT Bold", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(312, 225);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(91, 15);
            this.label4.TabIndex = 21;
            this.label4.Text = "Mot de passe";
            // 
            // username
            // 
            this.username.Location = new System.Drawing.Point(315, 179);
            this.username.Name = "username";
            this.username.Size = new System.Drawing.Size(308, 20);
            this.username.TabIndex = 19;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.label2.Font = new System.Drawing.Font("Arial Rounded MT Bold", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(332, 83);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(171, 22);
            this.label2.TabIndex = 17;
            this.label2.Text = "Ajout d\'utilisateur";
            // 
            // button1
            // 
            this.button1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.button1.BackColor = System.Drawing.Color.IndianRed;
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Arial Rounded MT Bold", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.Location = new System.Drawing.Point(332, 379);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(122, 28);
            this.button1.TabIndex = 24;
            this.button1.Text = "Annuler";
            this.button1.UseVisualStyleBackColor = false;
            // 
            // AjoutUser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(660, 450);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.fermer);
            this.Controls.Add(this.showPassword);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.ajoutBtn);
            this.Controls.Add(this.passwordConfirm);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.password);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.username);
            this.Controls.Add(this.label2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "AjoutUser";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Ajout User";
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
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label fermer;
        private System.Windows.Forms.CheckBox showPassword;
        private System.Windows.Forms.Button ajoutBtn;
        private System.Windows.Forms.TextBox passwordConfirm;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox password;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox username;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
    }
}