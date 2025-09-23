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

namespace rental_mobiV2
{
    public partial class KelolaUserAdmin : Form
    {
        private string username;
        private string connStr = "server=localhost;user=root;password=;database=rental_mobil;";
        public KelolaUserAdmin(string uname)
        {
            InitializeComponent();
            username = uname;

            // contoh label manual
            Label lbl = new Label();
            lbl.Text = "Selamat Datang Admin: " + username;
            lbl.AutoSize = true;
            lbl.Location = new System.Drawing.Point(20, 20);
            this.Controls.Add(lbl);
        }

        private void KelolaUserAdmin_Load(object sender, EventArgs e)
        {
            // isi combobox
            cmbAddRole.Items.Clear();
            cmbAddRole.Items.Add("admin");
            cmbAddRole.Items.Add("kasir");
            cmbAddRole.SelectedIndex = 0;
            LoadUsers();
        }
        private void LoadUsers()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    conn.Open();
                    string query = "SELECT id, username, password, role FROM users";
                    MySqlDataAdapter da = new MySqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // reset tabel
                    dataGridView1.Columns.Clear();

                    // Data tampilan
                    DataTable dtDisplay = new DataTable();
                    dtDisplay.Columns.Add("No", typeof(int));
                    dtDisplay.Columns.Add("id", typeof(int));
                    dtDisplay.Columns.Add("username", typeof(string));
                    dtDisplay.Columns.Add("password", typeof(string));
                    dtDisplay.Columns.Add("role", typeof(string));

                    int counter = 1;
                    foreach (DataRow row in dt.Rows)
                    {
                        dtDisplay.Rows.Add(counter, row["id"], row["username"], row["password"], row["role"]);
                        counter++;
                    }

                    dataGridView1.DataSource = dtDisplay;

                    // Tambah tombol Edit
                    if (!dataGridView1.Columns.Contains("colEdit"))
                    {
                        DataGridViewButtonColumn colEdit = new DataGridViewButtonColumn();
                        colEdit.Name = "colEdit";
                        colEdit.HeaderText = "Edit";
                        colEdit.Text = "Edit";
                        colEdit.UseColumnTextForButtonValue = true;
                        dataGridView1.Columns.Add(colEdit);
                    }

                    // Tambah tombol Delete
                    if (!dataGridView1.Columns.Contains("colDelete"))
                    {
                        DataGridViewButtonColumn colDelete = new DataGridViewButtonColumn();
                        colDelete.Name = "colDelete";
                        colDelete.HeaderText = "Delete";
                        colDelete.Text = "Delete";
                        colDelete.UseColumnTextForButtonValue = true;
                        dataGridView1.Columns.Add(colDelete);
                    }

                    // Styling
                    dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    dataGridView1.AllowUserToAddRows = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error load users: " + ex.Message);
            }
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
        private void HapusUser(string id)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    conn.Open();
                    string query = "DELETE FROM users WHERE id=@id";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("User berhasil dihapus!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error hapus user: " + ex.Message);
            }
        }
        private void ClearForm()
        {
            txtAddUsername.Text = "";
            txtAddPassword.Text = "";
        }

        private void btnTambah_Click(object sender, EventArgs e)
        {
            string username = txtAddUsername.Text.Trim();
            string password = txtAddPassword.Text.Trim();
            string role = cmbAddRole.SelectedItem?.ToString() ?? "kasir";

            if (username == "" || password == "")
            {
                MessageBox.Show("Username & password wajib diisi!");
                return;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    conn.Open();
                    string query = "INSERT INTO users (username, password, role) VALUES (@username, @password, @role)";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password);
                    cmd.Parameters.AddWithValue("@role", role);
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("User berhasil ditambahkan!");
                LoadUsers();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error tambah user: " + ex.Message);
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string colName = dataGridView1.Columns[e.ColumnIndex].Name;

            if (colName == "colEdit")
            {
                // ambil data row
                string selectedId = dataGridView1.Rows[e.RowIndex].Cells["id"].Value?.ToString() ?? "";
                string selectedUsername = dataGridView1.Rows[e.RowIndex].Cells["username"].Value?.ToString() ?? "";
                string selectedPassword = dataGridView1.Rows[e.RowIndex].Cells["password"].Value?.ToString() ?? "";
                string selectedRole = dataGridView1.Rows[e.RowIndex].Cells["role"].Value?.ToString() ?? "";

                // buka modal edit
                FormEditUser frm = new FormEditUser(selectedId, selectedUsername, selectedPassword, selectedRole);
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    LoadUsers();
                }
            }
            else if (colName == "colDelete")
            {
                string selectedId = dataGridView1.Rows[e.RowIndex].Cells["id"].Value?.ToString() ?? "";
                DialogResult result = MessageBox.Show("Yakin hapus user ini?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    HapusUser(selectedId);
                    LoadUsers();
                }
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {
            DashboardAdmin DashboardForm = new DashboardAdmin(username);
            DashboardForm.Show();
            this.Hide(); // sembunyiin form sekarang
        }

        private void label3_Click(object sender, EventArgs e)
        {
            DataMobilAdmin mobilForm = new DataMobilAdmin(username);
            mobilForm.Show();
            this.Hide(); // sembunyiin form sekarang
        }

        private void label10_Click(object sender, EventArgs e)
        {

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
    }
}
