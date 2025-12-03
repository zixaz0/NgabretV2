using iTextSharp.text;
using iTextSharp.text.pdf;
using MySql.Data.MySqlClient;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Windows.Forms;

// === FIX NAMESPACE BENTROK ===
using PdfFont = iTextSharp.text.Font;

namespace rental_mobiV2
{
    public partial class FormPengembalian : Form
    {
        private string connStr = "server=localhost;user=root;password=;database=rental_mobil;";
        private int selectedTransaksiId = -1;
        private string username;
        // ⚠ Pakai App Password Gmail
        // Ubah ini ke email dan app-password Gmail kalian
        private const string SENDER_EMAIL = "ngabret.id30@gmail.com";
        private const string SENDER_APP_PASSWORD = "tdvt vhqi kfew mmje";

        public FormPengembalian(string uname)
        {
            InitializeComponent();
            username = uname;
        }

        // ============================
        // Hitung denda otomatis
        // ============================
        private decimal HitungDenda(int idTransaksi)
        {
            if (idTransaksi <= 0) return 0;

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();

                string q = @"
                    SELECT 
                        t.tanggal_selesai,
                        m.harga_sewa_perhari
                    FROM transaksi t
                    JOIN mobil m ON t.id_mobil = m.id
                    WHERE t.id = @id";
                MySqlCommand cmd = new MySqlCommand(q, conn);
                cmd.Parameters.AddWithValue("@id", idTransaksi);

                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        if (dr["tanggal_selesai"] == DBNull.Value) return 0;

                        DateTime dueDate = Convert.ToDateTime(dr["tanggal_selesai"]);
                        DateTime now = DateTime.Now.Date;

                        int telat = (now - dueDate.Date).Days;
                        if (telat <= 0) return 0;

                        decimal hargaPerHari = dr["harga_sewa_perhari"] != DBNull.Value
                            ? Convert.ToDecimal(dr["harga_sewa_perhari"])
                            : 0;

                        return telat * hargaPerHari;
                    }
                }
            }

            return 0;
        }

        // ============================
        // Kirim email pemberitahuan terlambat
        // ============================
        private void KirimEmailTerlambat(int idTransaksi, decimal denda)
        {
            try
            {
                if (denda <= 0) return;

                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    conn.Open();

                    string q = @"
                        SELECT p.email, p.nama, m.merk, m.model, t.tanggal_selesai
                        FROM transaksi t
                        JOIN pelanggan p ON t.id_pelanggan = p.id
                        JOIN mobil m ON t.id_mobil = m.id
                        WHERE t.id = @id";
                    MySqlCommand cmd = new MySqlCommand(q, conn);
                    cmd.Parameters.AddWithValue("@id", idTransaksi);

                    using (MySqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (!dr.Read()) return;

                        string email = dr["email"]?.ToString();
                        if (string.IsNullOrWhiteSpace(email)) return;

                        string nama = dr["nama"]?.ToString() ?? "";
                        string mobil = $"{dr["merk"]} {dr["model"]}";
                        DateTime due = Convert.ToDateTime(dr["tanggal_selesai"]);

                        MailMessage msg = new MailMessage();
                        msg.To.Add(email);
                        msg.From = new MailAddress(SENDER_EMAIL, "Rental Mobil V2");
                        msg.Subject = "Pemberitahuan: Pengembalian Mobil Terlambat";
                        msg.Body =
                        $@"Halo {nama},

                        Anda terlambat mengembalikan mobil:

                        Mobil: {mobil}
                        Tanggal seharusnya kembali: {due:dd/MM/yyyy}
                        Denda yang dikenakan: Rp {denda:N0}

                        Harap segera melakukan pelunasan dan mengembalikan mobil.

                        Terima kasih,
                        Ngabret.id";

                        using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                        {
                            smtp.EnableSsl = true;
                            smtp.Credentials = new NetworkCredential(SENDER_EMAIL, SENDER_APP_PASSWORD);
                            smtp.Send(msg);
                        }
                    }
                }
            }
            catch
            {
                // Jangan ganggu alur meski email gagal
            }
        }

        // ============================
        // Kirim email terima kasih + ringkasan (selalu dikirim)
        // ============================
        private void KirimEmailPengembalianBerhasil(int idTransaksi)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    conn.Open();

                    string q = @"
                        SELECT p.email, p.nama, m.merk, m.model, t.total_harga, t.denda
                        FROM transaksi t
                        JOIN pelanggan p ON t.id_pelanggan = p.id
                        JOIN mobil m ON t.id_mobil = m.id
                        WHERE t.id = @id";
                    MySqlCommand cmd = new MySqlCommand(q, conn);
                    cmd.Parameters.AddWithValue("@id", idTransaksi);

                    using (MySqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (!dr.Read()) return;

                        string email = dr["email"]?.ToString();
                        if (string.IsNullOrWhiteSpace(email)) return;

                        string nama = dr["nama"]?.ToString() ?? "";
                        string mobil = $"{dr["merk"]} {dr["model"]}";
                        decimal total = dr["total_harga"] != DBNull.Value ? Convert.ToDecimal(dr["total_harga"]) : 0;
                        decimal denda = dr["denda"] != DBNull.Value ? Convert.ToDecimal(dr["denda"]) : 0;
                        decimal grandTotal = total + denda;

                        MailMessage msg = new MailMessage();
                        msg.To.Add(email);
                        msg.From = new MailAddress(SENDER_EMAIL, "Ngabret.id");
                        msg.Subject = "Terima Kasih - Pengembalian Mobil Berhasil";
                        msg.Body =
                            $@"Halo {nama},

                            Terima kasih telah mengembalikan mobil yang Anda sewa.

                            Detail:
                            Mobil: {mobil}
                            Total Sewa: Rp {total:N0}
                            Denda: Rp {denda:N0}
                            Total Pembayaran: Rp {grandTotal:N0}

                            Jika butuh struk, Anda dapat menghubungi kami.

                            Salam,
                            Ngabret.id";

                        using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                        {
                            smtp.EnableSsl = true;
                            smtp.Credentials = new NetworkCredential(SENDER_EMAIL, SENDER_APP_PASSWORD);
                            smtp.Send(msg);
                        }
                    }
                }
            }
            catch
            {
                // Jangan ganggu alur kalau email gagal
            }
        }

        // ============================
        // Load kartu mobil yang sedang disewa (untuk proses pengembalian)
        // ============================
        private void FormPengembalian_Load(object sender, EventArgs e)
        {
            LoadCardMobil();
        }

        private void btnCari_Click(object sender, EventArgs e)
        {
            LoadCardMobil(txtCari.Text.Trim());
        }

        private void LoadCardMobil(string keyword = "")
        {
            flowPanelCards.Controls.Clear();

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();

                string query = @"
                    SELECT
                        t.id AS id_transaksi,
                        m.id AS id_mobil,
                        m.merk,
                        m.model,
                        m.foto,
                        p.nama AS penyewa
                    FROM transaksi t
                    JOIN mobil m ON t.id_mobil = m.id
                    JOIN pelanggan p ON t.id_pelanggan = p.id
                    WHERE m.status = 'disewa'
                    AND (m.merk LIKE @kw OR p.nama LIKE @kw)
                    AND t.tanggal_kembali IS NULL";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@kw", "%" + keyword + "%");

                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        string merk = dr["merk"].ToString();
                        string model = dr["model"].ToString();
                        string penyewa = dr["penyewa"].ToString();
                        string fotoPath = dr["foto"].ToString();
                        int idTransaksi = Convert.ToInt32(dr["id_transaksi"]);

                        Panel card = new Panel
                        {
                            Width = 250,
                            Height = 300,
                            BorderStyle = BorderStyle.FixedSingle,
                            BackColor = Color.White,
                            Margin = new Padding(10)
                        };

                        PictureBox pic = new PictureBox
                        {
                            Width = 220,
                            Height = 150,
                            SizeMode = PictureBoxSizeMode.Zoom,
                            Location = new Point(15, 10),
                            BorderStyle = BorderStyle.FixedSingle
                        };

                        if (File.Exists(fotoPath))
                        {
                            try { pic.Image = System.Drawing.Image.FromFile(fotoPath); }
                            catch { /* ignore image load errors */ }
                        }

                        Label lblMobil = new Label
                        {
                            Text = $"{merk} {model}",
                            AutoSize = false,
                            TextAlign = ContentAlignment.MiddleCenter,
                            Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold),
                            Width = 230,
                            Location = new Point(10, 170)
                        };

                        Label lblPenyewa = new Label
                        {
                            Text = "Penyewa: " + penyewa,
                            AutoSize = false,
                            TextAlign = ContentAlignment.MiddleCenter,
                            Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Regular),
                            Width = 230,
                            Location = new Point(10, 200)
                        };

                        Button btnKembalikan = new Button
                        {
                            Text = "Kembalikan",
                            Width = 200,
                            Height = 35,
                            Location = new Point(25, 240),
                            BackColor = Color.SteelBlue,
                            ForeColor = Color.White,
                            FlatStyle = FlatStyle.Flat
                        };

                        btnKembalikan.Click += (s, ev) =>
                        {
                            selectedTransaksiId = idTransaksi;
                            ShowModal();
                        };

                        card.Controls.Add(pic);
                        card.Controls.Add(lblMobil);
                        card.Controls.Add(lblPenyewa);
                        card.Controls.Add(btnKembalikan);

                        flowPanelCards.Controls.Add(card);
                    }
                }
            }
        }

        // ============================
        // Modal pengembalian
        // ============================
        private void ShowModal()
        {
            Form modal = new Form
            {
                Width = 420,
                Height = 380,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                Text = "Form Pengembalian",
                BackColor = Color.White
            };

            Label lblStatusPengembalian = new Label { Text = "Status Pengembalian:", Location = new Point(30, 40) };
            ComboBox cmbStatusPengembalian = new ComboBox
            {
                Location = new Point(200, 35),
                Width = 160,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbStatusPengembalian.Items.AddRange(new string[] { "lunas", "denda" });
            cmbStatusPengembalian.SelectedIndex = 0;

            Label lblStatusMobil = new Label { Text = "Status Mobil:", Location = new Point(30, 95) };
            ComboBox cmbStatusMobil = new ComboBox
            {
                Location = new Point(200, 90),
                Width = 160,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbStatusMobil.Items.AddRange(new string[] { "tersedia", "servis" });
            cmbStatusMobil.SelectedIndex = 0;

            Label lblDenda = new Label { Text = "Denda (Rp):", Location = new Point(30, 150) };
            decimal dendaOtomatis = HitungDenda(selectedTransaksiId);
            TextBox txtDenda = new TextBox
            {
                Location = new Point(200, 145),
                Width = 160,
                Text = dendaOtomatis.ToString("F0")
            };
            txtDenda.ReadOnly = true; // otomatis dihitung

            Button btnSimpan = new Button
            {
                Text = "Simpan",
                Width = 120,
                Height = 35,
                Location = new Point(140, 250),
                BackColor = Color.Green,
                ForeColor = Color.White
            };

            btnSimpan.Click += (s, e) =>
            {
                // Validasi simple
                if (selectedTransaksiId <= 0)
                {
                    MessageBox.Show("Transaksi tidak valid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Simpan pengembalian (denda otomatis dipakai)
                SimpanPengembalian(selectedTransaksiId, cmbStatusPengembalian.Text, cmbStatusMobil.Text);

                // Email thank you (selalu) dan email terlambat (jika ada denda)
                decimal denda = HitungDenda(selectedTransaksiId);
                try { KirimEmailPengembalianBerhasil(selectedTransaksiId); } catch { }
                try { if (denda > 0) KirimEmailTerlambat(selectedTransaksiId, denda); } catch { }

                // Tanyakan apakah ingin download/print struk
                var res = MessageBox.Show("Pengembalian berhasil disimpan.\n\nIngin mendownload/menyimpan struk PDF sekarang?", "Sukses", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (res == DialogResult.Yes)
                {
                    CetakStruk(selectedTransaksiId);
                }

                modal.Close();
                LoadCardMobil();
            };

            modal.Controls.Add(lblStatusPengembalian);
            modal.Controls.Add(cmbStatusPengembalian);
            modal.Controls.Add(lblStatusMobil);
            modal.Controls.Add(cmbStatusMobil);
            modal.Controls.Add(lblDenda);
            modal.Controls.Add(txtDenda);
            modal.Controls.Add(btnSimpan);
            modal.ShowDialog();
        }

        // ============================
        // Simpan pengembalian (DB updates)
        // ============================
        private void SimpanPengembalian(int idTransaksi, string statusPengembalian, string statusMobil)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    conn.Open();

                    decimal dendaFix = HitungDenda(idTransaksi);

                    string insertQuery = @"INSERT INTO pengembalian
                        (id_transaksi, tanggal_pengembalian, status_pengembalian, status_mobil, denda, created_at)
                        VALUES (@id_transaksi, NOW(), @status_pengembalian, @status_mobil, @denda, NOW())";
                    MySqlCommand cmd = new MySqlCommand(insertQuery, conn);
                    cmd.Parameters.AddWithValue("@id_transaksi", idTransaksi);
                    cmd.Parameters.AddWithValue("@status_pengembalian", statusPengembalian);
                    cmd.Parameters.AddWithValue("@status_mobil", statusMobil);
                    cmd.Parameters.AddWithValue("@denda", dendaFix);
                    cmd.ExecuteNonQuery();

                    string updateMobil = @"UPDATE mobil
                                           SET status=@status_mobil
                                           WHERE id = (SELECT id_mobil FROM transaksi WHERE id=@id_transaksi)";
                    MySqlCommand cmdMobil = new MySqlCommand(updateMobil, conn);
                    cmdMobil.Parameters.AddWithValue("@status_mobil", statusMobil);
                    cmdMobil.Parameters.AddWithValue("@id_transaksi", idTransaksi);
                    cmdMobil.ExecuteNonQuery();

                    string updateTransaksi = @"
                        UPDATE transaksi
                        SET tanggal_kembali = NOW(), denda = @denda
                        WHERE id = @id";
                    MySqlCommand cmdTransaksi = new MySqlCommand(updateTransaksi, conn);
                    cmdTransaksi.Parameters.AddWithValue("@id", idTransaksi);
                    cmdTransaksi.Parameters.AddWithValue("@denda", dendaFix);
                    cmdTransaksi.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal menyimpan data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ============================
        // Cetak / Simpan Struk PDF (80mm)
        // ============================
        private void CetakStruk(int idTransaksi)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    conn.Open();

                    string q = @"
                        SELECT t.*, m.merk, m.model, p.nama, pen.denda as denda_pengembalian
                        FROM transaksi t
                        JOIN mobil m ON t.id_mobil = m.id
                        JOIN pelanggan p ON t.id_pelanggan = p.id
                        LEFT JOIN pengembalian pen ON pen.id_transaksi = t.id
                        WHERE t.id = @id";
                    MySqlCommand cmd = new MySqlCommand(q, conn);
                    cmd.Parameters.AddWithValue("@id", idTransaksi);

                    using (MySqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (!dr.Read()) return;

                        // Tanyakan lokasi simpan via SaveFileDialog
                        SaveFileDialog sfd = new SaveFileDialog();
                        sfd.Filter = "PDF (*.pdf)|*.pdf";
                        sfd.FileName = $"Struk_Pengembalian_{idTransaksi}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                        if (sfd.ShowDialog() != DialogResult.OK) return;

                        string savePath = sfd.FileName;

                        // Ukuran struk kecil (80mm width)
                        iTextSharp.text.Rectangle receiptSize = new iTextSharp.text.Rectangle(226.77f, 600f); // 80mm x ~210mm
                        Document doc = new Document(receiptSize, 10, 10, 10, 10);
                        PdfWriter.GetInstance(doc, new FileStream(savePath, FileMode.Create));
                        doc.Open();

                        // Font definitions
                        PdfFont headerFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 14);
                        PdfFont titleFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 11);
                        PdfFont normalFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA, 9);
                        PdfFont boldFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 9);
                        PdfFont smallFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA, 7);

                        // HEADER
                        Paragraph header = new Paragraph("Ngabret.id", headerFont) { Alignment = Element.ALIGN_CENTER };
                        doc.Add(header);

                        Paragraph subtitle = new Paragraph("Jl. Raya Rental No. 123\nTelp: 0812-3456-7890", smallFont) { Alignment = Element.ALIGN_CENTER, SpacingAfter = 5f };
                        doc.Add(subtitle);

                        doc.Add(new Paragraph("================================================", smallFont) { Alignment = Element.ALIGN_CENTER, SpacingAfter = 5f });

                        Paragraph strukTitle = new Paragraph("STRUK PENGEMBALIAN", titleFont) { Alignment = Element.ALIGN_CENTER, SpacingAfter = 5f };
                        doc.Add(strukTitle);

                        doc.Add(new Paragraph("================================================", smallFont) { Alignment = Element.ALIGN_CENTER, SpacingAfter = 10f });

                        // INFO TRANSAKSI
                        doc.Add(new Paragraph($"No. Transaksi: {idTransaksi}", normalFont));
                        doc.Add(new Paragraph($"Tanggal: {DateTime.Now:dd/MM/yyyy HH:mm}", normalFont));
                        doc.Add(new Paragraph($"Kasir: {username}", normalFont) { SpacingAfter = 10f });

                        doc.Add(new Paragraph("------------------------------------------------", smallFont) { Alignment = Element.ALIGN_CENTER, SpacingAfter = 5f });

                        // DATA PENYEWA & MOBIL
                        doc.Add(new Paragraph("DETAIL SEWA", boldFont) { SpacingAfter = 5f });
                        doc.Add(new Paragraph($"Penyewa: {dr["nama"]}", normalFont));
                        doc.Add(new Paragraph($"Mobil: {dr["merk"]} {dr["model"]}", normalFont) { SpacingAfter = 5f });

                        if (dr["tanggal_sewa"] != DBNull.Value)
                            doc.Add(new Paragraph($"Tgl Sewa: {Convert.ToDateTime(dr["tanggal_sewa"]):dd/MM/yyyy}", normalFont));
                        if (dr["tanggal_selesai"] != DBNull.Value)
                            doc.Add(new Paragraph($"Tgl Selesai: {Convert.ToDateTime(dr["tanggal_selesai"]):dd/MM/yyyy}", normalFont));
                        doc.Add(new Paragraph($"Tgl Kembali: {DateTime.Now:dd/MM/yyyy}", normalFont) { SpacingAfter = 10f });

                        doc.Add(new Paragraph("------------------------------------------------", smallFont) { Alignment = Element.ALIGN_CENTER, SpacingAfter = 5f });

                        // RINCIAN BIAYA
                        doc.Add(new Paragraph("RINCIAN BIAYA", boldFont) { SpacingAfter = 5f });

                        decimal totalHarga = dr["total_harga"] != DBNull.Value ? Convert.ToDecimal(dr["total_harga"]) : 0;
                        decimal denda = dr["denda_pengembalian"] != DBNull.Value ? Convert.ToDecimal(dr["denda_pengembalian"]) : 0;
                        decimal grandTotal = totalHarga + denda;

                        PdfPTable tableHarga = new PdfPTable(2) { WidthPercentage = 100 };
                        tableHarga.SetWidths(new float[] { 60f, 40f });
                        tableHarga.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;

                        PdfPCell left = new PdfPCell(new Phrase("Biaya Sewa", normalFont)) { Border = iTextSharp.text.Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT };
                        PdfPCell right = new PdfPCell(new Phrase($"Rp {totalHarga:N0}", normalFont)) { Border = iTextSharp.text.Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT };
                        tableHarga.AddCell(left); tableHarga.AddCell(right);

                        if (denda > 0)
                        {
                            left = new PdfPCell(new Phrase("Denda", normalFont)) { Border = iTextSharp.text.Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT };
                            right = new PdfPCell(new Phrase($"Rp {denda:N0}", normalFont)) { Border = iTextSharp.text.Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT };
                            tableHarga.AddCell(left); tableHarga.AddCell(right);
                        }

                        doc.Add(tableHarga);

                        doc.Add(new Paragraph("------------------------------------------------", smallFont) { Alignment = Element.ALIGN_CENTER, SpacingAfter = 5f, SpacingBefore = 5f });

                        PdfPTable tableTotal = new PdfPTable(2) { WidthPercentage = 100 };
                        tableTotal.SetWidths(new float[] { 60f, 40f });
                        tableTotal.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;

                        left = new PdfPCell(new Phrase("TOTAL", boldFont)) { Border = iTextSharp.text.Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT };
                        right = new PdfPCell(new Phrase($"Rp {grandTotal:N0}", boldFont)) { Border = iTextSharp.text.Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT };
                        tableTotal.AddCell(left); tableTotal.AddCell(right);

                        doc.Add(tableTotal);

                        doc.Add(new Paragraph("================================================", smallFont) { Alignment = Element.ALIGN_CENTER, SpacingAfter = 10f, SpacingBefore = 5f });

                        Paragraph thanks = new Paragraph("Terima Kasih", titleFont) { Alignment = Element.ALIGN_CENTER, SpacingAfter = 3f };
                        doc.Add(thanks);

                        Paragraph footer = new Paragraph("Semoga Anda Puas Dengan Layanan Kami\nSampai Jumpa Kembali!", smallFont) { Alignment = Element.ALIGN_CENTER, SpacingAfter = 10f };
                        doc.Add(footer);

                        doc.Add(new Paragraph("================================================", smallFont) { Alignment = Element.ALIGN_CENTER });

                        doc.Close();

                        MessageBox.Show($"Struk berhasil disimpan di:\n{savePath}", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Buka file secara otomatis
                        Process.Start(new ProcessStartInfo() { FileName = savePath, UseShellExecute = true });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal mencetak struk: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnKembali_Click(object sender, EventArgs e)
        {
            DashboardKasir dashboard = new DashboardKasir(username);
            dashboard.Show();
            this.Hide();
        }
    }
}