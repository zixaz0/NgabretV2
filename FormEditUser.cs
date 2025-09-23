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
    public partial class FormEditUser : Form
    {
        private string connStr = "server=localhost;user=root;password=;database=rental_mobil;";
        public string UserId { get; set; }
        public FormEditUser(string id, string username, string password, string role)
        {
            InitializeComponent();

            UserId = id;
            txtEditUsername.Text = username;
            txtEditPassword.Text = password;
            cmbEditRole.Items.Add("admin");
            cmbEditRole.Items.Add("kasir");
            cmbEditRole.SelectedItem = role;
        }

        private void FormEditUser_Load(object sender, EventArgs e)
        {

        }

        private void btnSimpanEdit_Click(object sender, EventArgs e)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    conn.Open();
                    string query = "UPDATE users SET username=@username, password=@password, role=@role WHERE id=@id";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@username", txtEditUsername.Text.Trim());
                    cmd.Parameters.AddWithValue("@password", txtEditPassword.Text.Trim());
                    cmd.Parameters.AddWithValue("@role", cmbEditRole.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@id", UserId);
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("User berhasil diupdate!");
                this.DialogResult = DialogResult.OK; // biar form utama tahu sukses
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error update user: " + ex.Message);
            }
        }

        private void btnBatal_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
    }
