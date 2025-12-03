using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace rental_mobiV2
{
    public partial class FormEditMobil : Form
    {
        private string connStr = "server=localhost;user=root;password=;database=rental_mobil;";
        private string mobilId;
        private string foto_lama;
        private string foto_baru;

        private PictureBox picPreview;
        private Button btnPilihFoto;
        private Button btnHapusFoto;

        public FormEditMobil(string id, string merk, string model, string tahun, string no_plat, string harga_sewa_perhari, string foto = "")
        {
            InitializeComponent();
            mobilId = id;
            foto_lama = foto;

            // Isi textbox dengan data lama
            txtMerk.Text = merk;
            txtModel.Text = model;
            txtTahun.Text = tahun;
            txtNoPlat.Text = no_plat;
            txtHarga.Text = harga_sewa_perhari;

            InitializeFotoComponents();
            LoadFotoLama();
        }

        private void InitializeFotoComponents()
        {
            // PictureBox untuk preview foto
            picPreview = new PictureBox
            {
                Name = "picPreview",
                Size = new Size(220, 180), // sedikit lebih besar biar proporsional
                Location = new Point(30, 60), // sejajar di bawah header oranye
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.LightGray
            };

            // Tombol Ganti Foto
            btnPilihFoto = new Button
            {
                Name = "btnPilihFoto",
                Text = "Ganti Foto",
                Size = new Size(100, 35),
                Location = new Point(30, 250), // di bawah picPreview
                BackColor = Color.WhiteSmoke,
                FlatStyle = FlatStyle.Flat
            };
            btnPilihFoto.Click += BtnPilihFoto_Click;

            // Tombol Hapus Foto
            btnHapusFoto = new Button
            {
                Name = "btnHapusFoto",
                Text = "Hapus Foto",
                Size = new Size(100, 35),
                Location = new Point(150, 250), // sejajar di samping tombol Ganti
                BackColor = Color.WhiteSmoke,
                FlatStyle = FlatStyle.Flat
            };
            btnHapusFoto.Click += BtnHapusFoto_Click;

            // Tambahkan ke form
            this.Controls.Add(picPreview);
            this.Controls.Add(btnPilihFoto);
            this.Controls.Add(btnHapusFoto);
        }

        private void LoadFotoLama()
        {
            if (!string.IsNullOrEmpty(foto_lama) && File.Exists(foto_lama))
            {
                try
                {
                    picPreview.Image = Image.FromFile(foto_lama);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading foto: " + ex.Message);
                }
            }
            else
            {
                picPreview.Image = null;
            }
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

                        // Preview foto baru
                        picPreview.Image = Image.FromFile(ofd.FileName);
                        foto_baru = ofd.FileName;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error membaca foto: " + ex.Message);
                    }
                }
            }
        }

        private void BtnHapusFoto_Click(object sender, EventArgs e)
        {
            if (picPreview.Image != null)
            {
                picPreview.Image.Dispose();
                picPreview.Image = null;
            }
            foto_baru = "HAPUS"; // flag untuk hapus foto
        }

        private void FormEditMobil_Load(object sender, EventArgs e)
        {
        }

        private void btnSimpan_Click(object sender, EventArgs e)
        {
            try
            {
                string fotoPath = foto_lama; // default pake foto lama

                // Proses foto baru jika ada
                if (!string.IsNullOrEmpty(foto_baru))
                {
                    if (foto_baru == "HAPUS")
                    {
                        // Hapus foto lama dari disk
                        if (!string.IsNullOrEmpty(foto_lama) && File.Exists(foto_lama))
                        {
                            try
                            {
                                File.Delete(foto_lama);
                            }
                            catch { }
                        }
                        fotoPath = null;
                    }
                    else
                    {
                        // Copy foto baru
                        string folder = Path.Combine(Application.StartupPath, "FotoMobil");
                        if (!Directory.Exists(folder))
                            Directory.CreateDirectory(folder);

                        string ext = Path.GetExtension(foto_baru);
                        string namaFile = $"{txtMerk.Text}_{txtModel.Text}_{DateTime.Now:yyyyMMddHHmmss}{ext}";
                        string destPath = Path.Combine(folder, namaFile);

                        File.Copy(foto_baru, destPath, true);
                        fotoPath = destPath;

                        // Hapus foto lama jika ada
                        if (!string.IsNullOrEmpty(foto_lama) && File.Exists(foto_lama) && foto_lama != destPath)
                        {
                            try
                            {
                                File.Delete(foto_lama);
                            }
                            catch { }
                        }
                    }
                }

                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    conn.Open();
                    string query = @"UPDATE mobil
                         SET merk=@merk, model=@model, tahun=@tahun,
                             no_plat=@no_plat, harga_sewa_perhari=@harga_sewa_perhari,
                             foto=@foto
                         WHERE id=@id";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@merk", txtMerk.Text);
                    cmd.Parameters.AddWithValue("@model", txtModel.Text);
                    cmd.Parameters.AddWithValue("@tahun", txtTahun.Text);
                    cmd.Parameters.AddWithValue("@no_plat", txtNoPlat.Text);

                    decimal harga = decimal.Parse(txtHarga.Text,
                        System.Globalization.CultureInfo.GetCultureInfo("id-ID"));
                    cmd.Parameters.AddWithValue("@harga_sewa_perhari", harga);
                    cmd.Parameters.AddWithValue("@foto", (object)fotoPath ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@id", mobilId);

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Data mobil berhasil diperbarui!");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error update mobil: " + ex.Message);
            }
        }

        private void btnBatal_Click(object sender, EventArgs e)
        {
            // Dispose image sebelum close
            if (picPreview.Image != null)
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