using System;
using System.IO;
using System.Drawing;
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
        public string foto_path { get; set; }

        private PictureBox picPreview;
        private Button btnPilihFoto;

        public FormTambahMobil()
        {
            InitializeComponent();
            InitializeFotoComponents();
        }

        private void InitializeFotoComponents()
        {
            // PictureBox untuk preview foto
            picPreview = new PictureBox
            {
                Name = "picPreview",
                Size = new Size(200, 150),
                Location = new Point(20, 70), // geser biar sejajar form
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.LightGray
            };

            // Button untuk pilih foto
            btnPilihFoto = new Button
            {
                Name = "btnPilihFoto",
                Text = "Pilih Foto",
                Size = new Size(100, 30),
                Location = new Point(20, 230)
            };
            btnPilihFoto.Click += BtnPilihFoto_Click;

            //tambahkan ke panel2, bukan ke form
            panel2.Controls.Add(picPreview);
            panel2.Controls.Add(btnPilihFoto);
        }


        private void BtnPilihFoto_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                ofd.Title = "Pilih Foto Mobil";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Dispose image lama biar ga lock file
                        if (picPreview.Image != null)
                        {
                            picPreview.Image.Dispose();
                        }

                        // Preview foto
                        picPreview.Image = Image.FromFile(ofd.FileName);

                        // Simpan path sementara
                        foto_path = ofd.FileName;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error membaca foto: " + ex.Message);
                    }
                }
            }
        }

        private void FormTambahMobil_Load(object sender, EventArgs e)
        {
        }

        private void btnSimpan_Click(object sender, EventArgs e)
        {
            try
            {
                // Validasi input
                if (string.IsNullOrWhiteSpace(txtMerk.Text) ||
                    string.IsNullOrWhiteSpace(txtModel.Text) ||
                    string.IsNullOrWhiteSpace(txtTahun.Text) ||
                    string.IsNullOrWhiteSpace(txtPlat.Text) ||
                    string.IsNullOrWhiteSpace(txtHarga.Text))
                {
                    MessageBox.Show("Semua field harus diisi!");
                    return;
                }

                // Ambil nilai dari textbox
                merk = txtMerk.Text;
                model = txtModel.Text;
                tahun = int.Parse(txtTahun.Text);
                no_plat = txtPlat.Text;
                harga_sewa_perhari = decimal.Parse(txtHarga.Text);

                // Copy foto ke folder project jika ada
                if (!string.IsNullOrEmpty(foto_path) && File.Exists(foto_path))
                {
                    string folder = Path.Combine(Application.StartupPath, "FotoMobil");

                    // Buat folder kalau belum ada
                    if (!Directory.Exists(folder))
                        Directory.CreateDirectory(folder);

                    // Generate nama file unik
                    string ext = Path.GetExtension(foto_path);
                    string namaFile = $"{merk}_{model}_{DateTime.Now:yyyyMMddHHmmss}{ext}";
                    string destPath = Path.Combine(folder, namaFile);

                    // Copy file
                    File.Copy(foto_path, destPath, true);

                    // Simpan path relatif ke database
                    foto_path = destPath;
                }

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
            // Dispose image sebelum close
            if (picPreview != null && picPreview.Image != null)
            {
                picPreview.Image.Dispose();
            }

            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnBatal_Click_1(object sender, EventArgs e)
        {
            // Dispose image sebelum close
            if (picPreview != null && picPreview.Image != null)
            {
                picPreview.Image.Dispose();
            }

            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }
    }
}