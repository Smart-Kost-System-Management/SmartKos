using MySql.Data.MySqlClient;
using SmartKos.controller; 
using SmartKos.lib;
using SmartKos.model;      
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmartKos.view
{
    public partial class FormDashboard : Form
    {
        private Kamar kamarCtrl;
        private ExcelService excelSvc;

        public FormDashboard()
        {
            InitializeComponent();
            kamarCtrl = new Kamar();
            excelSvc = new ExcelService();

            // Apply Styling
            UIHelper.SetFormStyle(this);
            UIHelper.StyleDataGrid(dgvData);
            UIHelper.StyleButton(btnSimpan, true);
            UIHelper.StyleButton(btnUbah, false);
            UIHelper.StyleButton(btnHapus, false);
            UIHelper.StyleButton(btnClear, false);
            UIHelper.StyleButton(btnRefresh, false);
            UIHelper.StyleButton(btnExport, false);
            btnExport.BackColor = UIHelper.SuccessColor;

            // Load items into Status ComboBox
            cmbStatus.Items.Clear();
            cmbStatus.Items.AddRange(new string[] { "Terisi", "Kosong", "Maintenance" });
        }

        private void FormDashboard_Load(object sender, EventArgs e)
        {
            RefreshSemua();
        }

        private void RefreshSemua()
        {
            LoadDataGrid();
            LoadDataVisual();
            BersihkanInput();
        }

        private void LoadDataGrid()
        {
            dgvData.DataSource = kamarCtrl.TampilSemuaKamar();
            
            if (dgvData.Columns.Count > 0)
            {
                if (dgvData.Columns.Contains("KamarID")) dgvData.Columns["KamarID"].Visible = false;
                if (dgvData.Columns.Contains("NomorKamar")) dgvData.Columns["NomorKamar"].HeaderText = "No. Kamar";
                if (dgvData.Columns.Contains("TipeKamar")) dgvData.Columns["TipeKamar"].HeaderText = "Tipe Kamar";
                if (dgvData.Columns.Contains("Harga")) dgvData.Columns["Harga"].HeaderText = "Harga (Rp)";
                if (dgvData.Columns.Contains("Status")) dgvData.Columns["Status"].HeaderText = "Status";
            }
        }

        private void LoadDataVisual()
        {
            pnlKamar.Controls.Clear();
            List<M_kamar> list = kamarCtrl.GetStatusKamar();

            int x = 10, y = 10;
            int counter = 0;
            foreach (var item in list)
            {
                Button btn = new Button();
                btn.Text = "KMR " + item.NomorKamar + "\n" + item.Status;
                btn.Size = new Size(100, 100);
                btn.Location = new Point(x, y);

                UIHelper.StyleButton(btn, false);
                btn.Height = 100;

                if (item.Status == "Terisi")
                    btn.BackColor = UIHelper.DangerColor;
                else if (item.Status == "Kosong")
                    btn.BackColor = UIHelper.SuccessColor;
                else
                    btn.BackColor = UIHelper.WarningColor;

                btn.Click += (s, ev) => {
                    txtNoKamar.Text = item.NomorKamar;
                    cmbStatus.Text = item.Status;
                    // Try to select in Grid too if possible
                    foreach (DataGridViewRow row in dgvData.Rows)
                    {
                        if (row.Cells["NomorKamar"].Value?.ToString() == item.NomorKamar)
                        {
                            row.Selected = true;
                            dgvData_CellClick(null, new DataGridViewCellEventArgs(0, row.Index));
                            break;
                        }
                    }
                };

                pnlKamar.Controls.Add(btn);
                x += 110;
                counter++;
                if (counter >= 7) { x = 10; y += 110; counter = 0; }
            }
        }

        private void BersihkanInput()
        {
            txtNoKamar.Clear();
            txtHarga.Clear();
            cmbTipe.SelectedIndex = -1;
            cmbStatus.SelectedIndex = -1;
            txtNoKamar.Focus();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshSemua();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            excelSvc.ExportToExcel(dgvData);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            BersihkanInput();
        }

        private void btnHapus_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtNoKamar.Text))
            {
                MessageBox.Show("Pilih data yang akan dihapus!");
                return;
            }

            if (MessageBox.Show("Hapus kamar " + txtNoKamar.Text + "?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (kamarCtrl.HapusKamar(txtNoKamar.Text))
                {
                    MessageBox.Show("Data berhasil dihapus!");
                    RefreshSemua();
                }
            }
        }

        private void btnSimpan_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtNoKamar.Text) || cmbTipe.SelectedIndex == -1 || string.IsNullOrEmpty(txtHarga.Text))
            {
                MessageBox.Show("Data belum lengkap!", "Peringatan");
                return;
            }

            if (kamarCtrl.TambahKamar(txtNoKamar.Text, cmbTipe.Text, txtHarga.Text, cmbStatus.Text))
            {
                MessageBox.Show("Data berhasil disimpan!");
                RefreshSemua();
            }
        }

        private void btnUbah_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtNoKamar.Text))
            {
                MessageBox.Show("Pilih data yang akan diubah!");
                return;
            }

            if (kamarCtrl.UpdateStatusKamar(txtNoKamar.Text, cmbStatus.Text, cmbTipe.Text, txtHarga.Text))
            {
                MessageBox.Show("Data berhasil diupdate!");
                RefreshSemua();
            }
        }

        private void dgvData_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvData.Rows[e.RowIndex];
                txtNoKamar.Text = row.Cells["NomorKamar"].Value?.ToString();
                cmbTipe.Text = row.Cells["TipeKamar"].Value?.ToString();
                txtHarga.Text = row.Cells["Harga"].Value?.ToString();
                cmbStatus.Text = row.Cells["Status"].Value?.ToString();
            }
        }

        private void txtCari_TextChanged(object sender, EventArgs e)
        {
            dgvData.DataSource = kamarCtrl.CariKamar(txtCari.Text);
        }
    }
}
