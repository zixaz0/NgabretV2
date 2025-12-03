using MySql.Data.MySqlClient;
using System;
using System.Drawing;
using System.Net;
using System.Net.Mail;
using System.Windows.Forms;

namespace rental_mobiV2
{
    public partial class FormTransaksi : Form
    {
        private string connectionString = "server=localhost;uid=root;pwd=;database=rental_mobil;";
        private string idMobilDipilih;

        public FormTransaksi(string idMobil)
        {
            InitializeComponent();
            idMobilDipilih = idMobil;
        }

        private void FormTransaksi_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(idMobilDipilih))
            {
                LoadDataMobil(idMobilDipilih);
            }

            dtpTanggalSewa.MinDate = DateTime.Today;
            dtpTanggalSelesai.MinDate = DateTime.Today;
        }

        private void LoadDataMobil(string id)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM mobil WHERE id = @id";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);

                MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    lblMerkMobil.Text = reader["merk"].ToString();
                    lblModelMobil.Text = reader["model"].ToString();
                    lblThnMobil.Text = reader["tahun"].ToString();
                    lblNoPlatMobil.Text = reader["no_plat"].ToString();
                    lblHargaMobil.Text = reader["harga_sewa_perhari"].ToString();

                    string foto = reader["foto"].ToString();
                    if (System.IO.File.Exists(foto))
                        picMobil.Image = Image.FromFile(foto);
                }

                reader.Close();
            }
        }

        private void KirimEmailTransaksi(
            string emailPelanggan,
            string namaPelanggan,
            string merkMobil,
            string modelMobil,
            DateTime tglSewa,
            DateTime tglSelesai,
            decimal totalHarga)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("emailkamu@gmail.com", "Ngabret.id");
                mail.To.Add(emailPelanggan);
                mail.Subject = "Konfirmasi Transaksi Rental Mobil";

                mail.Body =
                    $"Halo Pelanggan terhormat {namaPelanggan},\n\n" +
                    $"Terima kasih telah menyewa mobil di tempat kami.\n\n" +
                    $"Detail Transaksi:\n" +
                    $"Mobil: {merkMobil} {modelMobil}\n" +
                    $"Tanggal Sewa: {tglSewa:dd-MM-yyyy}\n" +
                    $"Tanggal Selesai: {tglSelesai:dd-MM-yyyy}\n" +
                    $"Total Harga: Rp {totalHarga:N0}\n\n" +
                    $"Jika ada pertanyaan silakan hubungi kami.\n\n" +
                    $"Hormat kami,\nNgabret.id";

                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.EnableSsl = true;

                // ⚠ Pakai App Password Gmail
                smtp.Credentials = new NetworkCredential("ngabret.id30@gmail.com", "tdvt vhqi kfew mmje");

                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Email gagal dikirim: " + ex.Message);
            }
        }

        private void btnLanjut_Click(object sender, EventArgs e)
        {
            // ✅ Validasi input pelanggan
            if (string.IsNullOrWhiteSpace(txtNamaPelanggan.Text) ||
                string.IsNullOrWhiteSpace(txtKTP.Text) ||
                string.IsNullOrWhiteSpace(txtTelepon.Text))
            {
                MessageBox.Show("Mohon lengkapi data pelanggan!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // ✅ Validasi tanggal sewa
            if (dtpTanggalSewa.Value.Date > dtpTanggalSelesai.Value.Date)
            {
                MessageBox.Show("Tanggal selesai tidak boleh sebelum tanggal sewa!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // ✅Gunakan transaksi MySQL agar data aman
                    using (MySqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // 1️⃣ Insert data pelanggan baru
                            string queryPelanggan = @"INSERT INTO pelanggan
                                (nama, alamat, no_telepon, no_ktp, email, created_at, status)
                                VALUES (@nama, @alamat, @no_telepon, @no_ktp, @email, NOW(), 'tersedia')";
                            MySqlCommand cmdPelanggan = new MySqlCommand(queryPelanggan, conn, transaction);
                            cmdPelanggan.Parameters.AddWithValue("@nama", txtNamaPelanggan.Text);
                            cmdPelanggan.Parameters.AddWithValue("@alamat", txtAlamat.Text);
                            cmdPelanggan.Parameters.AddWithValue("@no_telepon", txtTelepon.Text);
                            cmdPelanggan.Parameters.AddWithValue("@no_ktp", txtKTP.Text);
                            cmdPelanggan.Parameters.AddWithValue("@email", txtEmail.Text);
                            cmdPelanggan.ExecuteNonQuery();

                            long idPelangganBaru = cmdPelanggan.LastInsertedId;

                            // 2️⃣ Ambil harga sewa dari label
                            decimal hargaPerHari = Convert.ToDecimal(lblHargaMobil.Text);

                            // Hitung total harga
                            TimeSpan durasi = dtpTanggalSelesai.Value - dtpTanggalSewa.Value;

                            // Total jam dibagi 24, minimal 1 hari
                            int jumlahHari = (int)Math.Ceiling(durasi.TotalHours / 24);

                            // Biar gak nol
                            if (jumlahHari < 1)
                                jumlahHari = 1;
                            decimal totalHarga = hargaPerHari * jumlahHari;

                            // 3️⃣ Simpan ke tabel transaksi
                            string queryTransaksi = @"INSERT INTO transaksi
                                (id_pelanggan, id_mobil, tanggal_sewa, tanggal_selesai, total_harga, created_at, tanggal_kembali, denda)
                                VALUES (@id_pelanggan, @id_mobil, @tanggal_sewa, @tanggal_selesai, @total_harga, NOW(), NULL, 0)";
                            MySqlCommand cmdTransaksi = new MySqlCommand(queryTransaksi, conn, transaction);
                            cmdTransaksi.Parameters.AddWithValue("@id_pelanggan", idPelangganBaru);
                            cmdTransaksi.Parameters.AddWithValue("@id_mobil", idMobilDipilih);
                            cmdTransaksi.Parameters.AddWithValue("@tanggal_sewa", dtpTanggalSewa.Value.Date);
                            cmdTransaksi.Parameters.AddWithValue("@tanggal_selesai", dtpTanggalSelesai.Value.Date);
                            cmdTransaksi.Parameters.AddWithValue("@total_harga", totalHarga);
                            cmdTransaksi.ExecuteNonQuery();

                            // 4️⃣ Update status mobil jadi 'disewa'
                            string updateMobil = "UPDATE mobil SET status='disewa' WHERE id=@id";
                            MySqlCommand cmdUpdate = new MySqlCommand(updateMobil, conn, transaction);
                            cmdUpdate.Parameters.AddWithValue("@id", idMobilDipilih);
                            cmdUpdate.ExecuteNonQuery();

                            // Commit
                            transaction.Commit();

                            // Kirim email setelah transaksi berhasil
                            KirimEmailTransaksi(
                                txtEmail.Text,
                                txtNamaPelanggan.Text,
                                lblMerkMobil.Text,
                                lblModelMobil.Text,
                                dtpTanggalSewa.Value.Date,
                                dtpTanggalSelesai.Value.Date,
                                totalHarga
                            );

                            MessageBox.Show($"Transaksi berhasil dibuat!\nTotal harga: Rp {totalHarga:N0}\nEmail sudah dikirim ke pelanggan.",
                                "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            this.Close();

                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            MessageBox.Show("Gagal menyimpan data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kesalahan koneksi: " + ex.Message);
            }
        }

        private void btnKembali_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dtpTanggalSewa_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}