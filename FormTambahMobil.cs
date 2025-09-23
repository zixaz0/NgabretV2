using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace rental_mobiV2
{
    public partial class FormTambahMobil : Form
    {
        public string merk { get; set; }
        public string model { get; set; }
        public int tahun { get; set; }
        public string no_plat { get; set; }
        public decimal harga_sewa_perhari { get; set; }
        public string Status { get; set; } = "tersedia";
        public FormTambahMobil()
        {
            InitializeComponent();
        }

        private void FormTambahMobil_Load(object sender, EventArgs e)
        {

        }

        private void btnSimpan_Click(object sender, EventArgs e)
        {
            try
            {
                // Ambil nilai dari textbox
                merk = txtMerk.Text;
                model = txtModel.Text;
                tahun = int.Parse(txtTahun.Text);
                no_plat = txtPlat.Text;
                harga_sewa_perhari = decimal.Parse(txtHarga.Text);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Input tidak valid: " + ex.Message);
            }
        }

        private void btnBatal_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnBatal_Click_1(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
    }

