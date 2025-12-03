
namespace rental_mobiV2
{
    partial class FormPengembalian
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
            this.flowPanelCards = new System.Windows.Forms.FlowLayoutPanel();
            this.ModalPanel = new System.Windows.Forms.Panel();
            this.panelTop = new System.Windows.Forms.Panel();
            this.btnKembali = new System.Windows.Forms.Button();
            this.btnCari = new System.Windows.Forms.Button();
            this.txtCari = new System.Windows.Forms.TextBox();
            this.panel1.SuspendLayout();
            this.flowPanelCards.SuspendLayout();
            this.panelTop.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.flowPanelCards);
            this.panel1.Controls.Add(this.panelTop);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(800, 450);
            this.panel1.TabIndex = 0;
            // 
            // flowPanelCards
            // 
            this.flowPanelCards.AutoScroll = true;
            this.flowPanelCards.Controls.Add(this.ModalPanel);
            this.flowPanelCards.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowPanelCards.Location = new System.Drawing.Point(0, 77);
            this.flowPanelCards.Name = "flowPanelCards";
            this.flowPanelCards.Size = new System.Drawing.Size(800, 373);
            this.flowPanelCards.TabIndex = 2;
            // 
            // ModalPanel
            // 
            this.ModalPanel.Location = new System.Drawing.Point(3, 3);
            this.ModalPanel.Name = "ModalPanel";
            this.ModalPanel.Size = new System.Drawing.Size(247, 153);
            this.ModalPanel.TabIndex = 0;
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.panelTop.Controls.Add(this.btnKembali);
            this.panelTop.Controls.Add(this.btnCari);
            this.panelTop.Controls.Add(this.txtCari);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(800, 77);
            this.panelTop.TabIndex = 1;
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
            // btnCari
            // 
            this.btnCari.Location = new System.Drawing.Point(734, 30);
            this.btnCari.Name = "btnCari";
            this.btnCari.Size = new System.Drawing.Size(54, 23);
            this.btnCari.TabIndex = 1;
            this.btnCari.Text = "🔍";
            this.btnCari.UseVisualStyleBackColor = true;
            // 
            // txtCari
            // 
            this.txtCari.Location = new System.Drawing.Point(582, 33);
            this.txtCari.Name = "txtCari";
            this.txtCari.Size = new System.Drawing.Size(137, 20);
            this.txtCari.TabIndex = 0;
            // 
            // FormPengembalian
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.panel1);
            this.Name = "FormPengembalian";
            this.Text = "FormPengembalian";
            this.Load += new System.EventHandler(this.FormPengembalian_Load);
            this.panel1.ResumeLayout(false);
            this.flowPanelCards.ResumeLayout(false);
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.FlowLayoutPanel flowPanelCards;
        private System.Windows.Forms.Panel ModalPanel;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Button btnKembali;
        private System.Windows.Forms.Button btnCari;
        private System.Windows.Forms.TextBox txtCari;
    }
}