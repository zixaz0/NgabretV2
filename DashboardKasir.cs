using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace rental_mobiV2
{
    public partial class DashboardKasir : Form
    {
        private string username;
        private string connStr = "server=localhost;user=root;password=;database=rental_mobil;";

        public DashboardKasir(string uname)
        {
            InitializeComponent();
            username = uname;
        }

        private void DashboardKasir_Load(object sender, EventArgs e)
        {
            // Saat form dibuka, langsung load dua jenis mobil
            LoadMobilDisewa();
            LoadMobilTersedia();
        }

        // === FUNGSI UNTUK LOAD MOBIL DISEWA ===
        private void LoadMobilDisewa()
        {
            flowLayoutDisewa.Controls.Clear();

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();
                string query = "SELECT * FROM mobil WHERE status = 'disewa'";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        Panel card = BuatCardMobil(dr);
                        flowLayoutDisewa.Controls.Add(card);
                    }
                }
            }

            lblDisewaCount.Text = $"{flowLayoutDisewa.Controls.Count} Mobil Disewa";
        }

        // === FUNGSI UNTUK LOAD MOBIL TERSEDIA ===
        private void LoadMobilTersedia()
        {
            flowLayoutTersedia.Controls.Clear();

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();
                string query = "SELECT * FROM mobil WHERE status = 'tersedia'";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        Panel card = BuatCardMobil(dr);
                        flowLayoutTersedia.Controls.Add(card);
                    }
                }
            }

            lblTersediaCount.Text = $"{flowLayoutTersedia.Controls.Count} Mobil Tersedia";
        }

        // === FUNGSI UNTUK BUAT CARD MOBIL ===
        private Panel BuatCardMobil(MySqlDataReader dr)
        {
            string merk = dr["merk"].ToString();
            string model = dr["model"].ToString();
            string fotoPath = dr["foto"].ToString();
            string status = dr["status"].ToString();

            Panel card = new Panel
            {
                Width = 200,
                Height = 230,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                Margin = new Padding(10)
            };

            PictureBox pic = new PictureBox
            {
                Width = 180,
                Height = 120,
                Location = new Point(10, 10),
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.FixedSingle
            };

            try
            {
                if (!string.IsNullOrEmpty(fotoPath) && File.Exists(fotoPath))
                    pic.Image = Image.FromFile(fotoPath);
            }
            catch { }

            Label lblMerk = new Label
            {
                Text = $"{merk} {model}",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = false,
                Width = 180,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(10, 140)
            };

            Label lblStatus = new Label
            {
                Text = "Status: " + status,
                Font = new Font("Segoe UI", 9, FontStyle.Italic),
                AutoSize = false,
                Width = 180,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(10, 170),
                ForeColor = status == "disewa" ? Color.Red : Color.Green
            };

            card.Controls.Add(pic);
            card.Controls.Add(lblMerk);
            card.Controls.Add(lblStatus);

            return card;
        }

        // === NAVIGASI (tombol ke form lain) ===
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            FormMobil mobilForm = new FormMobil(username);
            mobilForm.Show();
            this.Hide();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            FormPengembalian formPengembalian = new FormPengembalian(username);
            formPengembalian.Show();
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Apakah Anda yakin ingin logout?",
                "Konfirmasi Logout",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                LoginForm loginForm = new LoginForm();
                loginForm.Show();
                this.Close();
            }
        }
        private void panel2_Paint(object sender, PaintEventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }
        private void label1_Click(object sender, EventArgs e) { }
        private void panel3_Paint(object sender, PaintEventArgs e) { }
        private void panel6_Paint(object sender, PaintEventArgs e) { }
        private void panel5_Paint(object sender, PaintEventArgs e) { }
        private void panel4_Paint(object sender, PaintEventArgs e) { }
        private void label3_Click(object sender, EventArgs e) { }
        private void panel1_Paint(object sender, PaintEventArgs e) { }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            RiwayatKasir riwayatForm = new RiwayatKasir(username);
            riwayatForm.Show();
            this.Hide();
        }

        private void label4_Click(object sender, EventArgs e)
        {
            RiwayatKasir riwayatForm = new RiwayatKasir(username);
            riwayatForm.Show();
            this.Hide();
        }
    }
}
