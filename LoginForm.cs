using MySql.Data.MySqlClient;
using System;
using System.Windows.Forms;

namespace rental_mobiV2
{
    public partial class LoginForm : Form
    {
        private MySqlConnection conn = new MySqlConnection(
           "Server=localhost;Database=rental_mobil;Uid=root;Pwd=;"
       );
        public LoginForm()
        {
            InitializeComponent();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                textBox2.UseSystemPasswordChar = true;
            }
            else
            {
                textBox2.UseSystemPasswordChar = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                conn.Open();
                string query = "SELECT * FROM users WHERE username=@username AND password=@password";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@username", textBox1.Text);
                cmd.Parameters.AddWithValue("@password", textBox2.Text);

                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    string role = reader["role"].ToString();

                    if (role == "admin")
                    {
                        MessageBox.Show("Login berhasil sebagai Admin!");
                        DashboardAdmin adminForm = new DashboardAdmin(textBox1.Text);
                        adminForm.Show();
                        this.Hide();
                    }
                    else if (role == "kasir")
                    {
                        MessageBox.Show("Login berhasil sebagai Kasir!");
                        DashboardKasir kasirForm = new DashboardKasir(textBox1.Text);
                        kasirForm.Show();
                        this.Hide();
                    }
                }
                else
                {
                    MessageBox.Show("Username atau password salah!");
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}