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
        public FormDashboard()
        {
            InitializeComponent();
            kamarCtrl = new Kamar();
            excelSvc = new ExcelService();
        }

        private void FormDashboard_Load(object sender, EventArgs e)
        {
            LoadDataVisual();
            LoadDataGrid();
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
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            excelSvc.ExportToExcel(dgvData);
        }
    }
}
