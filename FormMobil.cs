using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace rental_mobiV2
{

    public partial class FormMobil : Form
    {
        string connectionString = "server=localhost;uid=root;pwd=;database=rental_mobil;";
        private string username;
        private DateTime lastUpdateMobil = DateTime.MinValue;
        public FormMobil(string uname)
        {
            InitializeComponent();
            username = uname;
        }

        private DateTime GetLatestUpdate()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT MAX(updated_at) FROM mobil";
                MySqlCommand cmd = new MySqlCommand(query, conn);

                object result = cmd.ExecuteScalar();

                if (result != DBNull.Value && result != null)
                {
                    return Convert.ToDateTime(result);
                }

                return DateTime.MinValue;
            }
        }

        private void FormMobil_Load(object sender, EventArgs e)
        {
            cmbFilter.Items.AddRange(new string[] { "Semua", "tersedia", "disewa", "servis" });
            cmbFilter.SelectedIndex = 0;
            LoadDataMobil();

            // Simpan update awal
            lastUpdateMobil = GetLatestUpdate();

            // Aktifkan timer
            timerAutoRefresh.Interval = 1000; // 1 detik
            timerAutoRefresh.Start();
        }

        private void timerAutoRefresh_Tick(object sender, EventArgs e)
        {
            DateTime latestUpdate = GetLatestUpdate();

            if (latestUpdate > lastUpdateMobil)
            {
                // Ada perubahan → reload UI
                LoadDataMobil(txtCariMobil.Text, cmbFilter.Text);

                // Simpan update terakhir
                lastUpdateMobil = latestUpdate;
            }
        }

        private void LoadDataMobil(string keyword = "", string filter = "Semua")
        {
            flowMobil.SuspendLayout(); // cegah flicker

            flowMobil.Controls.Clear();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM mobil WHERE (merk LIKE @keyword OR model LIKE @keyword)";
                if (filter != "Semua") query += " AND status = @status";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@keyword", "%" + keyword + "%");
                if (filter != "Semua")
                    cmd.Parameters.AddWithValue("@status", filter.ToLower());

                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Panel card = BuatCardMobil(
                        reader["id"].ToString(),
                        reader["merk"].ToString(),
                        reader["model"].ToString(),
                        reader["tahun"].ToString(),
                        reader["harga_sewa_perhari"].ToString(),
                        reader["foto"].ToString(),
                        reader["status"].ToString()
                    );
                    flowMobil.Controls.Add(card);
                }
            }

            flowMobil.ResumeLayout();
        }

        private Panel BuatCardMobil(string id, string merk, string model, string tahun, string harga, string foto, string status)
        {
            // Panel utama card
            Panel card = new Panel
            {
                Width = 250,
                Height = 340,
                Margin = new Padding(12),
                BackColor = Color.White,
                BorderStyle = BorderStyle.None
            };
            card.Paint += (s, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, card.ClientRectangle,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.LightGray, 1, ButtonBorderStyle.Solid);
            };

            // Foto mobil
            PictureBox pic = new PictureBox
            {
                Width = 220,
                Height = 140,
                Location = new Point(15, 10),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.WhiteSmoke,
                BorderStyle = BorderStyle.None
            };
            try
            {
                if (System.IO.File.Exists(foto))
                    pic.Image = Image.FromFile(foto);
            }
            catch { }

            // Label merk
            Label lblMerk = new Label
            {
                Text = merk + " " + model,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 40, 40),
                AutoSize = false,
                Width = 220,
                Location = new Point(15, 160),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Label tahun & harga
            Label lblTahun = new Label
            {
                Text = "Tahun: " + tahun,
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.DimGray,
                AutoSize = false,
                Width = 100,
                Location = new Point(25, 190)
            };

            Label lblHarga = new Label
            {
                Text = "Rp " + harga + " /hari",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.SeaGreen,
                AutoSize = false,
                Width = 120,
                Location = new Point(130, 190),
                TextAlign = ContentAlignment.MiddleRight
            };

            // Badge status
            Label lblStatus = new Label
            {
                Text = status.ToUpper(),
                AutoSize = false,
                Width = 100,
                Height = 25,
                Location = new Point(75, 215),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = (status == "tersedia") ? Color.FromArgb(46, 204, 113) :
                            (status == "disewa") ? Color.FromArgb(231, 76, 60) :
                            Color.FromArgb(241, 196, 15),
                BorderStyle = BorderStyle.None,
                Padding = new Padding(2)
            };
            lblStatus.Region = new Region(new Rectangle(0, 0, lblStatus.Width, lblStatus.Height));

            // Tombol Detail
            Button btnDetail = new Button
            {
                Text = "Detail",
                Width = 100,
                Height = 35,
                Location = new Point(20, 260),
                BackColor = Color.FromArgb(52, 73, 94),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Tag = id
            };
            btnDetail.FlatAppearance.BorderSize = 0;
            btnDetail.Click += BtnDetail_Click;

            // Tombol Sewa atau Badge
            if (status == "tersedia")
            {
                Button btnSewa = new Button
                {
                    Text = "Sewa",
                    Width = 100,
                    Height = 35,
                    Location = new Point(130, 260),
                    BackColor = Color.FromArgb(41, 128, 185),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 9, FontStyle.Bold),
                    Tag = id
                };
                btnSewa.FlatAppearance.BorderSize = 0;
                btnSewa.Click += BtnSewa_Click;
                card.Controls.Add(btnSewa);
            }
            else
            {
                Label lblInfo = new Label
                {
                    Text = (status == "disewa") ? "Sedang Disewa" : "Dalam Servis",
                    Width = 210,
                    Height = 35,
                    Location = new Point(20, 260),
                    TextAlign = ContentAlignment.MiddleCenter,
                    BackColor = Color.LightGray,
                    ForeColor = Color.FromArgb(80, 80, 80),
                    Font = new Font("Segoe UI", 9, FontStyle.Italic)
                };
                card.Controls.Add(lblInfo);
            }

            // Tambahkan semua elemen ke card
            card.Controls.Add(pic);
            card.Controls.Add(lblMerk);
            card.Controls.Add(lblTahun);
            card.Controls.Add(lblHarga);
            card.Controls.Add(lblStatus);
            card.Controls.Add(btnDetail);

            // Efek hover
            card.MouseEnter += (s, e) => card.BackColor = Color.FromArgb(245, 247, 250);
            card.MouseLeave += (s, e) => card.BackColor = Color.White;

            return card;
        }

        private void BtnDetail_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string idMobil = btn.Tag.ToString();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM mobil WHERE id = @id";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", idMobil);

                MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    string info = $"Merk: {reader["merk"]}\n" +
                                  $"Model: {reader["model"]}\n" +
                                  $"Tahun: {reader["tahun"]}\n" +
                                  $"Harga Sewa: Rp {reader["harga_sewa_perhari"]}/hari\n" +
                                  $"Status: {reader["status"]}";
                    MessageBox.Show(info, "Detail Mobil", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void BtnSewa_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string idMobil = btn.Tag.ToString();
            FormTransaksi frm = new FormTransaksi(idMobil);
            frm.ShowDialog();
        }

        private void btnCari_Click(object sender, EventArgs e)
        {
            LoadDataMobil(txtCariMobil.Text, cmbFilter.Text);
        }

        private void cmbFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadDataMobil(txtCariMobil.Text, cmbFilter.Text);
        }

        private void btnKembali_Click(object sender, EventArgs e)
        {
            DashboardKasir dashboard = new DashboardKasir(username);
            dashboard.Show();
            this.Hide();
        }

        private void flowMobil_Paint(object sender, PaintEventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {

        }
    }
}
