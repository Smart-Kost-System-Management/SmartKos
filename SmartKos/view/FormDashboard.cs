using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SmartKos.lib;
using SmartKos.controller; 
using SmartKos.model;      

namespace SmartKos.view
{
    public partial class FormDashboard : Form
    {
        private Kamar kamarCtrl;
        private ExcelService excelSvc;
        private string selectedKamarID = "";
        public FormDashboard()
        {
            InitializeComponent();
            kamarCtrl = new Kamar();
            excelSvc = new ExcelService();
        }

        private void FormDashboard_Load(object sender, EventArgs e)
        {
            RefreshSemua();
        }

        private void RefreshSemua()
        {
            LoadDataGrid();   // Refresh Tabel
            LoadDataVisual(); // Refresh Tombol Warna-warni
            BersihkanInput(); // Reset Textbox
        }

        private void BersihkanInput()
        {
            txtNoKamar.Clear();
            txtHarga.Clear();

            cmbTipe.SelectedIndex = -1;
            cmbTipe.Text = "";

            cmbStatus.SelectedIndex = -1;
            cmbStatus.Text = "";

            selectedKamarID = "";
            txtNoKamar.Focus();
        }

        private void LoadDataGrid()
        {
            dgvData.DataSource = kamarCtrl.TampilSemuaKamar();
        }

        private void LoadDataVisual()
        {
            pnlKamar.Controls.Clear(); // Bersihkan panel dulu
            List<M_kamar> list = kamarCtrl.GetListKamar();

            int x = 10;
            int y = 10;
            int counter = 0;

            foreach (var item in list)
            {
                Button btn = new Button();
                btn.Text = "KMR " + item.NomorKamar + "\n" + item.Status;
                btn.Size = new Size(100, 100);
                btn.Location = new Point(x, y);
                btn.FlatStyle = FlatStyle.Flat;
                btn.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                btn.ForeColor = Color.White;

                // Logika Warna (Syarat UI Menarik)
                if (item.Status == "Kosong") btn.BackColor = Color.SeaGreen;
                else btn.BackColor = Color.Crimson;

                // Event Klik (Optional: Tampilkan Detail)
                btn.Click += (s, ev) => { MessageBox.Show("Ini Kamar " + item.NomorKamar); };

                // Masukkan ke Panel
                pnlKamar.Controls.Add(btn);

                // Mengatur posisi tombol agar rapi (Grid Layout Manual)
                x += 110; // Geser ke kanan
                counter++;
                if (counter >= 7) // Jika sudah 7 tombol ke kanan, turun ke bawah
                {
                    x = 10;
                    y += 110;
                    counter = 0;
                }
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadDataVisual();
            LoadDataGrid();
            BersihkanInput();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            excelSvc.ExportToExcel(dgvData);
        }

        private void dgvData_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // Ambil data dari baris yang diklik
                DataGridViewRow row = dgvData.Rows[e.RowIndex];

                // Simpan ID ke variabel global (disembunyikan)
                selectedKamarID = row.Cells["KamarID"].Value.ToString();

                // Tampilkan data ke Textbox
                txtNoKamar.Text = row.Cells["NomorKamar"].Value.ToString();
                cmbTipe.Text = row.Cells["TipeKamar"].Value.ToString();

                // Format harga (hilangkan simbol mata uang jika ada biar bisa diedit angka saja)
                string hargaStr = row.Cells["Harga"].Value.ToString();
                txtHarga.Text = System.Text.RegularExpressions.Regex.Replace(hargaStr, "[^0-9]", "");

                cmbStatus.Text = row.Cells["Status"].Value.ToString();
            }
        }

        private void btnSimpan_Click(object sender, EventArgs e)
        {
            if (txtNoKamar.Text == "" || txtHarga.Text == "")
            {
                MessageBox.Show("Data tidak boleh kosong!", "Peringatan");
                return;
            }

            M_kamar kmr = new M_kamar();
            kmr.NomorKamar = txtNoKamar.Text;
            kmr.TipeKamar = cmbTipe.Text;
            kmr.Harga = Convert.ToDecimal(txtHarga.Text);
            kmr.Status = cmbStatus.Text;

            try
            {
                kamarCtrl.TambahKamar(kmr);
                MessageBox.Show("Data Berhasil Disimpan!");
                RefreshSemua();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnUbah_Click(object sender, EventArgs e)
        {
            if (selectedKamarID == "")
            {
                MessageBox.Show("Pilih data dari tabel terlebih dahulu!", "Peringatan");
                return;
            }

            M_kamar kmr = new M_kamar();
            kmr.NomorKamar = txtNoKamar.Text;
            kmr.TipeKamar = cmbTipe.Text;
            kmr.Harga = Convert.ToDecimal(txtHarga.Text);
            kmr.Status = cmbStatus.Text;

            try
            {
                kamarCtrl.UpdateKamar(kmr, selectedKamarID);
                MessageBox.Show("Data Berhasil Diubah!");
                RefreshSemua();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnHapus_Click(object sender, EventArgs e)
        {
            if (selectedKamarID == "")
            {
                MessageBox.Show("Pilih data yang ingin dihapus!", "Peringatan");
                return;
            }

            DialogResult jawab = MessageBox.Show("Yakin ingin menghapus data ini?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (jawab == DialogResult.Yes)
            {
                try
                {
                    kamarCtrl.HapusKamar(selectedKamarID);
                    MessageBox.Show("Data Berhasil Dihapus!");
                    RefreshSemua();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            BersihkanInput();
        }

        private void txtCari_TextChanged(object sender, EventArgs e)
        {
            if (txtCari.Text == "")
            {
                LoadDataGrid(); // Panggil fungsi load standar (SELECT ALL)
            }
            else
            {
                // Panggil fungsi Cari dari Controller
                dgvData.DataSource = kamarCtrl.CariKamar(txtCari.Text);
            }
        }
    }
}
