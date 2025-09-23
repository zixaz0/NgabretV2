using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;



namespace rental_mobiV2
{
    public partial class DataMobilAdmin : Form
    {
        private string username;
        private string connStr = "server=localhost;user=root;password=;database=rental_mobil;";

        public DataMobilAdmin(string uname)
        {
            InitializeComponent();
            username = uname;
        }

        private void DataMobilAdmin_Load(object sender, EventArgs e)
        {
            LoadMobil();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadDataMobil(txtSearch.Text.Trim());
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                                         "Apakah Anda yakin ingin logout?",
                                         "Konfirmasi Logout",
                                         MessageBoxButtons.YesNo,
                                         MessageBoxIcon.Question
                                     );

            if (result == DialogResult.Yes)
            {
                // Kembali ke form login
                LoginForm loginForm = new LoginForm();
                loginForm.Show();

                this.Close(); // tutup dashboard
            }
        }

        private void HapusMobil(string id)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    conn.Open();
                    string query = "DELETE FROM mobil WHERE id=@id";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
                MessageBox.Show("Mobil berhasil dihapus!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error hapus mobil: " + ex.Message);
            }
        }

        private void LoadMobil()
        {
            // pastikan control flowMobil ada dan diberi name 'flowMobil' di Designer
            flowMobil.Controls.Clear();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    conn.Open();
                    string query = "SELECT id, merk, model, tahun, no_plat, harga_sewa_perhari, status FROM mobil";
                    MySqlDataAdapter da = new MySqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    foreach (DataRow row in dt.Rows)
                    {
                        // ambil value ke variabel lokal supaya aman di lambda
                        string id = row["id"].ToString();
                        string merk = row["merk"].ToString();
                        string model = row["model"].ToString();
                        string tahun = row["tahun"].ToString();
                        string noPlat = row["no_plat"].ToString();
                        decimal harga = 0;
                        decimal.TryParse(row["harga_sewa_perhari"].ToString(), out harga);
                        string status = row["status"].ToString();

                        // Buat panel (card)
                        Panel card = new Panel
                        {
                            Width = 260,
                            Height = 180,
                            BorderStyle = BorderStyle.FixedSingle,
                            BackColor = Color.WhiteSmoke,
                            Margin = new Padding(8)
                        };

                        // Judul merk + model
                        Label lblTitle = new Label
                        {
                            Text = $"{merk} {model}",
                            Font = new Font("Segoe UI", 10, FontStyle.Bold),
                            Location = new Point(10, 8),
                            AutoSize = true
                        };

                        // Tahun & Plat
                        Label lblInfo = new Label
                        {
                            Text = $"Tahun: {tahun}\nPlat: {noPlat}",
                            Location = new Point(10, 34),
                            AutoSize = true
                        };

                        // Harga
                        Label lblHarga = new Label
                        {
                            Text = $"Rp {harga:N0}/hari",
                            ForeColor = Color.DarkGreen,
                            Location = new Point(10, 80),
                            AutoSize = true
                        };

                        // Status
                        Label lblStatus = new Label
                        {
                            Text = $"Status: {status}",
                            Location = new Point(10, 104),
                            AutoSize = true
                        };

                        Button btnEdit = new Button
                        {
                            Text = "Edit",
                            Width = 70,
                            Cursor = Cursors.Hand,
                            Location = new Point(10, 130),
                            BackColor = Color.LightBlue,
                        };
                        btnEdit.Click += (s, e) =>
                        {
                            FormEditMobil editForm = new FormEditMobil(
                                id, merk, model, tahun, noPlat, harga.ToString()
                            );

                            if (editForm.ShowDialog() == DialogResult.OK)
                            {
                                LoadMobil(); // refresh data setelah edit
                            }
                        };

                        // Tombol Delete
                        Button btnDelete = new Button
                        {
                            Text = "Delete",
                            Width = 70,
                            Cursor = Cursors.Hand,
                            Location = new Point(100, 130),
                            BackColor = Color.IndianRed
                        };
                        btnDelete.Click += (s, e) =>
                        {
                            var confirm = MessageBox.Show($"Yakin hapus mobil {merk} {model} (plat {noPlat})?",
                                "Konfirmasi Hapus", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                            if (confirm == DialogResult.Yes)
                            {
                                HapusMobil(id);
                                LoadMobil(); // refresh
                            }
                        };
                        card.Controls.Add(lblTitle);
                        card.Controls.Add(lblInfo);
                        card.Controls.Add(lblHarga);
                        card.Controls.Add(lblStatus);
                        card.Controls.Add(btnEdit);
                        card.Controls.Add(btnDelete);
                        // card.Controls.Add(pic);

                        // Tambahkan card ke flowlayoutpanel
                        flowMobil.Controls.Add(card);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error load mobil: " + ex.Message);
            }
        }

        private void btnTambahMobil_Click_1(object sender, EventArgs e)
        {
            using (FormTambahMobil form = new FormTambahMobil())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    string query = "INSERT INTO mobil (merk, model, tahun, no_plat, harga_sewa_perhari, status) " +
                                   "VALUES (@merk, @model, @tahun, @no_plat, @harga_sewa_perhari, @status)";

                    using (var conn = new MySqlConnection("server=localhost;database=rental_mobil;uid=root;pwd=;"))
                    {
                        conn.Open();
                        using (var cmd = new MySqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@merk", form.merk);
                            cmd.Parameters.AddWithValue("@model", form.model);
                            cmd.Parameters.AddWithValue("@tahun", form.tahun);
                            cmd.Parameters.AddWithValue("@no_plat", form.no_plat);
                            cmd.Parameters.AddWithValue("@harga_sewa_perhari", form.harga_sewa_perhari);
                            cmd.Parameters.AddWithValue("@status", "tersedia"); // default

                            cmd.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show("Mobil berhasil ditambahkan!");
                    LoadMobil();
                }
            }
        }

        private void LoadDataMobil(string keyword = "")
        {
            flowMobil.Controls.Clear();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    conn.Open();
                    string query = "SELECT id, merk, model, tahun, no_plat, harga_sewa_perhari, status FROM mobil";

                    // tambahin filter kalau ada keyword
                    if (!string.IsNullOrEmpty(keyword))
                    {
                        query += " WHERE merk LIKE @keyword OR model LIKE @keyword OR no_plat LIKE @keyword";
                    }

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    if (!string.IsNullOrEmpty(keyword))
                        cmd.Parameters.AddWithValue("@keyword", "%" + keyword + "%");

                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    foreach (DataRow row in dt.Rows)
                    {
                        string id = row["id"].ToString();
                        string merk = row["merk"].ToString();
                        string model = row["model"].ToString();
                        string tahun = row["tahun"].ToString();
                        string noPlat = row["no_plat"].ToString();
                        decimal harga = 0;
                        decimal.TryParse(row["harga_sewa_perhari"].ToString(), out harga);
                        string status = row["status"].ToString();

                        // --- Card (copy paste style punyamu) ---
                        Panel card = new Panel
                        {
                            Width = 260,
                            Height = 180,
                            BorderStyle = BorderStyle.FixedSingle,
                            BackColor = Color.WhiteSmoke,
                            Margin = new Padding(8)
                        };

                        Label lblTitle = new Label
                        {
                            Text = $"{merk} {model}",
                            Font = new Font("Segoe UI", 10, FontStyle.Bold),
                            Location = new Point(10, 8),
                            AutoSize = true
                        };

                        Label lblInfo = new Label
                        {
                            Text = $"Tahun: {tahun}\nPlat: {noPlat}",
                            Location = new Point(10, 34),
                            AutoSize = true
                        };

                        Label lblHarga = new Label
                        {
                            Text = $"Rp {harga:N0}/hari",
                            ForeColor = Color.DarkGreen,
                            Location = new Point(10, 80),
                            AutoSize = true
                        };

                        Label lblStatus = new Label
                        {
                            Text = $"Status: {status}",
                            Location = new Point(10, 104),
                            AutoSize = true
                        };

                        Button btnEdit = new Button
                        {
                            Text = "Edit",
                            Width = 70,
                            Location = new Point(10, 130),
                            BackColor = Color.LightBlue
                        };
                        btnEdit.Click += (s, e) =>
                        {
                            FormEditMobil editForm = new FormEditMobil(
                                id, merk, model, tahun, noPlat, harga.ToString()
                            );

                            if (editForm.ShowDialog() == DialogResult.OK)
                            {
                                LoadDataMobil(keyword); // refresh dengan filter biar konsisten
                            }
                        };

                        Button btnDelete = new Button
                        {
                            Text = "Delete",
                            Width = 70,
                            Location = new Point(100, 130),
                            BackColor = Color.IndianRed
                        };
                        btnDelete.Click += (s, e) =>
                        {
                            var confirm = MessageBox.Show(
                                $"Yakin hapus mobil {merk} {model} (plat {noPlat})?",
                                "Konfirmasi Hapus",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Warning);

                            if (confirm == DialogResult.Yes)
                            {
                                HapusMobil(id);
                                LoadDataMobil(keyword);
                            }
                        };

                        card.Controls.Add(lblTitle);
                        card.Controls.Add(lblInfo);
                        card.Controls.Add(lblHarga);
                        card.Controls.Add(lblStatus);
                        card.Controls.Add(btnEdit);
                        card.Controls.Add(btnDelete);

                        flowMobil.Controls.Add(card);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error load mobil: " + ex.Message);
            }
        }

        private void flowMobil_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnTambahMobil_Click(object sender, EventArgs e)
        {
            using (FormTambahMobil form = new FormTambahMobil())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    string query = "INSERT INTO mobil (merk, model, tahun, no_plat, harga_sewa_perhari, status) " +
                                   "VALUES (@merk, @model, @tahun, @no_plat, @harga_sewa_perhari, @status)";

                    using (var conn = new MySqlConnection("server=localhost;database=rental_mobil;uid=root;pwd=;"))
                    {
                        conn.Open();
                        using (var cmd = new MySqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@merk", form.merk);
                            cmd.Parameters.AddWithValue("@model", form.model);
                            cmd.Parameters.AddWithValue("@tahun", form.tahun);
                            cmd.Parameters.AddWithValue("@no_plat", form.no_plat);
                            cmd.Parameters.AddWithValue("@harga_sewa_perhari", form.harga_sewa_perhari);
                            cmd.Parameters.AddWithValue("@status", "tersedia"); // default

                            cmd.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show("Mobil berhasil ditambahkan!");
                    LoadMobil();
                }
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {
            KelolaUserAdmin KasirForm = new KelolaUserAdmin(username);
            KasirForm.Show();
            this.Hide(); // sembunyiin form sekarang
        }

        private void label2_Click(object sender, EventArgs e)
        {
            DashboardAdmin DashboardForm = new DashboardAdmin(username);
            DashboardForm.Show();
            this.Hide(); // sembunyiin form sekarang
        }

        private void label5_Click(object sender, EventArgs e)
        {
            RiwayatAdmin PelangganForm = new RiwayatAdmin(username);
            PelangganForm.Show();
            this.Hide(); // sembunyiin form sekarang
        }
    }
}