
namespace rental_mobiV2
{
    partial class FormMobil
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
            this.components = new System.ComponentModel.Container();
            this.panelTop = new System.Windows.Forms.Panel();
            this.btnKembali = new System.Windows.Forms.Button();
            this.cmbFilter = new System.Windows.Forms.ComboBox();
            this.btnCari = new System.Windows.Forms.Button();
            this.txtCariMobil = new System.Windows.Forms.TextBox();
            this.flowMobil = new System.Windows.Forms.FlowLayoutPanel();
            this.timerAutoRefresh = new System.Windows.Forms.Timer(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.panelTop.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.panelTop.Controls.Add(this.btnKembali);
            this.panelTop.Controls.Add(this.cmbFilter);
            this.panelTop.Controls.Add(this.btnCari);
            this.panelTop.Controls.Add(this.txtCariMobil);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(800, 77);
            this.panelTop.TabIndex = 0;
            // 
            // btnKembali
            // 
            this.btnKembali.Font = new System.Drawing.Font("Poppins", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnKembali.Location = new System.Drawing.Point(0, 0);
            this.btnKembali.Name = "btnKembali";
            this.btnKembali.Size = new System.Drawing.Size(97, 27);
            this.btnKembali.TabIndex = 6;
            this.btnKembali.Text = "Kembali";
            this.btnKembali.UseVisualStyleBackColor = true;
            this.btnKembali.Click += new System.EventHandler(this.btnKembali_Click);
            // 
            // cmbFilter
            // 
            this.cmbFilter.FormattingEnabled = true;
            this.cmbFilter.Location = new System.Drawing.Point(635, 28);
            this.cmbFilter.Name = "cmbFilter";
            this.cmbFilter.Size = new System.Drawing.Size(121, 21);
            this.cmbFilter.TabIndex = 2;
            this.cmbFilter.SelectedIndexChanged += new System.EventHandler(this.cmbFilter_SelectedIndexChanged);
            // 
            // btnCari
            // 
            this.btnCari.Location = new System.Drawing.Point(261, 38);
            this.btnCari.Name = "btnCari";
            this.btnCari.Size = new System.Drawing.Size(54, 23);
            this.btnCari.TabIndex = 1;
            this.btnCari.Text = "🔍";
            this.btnCari.UseVisualStyleBackColor = true;
            this.btnCari.Click += new System.EventHandler(this.btnCari_Click);
            // 
            // txtCariMobil
            // 
            this.txtCariMobil.Location = new System.Drawing.Point(108, 40);
            this.txtCariMobil.Name = "txtCariMobil";
            this.txtCariMobil.Size = new System.Drawing.Size(137, 20);
            this.txtCariMobil.TabIndex = 0;
            // 
            // flowMobil
            // 
            this.flowMobil.AutoScroll = true;
            this.flowMobil.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowMobil.Location = new System.Drawing.Point(0, 77);
            this.flowMobil.Name = "flowMobil";
            this.flowMobil.Size = new System.Drawing.Size(800, 373);
            this.flowMobil.TabIndex = 1;
            this.flowMobil.Paint += new System.Windows.Forms.PaintEventHandler(this.flowMobil_Paint);
            // 
            // timerAutoRefresh
            // 
            this.timerAutoRefresh.Enabled = true;
            this.timerAutoRefresh.Interval = 1000;
            this.timerAutoRefresh.Tick += new System.EventHandler(this.timerAutoRefresh_Tick);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // FormMobil
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.flowMobil);
            this.Controls.Add(this.panelTop);
            this.Name = "FormMobil";
            this.Text = "FormMobil";
            this.Load += new System.EventHandler(this.FormMobil_Load);
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.ComboBox cmbFilter;
        private System.Windows.Forms.Button btnCari;
        private System.Windows.Forms.TextBox txtCariMobil;
        private System.Windows.Forms.FlowLayoutPanel flowMobil;
        private System.Windows.Forms.Button btnKembali;
        private System.Windows.Forms.Timer timerAutoRefresh;
        private System.Windows.Forms.Timer timer1;
    }
}