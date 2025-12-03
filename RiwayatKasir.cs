using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

namespace rental_mobiV2
{
    public partial class RiwayatKasir : Form
    {
        private string connStr = "server=localhost;user=root;password=;database=rental_mobil;";
        private string username;
        public RiwayatKasir(string uname)
        {
            InitializeComponent();
            username = uname;
        }

        private void RiwayatKasir_Load(object sender, EventArgs e)
        {
            LoadRiwayat();
        }
        private void LoadRiwayat(string keyword = "", DateTime? tanggalAwal = null, DateTime? tanggalAkhir = null)
        {
            string connectionString = "server=localhost;database=rental_mobil;uid=root;pwd=;";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string query = @"SELECT 
                                t.id AS 'ID Transaksi',
                                p.nama AS 'Nama Pelanggan',
                                m.merk AS 'Merk Mobil',
                                m.model AS 'Model Mobil',
                                t.tanggal_sewa AS 'Tanggal Sewa',
                                t.tanggal_selesai AS 'Tanggal Selesai',
                                t.tanggal_kembali AS 'Tanggal Kembali',
                                t.total_harga AS 'Total Harga',
                                t.denda AS 'Denda'
                             FROM transaksi t
                             JOIN pelanggan p ON t.id_pelanggan = p.id
                             JOIN mobil m ON t.id_mobil = m.id
                             WHERE 1=1"; // supaya gampang tambah kondisi

                    // Filter keyword
                    if (!string.IsNullOrEmpty(keyword))
                    {
                        query += @" AND (p.nama LIKE @keyword 
                            OR m.merk LIKE @keyword 
                            OR m.model LIKE @keyword)";
                    }

                    // Filter tanggal (misalnya pakai tanggal_sewa)
                    if (tanggalAwal.HasValue && tanggalAkhir.HasValue)
                    {
                        query += " AND t.tanggal_sewa BETWEEN @awal AND @akhir";
                    }

                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    if (!string.IsNullOrEmpty(keyword))
                    {
                        cmd.Parameters.AddWithValue("@keyword", "%" + keyword + "%");
                    }

                    if (tanggalAwal.HasValue && tanggalAkhir.HasValue)
                    {
                        // Format ke yyyy-MM-dd biar aman
                        cmd.Parameters.AddWithValue("@awal", tanggalAwal.Value.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@akhir", tanggalAkhir.Value.ToString("yyyy-MM-dd"));
                    }

                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dgvTransaksiTerbaru.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            LoadRiwayat(txtSearch.Text.Trim());
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadRiwayat(txtSearch.Text.Trim());
        }

        private void dtAwal_ValueChanged(object sender, EventArgs e)
        {
            LoadRiwayat(txtSearch.Text.Trim(), dtAwal.Value, dtAkhir.Value);
        }

        private void dtAkhir_ValueChanged(object sender, EventArgs e)
        {
            LoadRiwayat(txtSearch.Text.Trim(), dtAwal.Value, dtAkhir.Value);
        }

        private void btnKembali_Click(object sender, EventArgs e)
        {
            DashboardKasir dashboard = new DashboardKasir(username);
            dashboard.Show();
            this.Hide();
        }

        private void txtSearch_TextChanged_1(object sender, EventArgs e)
        {

        }
    }
}
