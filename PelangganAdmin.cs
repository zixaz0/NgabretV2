using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows.Forms;

namespace rental_mobiV2
{
    public partial class PelangganAdmin : Form
    {
        private string connStr = "server=localhost;user=root;password=;database=rental_mobil;";
        private string username;

        public PelangganAdmin(string uname)
        {
            InitializeComponent();
            username = uname;
        }

        private void label1_Click(object sender, EventArgs e)
        {
            KelolaUserAdmin KasirForm = new KelolaUserAdmin(username);
            KasirForm.Show();
            this.Hide(); // sembunyiin form sekarang
        }

        private void label3_Click(object sender, EventArgs e)
        {
            DataMobilAdmin mobilForm = new DataMobilAdmin(username);
            mobilForm.Show();
            this.Hide(); // sembunyiin form sekarang
        }

        private void label5_Click(object sender, EventArgs e)
        {
            RiwayatAdmin PelangganForm = new RiwayatAdmin(username);
            PelangganForm.Show();
            this.Hide(); // sembunyiin form sekarang
        }

        private void label2_Click(object sender, EventArgs e)
        {
            DashboardAdmin DashboardForm = new DashboardAdmin(username);
            DashboardForm.Show();
            this.Hide(); // sembunyiin form sekarang
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

        private void PelangganAdmin_Load(object sender, EventArgs e)
        {
            LoadPelanggan();
        }

        private void LoadPelanggan()
        {
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    string query = @"SELECT
                                        id AS 'ID',
                                        nama AS 'Nama',
                                        alamat AS 'Alamat',
                                        no_telepon AS 'No Telepon',
                                        no_ktp AS 'No KTP',
                                        status AS 'Status',
                                        email AS 'Email'
                                     FROM pelanggan";
                    MySqlDataAdapter da = new MySqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dgvPelanggan.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error load pelanggan: " + ex.Message);
                }
            }
        }

        private void dgvPelanggan_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
          
        }
    }
}