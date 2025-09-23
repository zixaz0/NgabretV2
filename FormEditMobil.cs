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
    public partial class FormEditMobil : Form
    {
        private string connStr = "server=localhost;user=root;password=;database=rental_mobil;";
        private string mobilId;
        public FormEditMobil(string id, string merk, string model, string tahun, string no_plat, string harga_sewa_perhari)
        {
            InitializeComponent();
            mobilId = id;

            // Isi textbox dengan data lama
            txtMerk.Text = merk;
            txtModel.Text = model;
            txtTahun.Text = tahun;
            txtNoPlat.Text = no_plat;
            txtHarga.Text = harga_sewa_perhari;
        }

        private void FormEditMobil_Load(object sender, EventArgs e)
        {

        }

        private void btnSimpan_Click(object sender, EventArgs e)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    conn.Open();
                    string query = @"UPDATE mobil
                         SET merk=@merk, model=@model, tahun=@tahun,
                             no_plat=@no_plat, harga_sewa_perhari=@harga_sewa_perhari
                         WHERE id=@id";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@merk", txtMerk.Text);
                    cmd.Parameters.AddWithValue("@model", txtModel.Text);
                    cmd.Parameters.AddWithValue("@tahun", txtTahun.Text);
                    cmd.Parameters.AddWithValue("@no_plat", txtNoPlat.Text);
                    decimal harga = decimal.Parse(txtHarga.Text,
                        System.Globalization.CultureInfo.GetCultureInfo("id-ID"));
                    cmd.Parameters.AddWithValue("@harga_sewa_perhari", harga);
                    cmd.Parameters.AddWithValue("@id", mobilId);

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Data mobil berhasil diperbarui!");
                this.DialogResult = DialogResult.OK; // biar parent tau sukses
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error update mobil: " + ex.Message);
            }
        }

        private void btnBatal_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
