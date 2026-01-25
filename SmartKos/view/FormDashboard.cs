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
            
            // Apply Modern UI
            UIHelper.SetFormStyle(this);
            UIHelper.StyleDataGrid(dgvData);
            
            // Custom button styling
            UIHelper.StyleButton(btnSimpan, true); // Primary
            UIHelper.StyleButton(btnRefresh, false); 
            UIHelper.StyleButton(btnExport, false); 
            btnExport.BackColor = UIHelper.SuccessColor; // Green for Excel
            
            // Header Title
            UIHelper.StyleLabel(label1, true);

            // Gradient Background
            this.Paint += (s, e) => UIHelper.PaintGradientBackground(this, e.Graphics, UIHelper.BackgroundColor, Color.White);
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
                
                // Use Helper for base style
                UIHelper.StyleButton(btn, false); 
                btn.Height = 100; // Restore height
                
                // FORCE ROUNDED REGION on these dynamic buttons
                // Need to do this AFTER adding to control or ensuring size, but Helper StyleButton attaches Resize event which handles it.
                // However, we need to make sure the resize event triggers or we call it manually if size doesn't change from default (40) to 100 yet.
                // Actually StyleButton sets size to 45. We set to 100 right after.
                // The Resize event inside StyleButton will catch the Height change to 100? No, setting Height triggers Resize.
                // So it should work automatically!
                
                // Logika Warna (Syarat UI Menarik)
                if (item.Status == "Kosong") btn.BackColor = UIHelper.SuccessColor;
                else btn.BackColor = UIHelper.DangerColor;
                
                // Fix text color if background is light/dark, but here we used strong colors so White is fine which Helper sets.

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
