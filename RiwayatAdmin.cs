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
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

namespace rental_mobiV2
{
    public partial class RiwayatAdmin : Form
    {
        private string connStr = "server=localhost;user=root;password=;database=rental_mobil;";
        private string username;
        public RiwayatAdmin(string uname)
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

        private void label2_Click(object sender, EventArgs e)
        {
            DashboardAdmin DashboardForm = new DashboardAdmin(username);
            DashboardForm.Show();
            this.Hide(); // sembunyiin form sekarang
        }
        private void LoadRiwayat(string keyword = "", DateTime? tanggalAwal = null, DateTime? tanggalAkhir = null)
        {
            string connectionString = "server=localhost;database=rental_mobil;uid=root;pwd=;";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string query = @"SELECT 
                                t.id AS 'ID Transaksi',
                                p.nama AS 'Nama Pelanggan',
                                m.merk AS 'Merk Mobil',
                                m.model AS 'Model Mobil',
                                t.tanggal_sewa AS 'Tanggal Sewa',
                                t.tanggal_selesai AS 'Tanggal Selesai',
                                t.tanggal_kembali AS 'Tanggal Kembali',
                                t.total_harga AS 'Total Harga',
                                t.denda AS 'Denda'
                             FROM transaksi t
                             JOIN pelanggan p ON t.id_pelanggan = p.id
                             JOIN mobil m ON t.id_mobil = m.id
                             WHERE 1=1"; // supaya gampang tambah kondisi

                    // Filter keyword
                    if (!string.IsNullOrEmpty(keyword))
                    {
                        query += @" AND (p.nama LIKE @keyword 
                            OR m.merk LIKE @keyword 
                            OR m.model LIKE @keyword)";
                    }

                    // Filter tanggal (misalnya pakai tanggal_sewa)
                    if (tanggalAwal.HasValue && tanggalAkhir.HasValue)
                    {
                        query += " AND t.tanggal_sewa BETWEEN @awal AND @akhir";
                    }

                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    if (!string.IsNullOrEmpty(keyword))
                    {
                        cmd.Parameters.AddWithValue("@keyword", "%" + keyword + "%");
                    }

                    if (tanggalAwal.HasValue && tanggalAkhir.HasValue)
                    {
                        // Format ke yyyy-MM-dd biar aman
                        cmd.Parameters.AddWithValue("@awal", tanggalAwal.Value.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@akhir", tanggalAkhir.Value.ToString("yyyy-MM-dd"));
                    }

                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dgvTransaksiTerbaru.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            LoadRiwayat(txtSearch.Text.Trim());
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadRiwayat(txtSearch.Text.Trim());
        }

        private void RiwayatAdmin_Load(object sender, EventArgs e)
        {
            LoadRiwayat();
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

        private void dtAwal_ValueChanged(object sender, EventArgs e)
        {
            LoadRiwayat(txtSearch.Text.Trim(), dtAwal.Value, dtAkhir.Value);
        }

        private void dtAkhir_ValueChanged(object sender, EventArgs e)
        {
            LoadRiwayat(txtSearch.Text.Trim(), dtAwal.Value, dtAkhir.Value);
        }

        private void btnExportPdf_Click(object sender, EventArgs e)
        {
            if (dgvTransaksiTerbaru.Rows.Count > 0)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "PDF (*.pdf)|*.pdf";
                saveFileDialog.FileName = "RiwayatTransaksi.pdf";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (FileStream stream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                        {
                            Document pdfDoc = new Document(PageSize.A4.Rotate(), 10f, 10f, 10f, 10f);
                            PdfWriter.GetInstance(pdfDoc, stream);
                            pdfDoc.Open();

                            // Judul
                            Paragraph title = new Paragraph(
                                "Laporan Riwayat Transaksi Ngabret.id\n",
                                iTextSharp.text.FontFactory.GetFont("Arial", 16, iTextSharp.text.Font.BOLD));
                            title.Alignment = Element.ALIGN_CENTER;
                            pdfDoc.Add(title);

                            // Periode (dipindah ke bawah judul)
                            string periodeText = $"Periode: {dtAwal.Value:dd-MM-yyyy} s/d {dtAkhir.Value:dd-MM-yyyy}";
                            Paragraph periodeParagraph = new Paragraph(
                                periodeText + "\n\n",
                                FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL)
                            );
                            periodeParagraph.Alignment = Element.ALIGN_CENTER;
                            pdfDoc.Add(periodeParagraph);

                            // Buat tabel sesuai jumlah kolom DataGridView
                            PdfPTable pdfTable = new PdfPTable(dgvTransaksiTerbaru.Columns.Count);
                            pdfTable.WidthPercentage = 100;

                            // Header
                            foreach (DataGridViewColumn column in dgvTransaksiTerbaru.Columns)
                            {
                                PdfPCell cell = new PdfPCell(new Phrase(column.HeaderText));
                                cell.BackgroundColor = new BaseColor(240, 240, 240);
                                pdfTable.AddCell(cell);
                            }

                            // Font normal untuk isi tabel
                            var fontNormal = FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL);

                            // Isi data
                            foreach (DataGridViewRow row in dgvTransaksiTerbaru.Rows)
                            {
                                if (!row.IsNewRow)
                                {
                                    for (int i = 0; i < row.Cells.Count; i++)
                                    {
                                        string cellValue = row.Cells[i].Value?.ToString();

                                        // Format tanggal
                                        if (row.Cells[i].Value is DateTime dt)
                                        {
                                            cellValue = dt.ToString("dd-MM-yyyy");
                                        }
                                        // Format angka (uang)
                                        else if (decimal.TryParse(cellValue, out decimal num))
                                        {
                                            cellValue = num.ToString("N0", new System.Globalization.CultureInfo("id-ID"));
                                        }

                                        // Tambahkan ke tabel PDF
                                        pdfTable.AddCell(new Phrase(cellValue ?? string.Empty, fontNormal));
                                    }
                                }
                            }

                            pdfDoc.Add(pdfTable);

                            pdfDoc.Close();
                            stream.Close();
                        }

                        MessageBox.Show("Data berhasil diexport ke PDF!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Tidak ada data untuk diexport!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void label4_Click(object sender, EventArgs e)
        {
            PelangganAdmin pelangganForm = new PelangganAdmin(username);
            pelangganForm.Show();
            this.Hide(); // sembunyiin form sekarang
        }

        private void panel13_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
