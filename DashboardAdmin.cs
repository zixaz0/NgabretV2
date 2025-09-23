using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Data;

namespace rental_mobiV2
{
    public partial class DashboardAdmin : Form
    {
        private string connStr = "server=localhost;user=root;password=;database=rental_mobil;";
        private string username;
        public DashboardAdmin(string uname)
        {
            InitializeComponent();
            username = uname;
        }
        private void panel2_Paint(object sender, PaintEventArgs e)
        {
        }

        private void label1_Click(object sender, EventArgs e)
        {
            KelolaUserAdmin KasirForm = new KelolaUserAdmin(username);
            KasirForm.Show();
            this.Hide(); // sembunyiin form sekarang
        }

        private void panel7_Paint(object sender, PaintEventArgs e)
        {
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
        private int GetCount(string query)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    object result = cmd.ExecuteScalar(); // ambil nilai tunggal
                    return Convert.ToInt32(result);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error load data: " + ex.Message);
                return 0;
            }
        }

        private void DashboardAdmin_Load(object sender, EventArgs e)
        {
            LoadTransaksiTerbaru();
            // hitung jumlah user
            int jumlahUser = GetCount("SELECT COUNT(*) FROM users");
            lblJumlahUser.Text = $"{jumlahUser}";
            // hitung jumlah mobil
            int jumlahMobil = GetCount("SELECT COUNT(*) FROM mobil");
            lblJumlahMobil.Text = $"{jumlahMobil}";
            // hitung jumlah pelanggan
            int jumlahPelanggan = GetCount("SELECT COUNT(*) FROM pelanggan");
            lblJumlahPelanggan.Text = $"{jumlahPelanggan}";
        }
        private void LoadTransaksiTerbaru()
        {
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    conn.Open();

                    string query = @"SELECT 
                                p.nama AS 'Pelanggan',
                                t.tanggal_kembali AS 'Tanggal Kembali',
                                m.model AS 'Model Mobil',
                                t.total_harga AS 'Total Harga',
                                t.denda AS 'Denda'
                             FROM transaksi t
                             JOIN pelanggan p ON t.id_pelanggan = p.id
                             JOIN mobil m ON t.id_mobil = m.id
                             ORDER BY t.created_at DESC
                             LIMIT 10";

                    MySqlDataAdapter da = new MySqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // Tambah kolom "No" manual
                    dt.Columns.Add("No", typeof(int));

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dt.Rows[i]["No"] = i + 1; // nomor urut mulai dari 1
                    }

                    // Pindahkan kolom "No" ke paling depan
                    dt.Columns["No"].SetOrdinal(0);

                    dgvTransaksiTerbaru.DataSource = dt;

                    // Format kolom tanggal
                    if (dgvTransaksiTerbaru.Columns["Tanggal Kembali"] != null)
                    {
                        dgvTransaksiTerbaru.Columns["Tanggal Kembali"].DefaultCellStyle.Format = "dd-MM-yyyy";
                    }

                    // Format angka jadi ribuan pakai titik
                    if (dgvTransaksiTerbaru.Columns["Total Harga"] != null)
                    {
                        dgvTransaksiTerbaru.Columns["Total Harga"].DefaultCellStyle.Format = "N0";
                    }
                    if (dgvTransaksiTerbaru.Columns["Denda"] != null)
                    {
                        dgvTransaksiTerbaru.Columns["Denda"].DefaultCellStyle.Format = "N0";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error saat load transaksi terbaru: " + ex.Message);
                }
            }
        }


        private void label7_Click(object sender, EventArgs e)
        {
            DataMobilAdmin mobilForm = new DataMobilAdmin(username);
            mobilForm.Show();
            this.Hide(); // sembunyiin form sekarang
        }

        private void label6_Click(object sender, EventArgs e)
        {
            KelolaUserAdmin KasirForm = new KelolaUserAdmin(username);
            KasirForm.Show();
            this.Hide(); // sembunyiin form sekarang
        }

        private void lblJumlahPelanggan_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {
            DataMobilAdmin mobilForm = new DataMobilAdmin(username);
            mobilForm.Show();
            this.Hide(); // sembunyiin form sekarang
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {
            RiwayatAdmin PelangganForm = new RiwayatAdmin(username);
            PelangganForm.Show();
            this.Hide(); // sembunyiin form sekarang
        }

        private void label12_Click(object sender, EventArgs e)
        {
            RiwayatAdmin PelangganForm = new RiwayatAdmin(username);
            PelangganForm.Show();
            this.Hide(); // sembunyiin form sekarang
        }
    }
}