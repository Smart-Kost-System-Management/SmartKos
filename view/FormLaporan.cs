using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using SmartKos.controller;
using SmartKos.model;

namespace SmartKos.view
{
    public partial class FormLaporan : Form
    {
        private Kamar _kamarCtrl;
        private PembayaranController _bayarCtrl;

        public FormLaporan()
        {
            InitializeComponent();
            _kamarCtrl = new Kamar();
            _bayarCtrl = new PembayaranController();
        }

        private void FormLaporan_Load(object sender, EventArgs e)
        {
            // Apply Global Styles
            // UIHelper.SetFormStyle(this); // Optional: if we want full form styling
            this.BackColor = Color.WhiteSmoke;
            this.Font = UIHelper.MainFont;

            // Style Controls
            UIHelper.StyleButton(btnRefresh, true); // Refresh as primary action here
            UIHelper.StyleLabel(lblTotalPendapatan, true); // Header style for Total
            UIHelper.StyleDataGrid(dgvPendapatan);

            // TabControl Styling (Manual as UIHelper doesn't have it yet)
            tabControl1.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            // panel1.BackColor = UIHelper.PrimaryColor; // Optional: Header strip

            LoadStats();
        }

        private void LoadStats()
        {
            LoadKamarStats();
            LoadPendapatanStats();
        }

        private void LoadKamarStats()
        {
            List<M_kamar> list = _kamarCtrl.GetAll();

            // Statistik using LINQ
            int terisi = list.Count(k => k.Status == "Terisi");
            int kosong = list.Count(k => k.Status == "Kosong");
            int maintenance = list.Count(k => k.Status == "Maintenance");

            chartKamar.Series[0].Points.Clear();
            
            // Add Data
            if (terisi > 0) chartKamar.Series[0].Points.AddXY("Terisi", terisi);
            if (kosong > 0) chartKamar.Series[0].Points.AddXY("Kosong", kosong);
            if (maintenance > 0) chartKamar.Series[0].Points.AddXY("Maintenance", maintenance);

            chartKamar.Series[0].IsValueShownAsLabel = true;
            // Chart Styling
            chartKamar.BackColor = Color.WhiteSmoke;
            chartKamar.ChartAreas[0].BackColor = Color.Transparent;
        }

        private void LoadPendapatanStats()
        {
            List<M_Pembayaran> list = _bayarCtrl.GetAll();

            // Populate Grid
            dgvPendapatan.DataSource = list;

            // --- GRID FORMATTING ---
            if (dgvPendapatan.Columns.Count > 0)
            {
                // 1. Reset Global AutoSize to allow custom Column modes
                dgvPendapatan.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                dgvPendapatan.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells; // Allow taller rows

                // Hide IDs
                if(dgvPendapatan.Columns["PembayaranID"] != null) dgvPendapatan.Columns["PembayaranID"].Visible = false;
                if(dgvPendapatan.Columns["PenghuniID"] != null) dgvPendapatan.Columns["PenghuniID"].Visible = false;

                // Format & Rename & Sizing
                if(dgvPendapatan.Columns["TanggalBayar"] != null) 
                {
                    dgvPendapatan.Columns["TanggalBayar"].HeaderText = "Tanggal";
                    dgvPendapatan.Columns["TanggalBayar"].DefaultCellStyle.Format = "dd MMM yyyy";
                    dgvPendapatan.Columns["TanggalBayar"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells; // Fit Content
                    dgvPendapatan.Columns["TanggalBayar"].DisplayIndex = 0;
                }
                if(dgvPendapatan.Columns["NamaPenghuni"] != null)
                {
                    dgvPendapatan.Columns["NamaPenghuni"].HeaderText = "Nama Penghuni";
                    dgvPendapatan.Columns["NamaPenghuni"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; // Fill remaining space
                    dgvPendapatan.Columns["NamaPenghuni"].DisplayIndex = 1;
                }
                if(dgvPendapatan.Columns["NomorKamar"] != null)
                {
                    dgvPendapatan.Columns["NomorKamar"].HeaderText = "Kamar";
                    dgvPendapatan.Columns["NomorKamar"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    dgvPendapatan.Columns["NomorKamar"].DisplayIndex = 2;
                }
                if(dgvPendapatan.Columns["Jumlah"] != null)
                {
                    dgvPendapatan.Columns["Jumlah"].HeaderText = "Rp Jumlah";
                    dgvPendapatan.Columns["Jumlah"].DefaultCellStyle.Format = "C0";
                    dgvPendapatan.Columns["Jumlah"].DefaultCellStyle.FormatProvider = System.Globalization.CultureInfo.GetCultureInfo("id-ID");
                    dgvPendapatan.Columns["Jumlah"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    dgvPendapatan.Columns["Jumlah"].DisplayIndex = 3;
                }
                if(dgvPendapatan.Columns["Status"] != null)
                {
                     dgvPendapatan.Columns["Status"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                     dgvPendapatan.Columns["Status"].DisplayIndex = 4;
                }
            }
            // -----------------------

            // Calculate Total
            decimal total = list.Sum(p => p.Jumlah);
            lblTotalPendapatan.Text = $"Total Pendapatan: Rp {total:N0}";
            lblTotalPendapatan.ForeColor = UIHelper.PrimaryColor; // Match theme

            // Chart Logic: Group by Month-Year
            var groupedData = list
                .GroupBy(p => p.TanggalBayar.ToString("MMM yyyy"))
                .Select(g => new { Bulan = g.Key, Total = g.Sum(p => p.Jumlah) })
                .ToList();

            chartPendapatan.Series[0].Points.Clear();
            foreach (var item in groupedData)
            {
                chartPendapatan.Series[0].Points.AddXY(item.Bulan, item.Total);
            }
            
            chartPendapatan.Series[0].IsValueShownAsLabel = true;
            // Chart Styling
            chartPendapatan.BackColor = Color.WhiteSmoke;
            chartPendapatan.ChartAreas[0].BackColor = Color.Transparent;
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadStats();
        }
    }
}
