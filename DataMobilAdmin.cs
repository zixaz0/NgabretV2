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
                LoginForm loginForm = new LoginForm();
                loginForm.Show();
                this.Close();
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

        // METHOD: Update Status Mobil
        private void UpdateStatusMobil(string id, string statusBaru)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    conn.Open();
                    string query = "UPDATE mobil SET status=@status WHERE id=@id";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@status", statusBaru);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
                MessageBox.Show($"Status mobil berhasil diubah menjadi '{statusBaru}'!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error update status: " + ex.Message);
            }
        }

        private void LoadMobil()
        {
            flowMobil.Controls.Clear();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    conn.Open();
                    // TAMBAH ORDER BY id DESC - mobil terbaru di atas
                    string query = "SELECT id, merk, model, tahun, no_plat, harga_sewa_perhari, status, foto FROM mobil ORDER BY id DESC";
                    MySqlDataAdapter da = new MySqlDataAdapter(query, conn);
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
                        string status = row["status"].ToString().ToLower();
                        string foto = row["foto"].ToString();

                        // TENTUKAN TINGGI CARD BERDASARKAN STATUS
                        int cardHeight = 280; // default tanpa tombol servis
                        if (status == "tersedia" || status == "servis")
                        {
                            cardHeight = 310; // tambah tinggi untuk tombol
                        }

                        Panel card = new Panel
                        {
                            Width = 260,
                            Height = cardHeight,
                            BorderStyle = BorderStyle.FixedSingle,
                            BackColor = Color.WhiteSmoke,
                            Margin = new Padding(8)
                        };

                        PictureBox picMobil = new PictureBox
                        {
                            Width = 240,
                            Height = 120,
                            Location = new Point(10, 8),
                            BorderStyle = BorderStyle.FixedSingle,
                            SizeMode = PictureBoxSizeMode.Zoom
                        };

                        if (!string.IsNullOrEmpty(foto) && System.IO.File.Exists(foto))
                        {
                            try
                            {
                                picMobil.Image = Image.FromFile(foto);
                            }
                            catch
                            {
                                picMobil.BackColor = Color.LightGray;
                            }
                        }
                        else
                        {
                            picMobil.BackColor = Color.LightGray;
                        }

                        Label lblTitle = new Label
                        {
                            Text = $"{merk} {model}",
                            Font = new Font("Segoe UI", 10, FontStyle.Bold),
                            Location = new Point(10, 135),
                            AutoSize = true
                        };

                        Label lblInfo = new Label
                        {
                            Text = $"Tahun: {tahun} | Plat: {noPlat}",
                            Location = new Point(10, 160),
                            AutoSize = true
                        };

                        Label lblHarga = new Label
                        {
                            Text = $"Rp {harga:N0}/hari",
                            ForeColor = Color.DarkGreen,
                            Font = new Font("Segoe UI", 9, FontStyle.Bold),
                            Location = new Point(10, 185),
                            AutoSize = true
                        };

                        Label lblStatus = new Label
                        {
                            Text = $"Status: {status}",
                            Location = new Point(10, 205),
                            AutoSize = true
                        };

                        Button btnEdit = new Button
                        {
                            Text = "Edit",
                            Width = 70,
                            Cursor = Cursors.Hand,
                            Location = new Point(10, 235),
                            BackColor = Color.LightBlue,
                        };
                        btnEdit.Click += (s, e) =>
                        {
                            FormEditMobil editForm = new FormEditMobil(
                                id, merk, model, tahun, noPlat, harga.ToString(), foto
                            );

                            if (editForm.ShowDialog() == DialogResult.OK)
                            {
                                LoadMobil();
                            }
                        };

                        Button btnDelete = new Button
                        {
                            Text = "Delete",
                            Width = 70,
                            Cursor = Cursors.Hand,
                            Location = new Point(100, 235),
                            BackColor = Color.IndianRed
                        };
                        btnDelete.Click += (s, e) =>
                        {
                            var confirm = MessageBox.Show($"Yakin hapus mobil {merk} {model} (plat {noPlat})?",
                                "Konfirmasi Hapus", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                            if (confirm == DialogResult.Yes)
                            {
                                HapusMobil(id);
                                LoadMobil();
                            }
                        };

                        card.Controls.Add(picMobil);
                        card.Controls.Add(lblTitle);
                        card.Controls.Add(lblInfo);
                        card.Controls.Add(lblHarga);
                        card.Controls.Add(lblStatus);
                        card.Controls.Add(btnEdit);
                        card.Controls.Add(btnDelete);

                        // TOMBOL SERVIS - HANYA MUNCUL SESUAI STATUS
                        if (status == "tersedia")
                        {
                            // Status TERSEDIA → Tombol SERVIS
                            Button btnServis = new Button
                            {
                                Text = "Servis",
                                Width = 160,
                                Cursor = Cursors.Hand,
                                Location = new Point(10, 265),
                                BackColor = Color.Orange,
                                ForeColor = Color.White,
                                Font = new Font("Segoe UI", 9, FontStyle.Bold)
                            };
                            btnServis.Click += (s, e) =>
                            {
                                var confirm = MessageBox.Show(
                                    $"Ubah status mobil {merk} {model} menjadi 'servis'?",
                                    "Konfirmasi Servis",
                                    MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Question);

                                if (confirm == DialogResult.Yes)
                                {
                                    UpdateStatusMobil(id, "servis");
                                    LoadMobil();
                                }
                            };
                            card.Controls.Add(btnServis);
                        }
                        else if (status == "servis")
                        {
                            // Status DISERVIS → Tombol SELESAI SERVIS
                            Button btnSelesaiServis = new Button
                            {
                                Text = "Selesai Servis",
                                Width = 160,
                                Cursor = Cursors.Hand,
                                Location = new Point(10, 265),
                                BackColor = Color.Green,
                                ForeColor = Color.White,
                                Font = new Font("Segoe UI", 9, FontStyle.Bold)
                            };
                            btnSelesaiServis.Click += (s, e) =>
                            {
                                var confirm = MessageBox.Show(
                                    $"Ubah status mobil {merk} {model} menjadi 'Tersedia'?",
                                    "Konfirmasi Selesai Servis",
                                    MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Question);

                                if (confirm == DialogResult.Yes)
                                {
                                    UpdateStatusMobil(id, "tersedia");
                                    LoadMobil();
                                }
                            };
                            card.Controls.Add(btnSelesaiServis);
                        }
                        // JIKA STATUS "DISEWA" → TIDAK ADA TOMBOL SERVIS

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
                            cmd.Parameters.AddWithValue("@status", "tersedia");

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
                    string query = "SELECT id, merk, model, tahun, no_plat, harga_sewa_perhari, status, foto FROM mobil";

                    if (!string.IsNullOrEmpty(keyword))
                    {
                        query += " WHERE merk LIKE @keyword OR model LIKE @keyword OR no_plat LIKE @keyword";
                    }

                    // TAMBAH ORDER BY id DESC - mobil terbaru di atas
                    query += " ORDER BY id DESC";

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
                        string status = row["status"].ToString().ToLower();
                        string foto = row["foto"].ToString();

                        // TENTUKAN TINGGI CARD
                        int cardHeight = 280;
                        if (status == "tersedia" || status == "servis")
                        {
                            cardHeight = 310;
                        }

                        Panel card = new Panel
                        {
                            Width = 260,
                            Height = cardHeight,
                            BorderStyle = BorderStyle.FixedSingle,
                            BackColor = Color.WhiteSmoke,
                            Margin = new Padding(8)
                        };

                        PictureBox picMobil = new PictureBox
                        {
                            Width = 240,
                            Height = 120,
                            Location = new Point(10, 8),
                            BorderStyle = BorderStyle.FixedSingle,
                            SizeMode = PictureBoxSizeMode.Zoom
                        };

                        if (!string.IsNullOrEmpty(foto) && System.IO.File.Exists(foto))
                        {
                            try { picMobil.Image = Image.FromFile(foto); }
                            catch { picMobil.BackColor = Color.LightGray; }
                        }
                        else
                        {
                            picMobil.BackColor = Color.LightGray;
                        }

                        Label lblTitle = new Label
                        {
                            Text = $"{merk} {model}",
                            Font = new Font("Segoe UI", 10, FontStyle.Bold),
                            Location = new Point(10, 135),
                            AutoSize = true
                        };

                        Label lblInfo = new Label
                        {
                            Text = $"Tahun: {tahun} | Plat: {noPlat}",
                            Location = new Point(10, 160),
                            AutoSize = true
                        };

                        Label lblHarga = new Label
                        {
                            Text = $"Rp {harga:N0}/hari",
                            ForeColor = Color.DarkGreen,
                            Font = new Font("Segoe UI", 9, FontStyle.Bold),
                            Location = new Point(10, 185),
                            AutoSize = true
                        };

                        Label lblStatus = new Label
                        {
                            Text = $"Status: {status}",
                            Location = new Point(10, 205),
                            AutoSize = true
                        };

                        Button btnEdit = new Button
                        {
                            Text = "Edit",
                            Width = 70,
                            Location = new Point(10, 235),
                            BackColor = Color.LightBlue
                        };
                        btnEdit.Click += (s, e) =>
                        {
                            FormEditMobil editForm = new FormEditMobil(
                                id, merk, model, tahun, noPlat, harga.ToString(), foto
                            );

                            if (editForm.ShowDialog() == DialogResult.OK)
                            {
                                LoadDataMobil(keyword);
                            }
                        };

                        Button btnDelete = new Button
                        {
                            Text = "Delete",
                            Width = 70,
                            Location = new Point(100, 235),
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

                        card.Controls.Add(picMobil);
                        card.Controls.Add(lblTitle);
                        card.Controls.Add(lblInfo);
                        card.Controls.Add(lblHarga);
                        card.Controls.Add(lblStatus);
                        card.Controls.Add(btnEdit);
                        card.Controls.Add(btnDelete);

                        // TOMBOL SERVIS - KONDISIONAL
                        if (status == "tersedia")
                        {
                            Button btnServis = new Button
                            {
                                Text = "Servis",
                                Width = 160,
                                Cursor = Cursors.Hand,
                                Location = new Point(10, 265),
                                BackColor = Color.Orange,
                                ForeColor = Color.White,
                                Font = new Font("Segoe UI", 9, FontStyle.Bold)
                            };
                            btnServis.Click += (s, e) =>
                            {
                                var confirm = MessageBox.Show(
                                    $"Ubah status mobil {merk} {model} menjadi 'servis'?",
                                    "Konfirmasi Servis",
                                    MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Question);

                                if (confirm == DialogResult.Yes)
                                {
                                    UpdateStatusMobil(id, "servis");
                                    LoadDataMobil(keyword);
                                }
                            };
                            card.Controls.Add(btnServis);
                        }
                        else if (status == "servis")
                        {
                            Button btnSelesaiServis = new Button
                            {
                                Text = "Selesai Servis",
                                Width = 160,
                                Cursor = Cursors.Hand,
                                Location = new Point(10, 265),
                                BackColor = Color.Green,
                                ForeColor = Color.White,
                                Font = new Font("Segoe UI", 9, FontStyle.Bold)
                            };
                            btnSelesaiServis.Click += (s, e) =>
                            {
                                var confirm = MessageBox.Show(
                                    $"Ubah status mobil {merk} {model} menjadi 'Tersedia'?",
                                    "Konfirmasi Selesai Servis",
                                    MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Question);

                                if (confirm == DialogResult.Yes)
                                {
                                    UpdateStatusMobil(id, "tersedia");
                                    LoadDataMobil(keyword);
                                }
                            };
                            card.Controls.Add(btnSelesaiServis);
                        }

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
                    string query = "INSERT INTO mobil (merk, model, tahun, no_plat, harga_sewa_perhari, status, foto) " +
                                   "VALUES (@merk, @model, @tahun, @no_plat, @harga_sewa_perhari, @status, @foto)";

                    using (var conn = new MySqlConnection(connStr))
                    {
                        conn.Open();
                        using (var cmd = new MySqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@merk", form.merk);
                            cmd.Parameters.AddWithValue("@model", form.model);
                            cmd.Parameters.AddWithValue("@tahun", form.tahun);
                            cmd.Parameters.AddWithValue("@no_plat", form.no_plat);
                            cmd.Parameters.AddWithValue("@harga_sewa_perhari", form.harga_sewa_perhari);
                            cmd.Parameters.AddWithValue("@status", "tersedia");
                            cmd.Parameters.AddWithValue("@foto", (object)form.foto_path ?? DBNull.Value);

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
            this.Hide();
        }

        private void label2_Click(object sender, EventArgs e)
        {
            DashboardAdmin DashboardForm = new DashboardAdmin(username);
            DashboardForm.Show();
            this.Hide();
        }

        private void label5_Click(object sender, EventArgs e)
        {
            RiwayatAdmin PelangganForm = new RiwayatAdmin(username);
            PelangganForm.Show();
            this.Hide();
        }

        private void label4_Click(object sender, EventArgs e)
        {
            PelangganAdmin pelangganForm = new PelangganAdmin(username);
            pelangganForm.Show();
            this.Hide();
        }

        private void panel13_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}