
namespace rental_mobiV2
{
    partial class FormEditUser
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
            this.txtEditId = new System.Windows.Forms.TextBox();
            this.btnSimpanEdit = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.cmbEditRole = new System.Windows.Forms.ComboBox();
            this.txtEditPassword = new System.Windows.Forms.TextBox();
            this.txtEditUsername = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.btnBatal = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtEditId
            // 
            this.txtEditId.Location = new System.Drawing.Point(343, 205);
            this.txtEditId.Name = "txtEditId";
            this.txtEditId.ReadOnly = true;
            this.txtEditId.Size = new System.Drawing.Size(35, 20);
            this.txtEditId.TabIndex = 77;
            this.txtEditId.Visible = false;
            // 
            // btnSimpanEdit
            // 
            this.btnSimpanEdit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.btnSimpanEdit.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSimpanEdit.Font = new System.Drawing.Font("Poppins", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSimpanEdit.Location = new System.Drawing.Point(249, 159);
            this.btnSimpanEdit.Name = "btnSimpanEdit";
            this.btnSimpanEdit.Size = new System.Drawing.Size(75, 23);
            this.btnSimpanEdit.TabIndex = 76;
            this.btnSimpanEdit.Text = "Update";
            this.btnSimpanEdit.UseVisualStyleBackColor = false;
            this.btnSimpanEdit.Click += new System.EventHandler(this.btnSimpanEdit_Click);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Poppins", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(51, 134);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(35, 22);
            this.label12.TabIndex = 75;
            this.label12.Text = "Role";
            // 
            // cmbEditRole
            // 
            this.cmbEditRole.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.cmbEditRole.FormattingEnabled = true;
            this.cmbEditRole.Items.AddRange(new object[] {
            "Admin",
            "Kasir"});
            this.cmbEditRole.Location = new System.Drawing.Point(55, 159);
            this.cmbEditRole.Name = "cmbEditRole";
            this.cmbEditRole.Size = new System.Drawing.Size(100, 21);
            this.cmbEditRole.TabIndex = 74;
            // 
            // txtEditPassword
            // 
            this.txtEditPassword.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.txtEditPassword.Location = new System.Drawing.Point(195, 84);
            this.txtEditPassword.Name = "txtEditPassword";
            this.txtEditPassword.Size = new System.Drawing.Size(100, 20);
            this.txtEditPassword.TabIndex = 73;
            this.txtEditPassword.UseSystemPasswordChar = true;
            // 
            // txtEditUsername
            // 
            this.txtEditUsername.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.txtEditUsername.Location = new System.Drawing.Point(52, 84);
            this.txtEditUsername.Name = "txtEditUsername";
            this.txtEditUsername.Size = new System.Drawing.Size(100, 20);
            this.txtEditUsername.TabIndex = 72;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Poppins", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(191, 59);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(68, 22);
            this.label13.TabIndex = 71;
            this.label13.Text = "Password";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Poppins", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(48, 59);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(71, 22);
            this.label14.TabIndex = 70;
            this.label14.Text = "Username";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Poppins", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(107, 3);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(153, 34);
            this.label15.TabIndex = 69;
            this.label15.Text = "Form Edit User";
            // 
            // btnBatal
            // 
            this.btnBatal.BackColor = System.Drawing.Color.Red;
            this.btnBatal.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBatal.Font = new System.Drawing.Font("Poppins", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBatal.Location = new System.Drawing.Point(188, 159);
            this.btnBatal.Name = "btnBatal";
            this.btnBatal.Size = new System.Drawing.Size(59, 23);
            this.btnBatal.TabIndex = 78;
            this.btnBatal.Text = "Batal";
            this.btnBatal.UseVisualStyleBackColor = false;
            this.btnBatal.Click += new System.EventHandler(this.btnBatal_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.panel1.Controls.Add(this.label15);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(377, 35);
            this.panel1.TabIndex = 79;
            // 
            // FormEditUser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(377, 225);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnBatal);
            this.Controls.Add(this.txtEditId);
            this.Controls.Add(this.btnSimpanEdit);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.cmbEditRole);
            this.Controls.Add(this.txtEditPassword);
            this.Controls.Add(this.txtEditUsername);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label14);
            this.Name = "FormEditUser";
            this.Text = "FormEditUser";
            this.Load += new System.EventHandler(this.FormEditUser_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtEditId;
        private System.Windows.Forms.Button btnSimpanEdit;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.ComboBox cmbEditRole;
        private System.Windows.Forms.TextBox txtEditPassword;
        private System.Windows.Forms.TextBox txtEditUsername;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Button btnBatal;
        private System.Windows.Forms.Panel panel1;
    }
}