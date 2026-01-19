using MySql.Data.MySqlClient;
using SmartKos.controller; 
using SmartKos.lib;
using SmartKos.model;      
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmartKos.view
{
    public partial class FormDashboard : Form
    {
        // TAMBAHKAN BARIS INI: Untuk menyimpan ID kamar yang dipilih dari tabel
        private Kamar kamarCtrl;
        private ExcelService excelSvc;
        public FormDashboard()
        {
            InitializeComponent();

            // HANYA GUNAKAN SATU BARIS INI UNTUK INISIALISASI
            kamarCtrl = new SmartKos.controller.Kamar();
            excelSvc = new ExcelService();
            cmbStatus.Items.Clear();
            cmbStatus.Items.AddRange(new string[] { "Terisi", "Kosong", "Maintenance" });
        }

        private void FormDashboard_Load(object sender, EventArgs e)
        {
            // Inisialisasi controller agar tidak NullReferenceException
            kamarCtrl = new SmartKos.controller.Kamar();

            LoadDataGrid();   // Isi tabel user
            LoadDataVisual(); // ISI MONITORING KAMAR   

            cmbStatus.Items.Clear();
            cmbStatus.Items.Add("Terisi");
            cmbStatus.Items.Add("Kosong");
            cmbStatus.Items.Add("Maintenance");

        }

        private void LoadDataGrid()
        {
            // Mengisi DataGrid dengan hasil query dari Kamar.cs
            dgvData.DataSource = kamarCtrl.TampilSemuaKamar();

            // Opsional: Mempercantik judul kolom di tampilan
            if (dgvData.Columns.Count > 0)
            {
                dgvData.Columns["NomorKamar"].HeaderText = "No. Kamar";
                dgvData.Columns["TipeKamar"].HeaderText = "Tipe Kamar";
                dgvData.Columns["Harga"].HeaderText = "Harga (Rp)";
                dgvData.Columns["Status"].HeaderText = "Status";
            }
        }

        private void LoadDataVisual()
        {
            pnlKamar.Controls.Clear(); // Bersihkan panel agar tidak menumpuk
            List<M_kamar> list = kamarCtrl.GetStatusKamar(); // Ambil data dari Kamar.cs

            int x = 10, y = 10;
            foreach (var item in list)
            {
                Button btn = new Button();
                btn.Text = "KMR " + item.NomorKamar + "\n" + item.Status;
                btn.Size = new Size(100, 100);
                btn.Location = new Point(x, y);

                // Memberi warna berdasarkan status dari database
                if (item.Status == "Terisi")
                    btn.BackColor = Color.Red;
                else if (item.Status == "Kosong")
                    btn.BackColor = Color.LightGreen;
                else
                    btn.BackColor = Color.Orange;

                // Tambahkan event klik agar data pindah ke TextBox saat kotak diklik
                btn.Click += (s, ev) => {
                    txtNomorKamar.Text = item.NomorKamar;
                    cmbStatus.Text = item.Status;
                };

                pnlKamar.Controls.Add(btn);
                x += 110;
                if (x > pnlKamar.Width - 100) { x = 10; y += 110; }
            }
            
        }

        private void FormDashboard_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadDataVisual();
            LoadDataGrid();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            excelSvc.ExportToExcel(dgvData);
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            string nomor = txtNomorKamar.Text; // Misal diisi 105
            string status = cmbStatus.Text;

            if (string.IsNullOrEmpty(nomor))
            {
                MessageBox.Show("Masukkan Nomor Kamar!");
                return;
            }

            // Cek apakah nomor kamar 105 sudah ada di database
            if (kamarCtrl.CekKamarEksis(nomor))
            {
                // JIKA ADA: Jalankan Update Status
                kamarCtrl.UpdateStatusKamar(nomor, status, null);
                MessageBox.Show("Status Kamar " + nomor + " Berhasil Diperbarui!");
            }
            else
            {
                // JIKA TIDAK ADA: Tambahkan sebagai kamar baru agar muncul di monitoring
                if (kamarCtrl.TambahKamarBaru(nomor, status))
                {
                    MessageBox.Show("Kamar Baru " + nomor + " Berhasil Ditambahkan ke Monitoring!");
                }
            }

            // REFRESH TAMPILAN agar kotak 105 langsung muncul
            LoadDataGrid();   // Update tabel bawah
            LoadDataVisual(); // Gambar ulang kotak-kotak monitoring
        }

        private void txtKamarID_TextChanged(object sender, EventArgs e)
        {

        }

        public List<M_kamar> GetDataKamar()
        {
            List<M_kamar> list = new List<M_kamar>();
            // ... isi logika SELECT * FROM tbl_kamar ...
            return list;
        }

        public List<M_kamar> GetStatusKamar()
        {
            List<M_kamar> list = new List<M_kamar>();

            // Gunakan nama tabel sesuai di database kamu (tbl_kamar)
            string query = "SELECT nomor_kamar, status FROM tbl_kamar";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(Koneksi.connString))
                {
                    conn.Open(); // Buka koneksi terlebih dahulu

                    // Perbaikan: Gunakan MySqlCommand untuk menjalankan query
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        // Perbaikan: Gunakan MySqlDataReader untuk membaca hasil query
                        using (MySqlDataReader rd = cmd.ExecuteReader())
                        {
                            while (rd.Read())
                            {
                                list.Add(new M_kamar
                                {
                                    // Pastikan properti ini ada di model M_kamar.cs
                                    NomorKamar = rd["nomor_kamar"].ToString(),
                                    Status = rd["status"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Terjadi Kesalahan: " + ex.Message);
            }

            return list;

        }
    }
    
}
