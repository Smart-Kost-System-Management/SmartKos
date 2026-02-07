namespace SmartKos.view
{
    partial class FormLaporan
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title1 = new System.Windows.Forms.DataVisualization.Charting.Title();
            
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title2 = new System.Windows.Forms.DataVisualization.Charting.Title();

            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabKamar = new System.Windows.Forms.TabPage();
            this.chartKamar = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.tabPendapatan = new System.Windows.Forms.TabPage();
            this.chartPendapatan = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.dgvPendapatan = new System.Windows.Forms.DataGridView();
            this.lblTotalPendapatan = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnRefresh = new System.Windows.Forms.Button();

            this.tabControl1.SuspendLayout();
            this.tabKamar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartKamar)).BeginInit();
            this.tabPendapatan.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartPendapatan)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPendapatan)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();

            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabKamar);
            this.tabControl1.Controls.Add(this.tabPendapatan);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 50);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(800, 400);
            this.tabControl1.TabIndex = 0;

            // 
            // tabKamar
            // 
            this.tabKamar.Controls.Add(this.chartKamar);
            this.tabKamar.Location = new System.Drawing.Point(4, 22);
            this.tabKamar.Name = "tabKamar";
            this.tabKamar.Padding = new System.Windows.Forms.Padding(3);
            this.tabKamar.Size = new System.Drawing.Size(792, 374);
            this.tabKamar.TabIndex = 0;
            this.tabKamar.Text = "Statistik Kamar";
            this.tabKamar.UseVisualStyleBackColor = true;

            // 
            // chartKamar
            // 
            chartArea1.Name = "ChartArea1";
            this.chartKamar.ChartAreas.Add(chartArea1);
            this.chartKamar.Dock = System.Windows.Forms.DockStyle.Fill;
            legend1.Name = "Legend1";
            this.chartKamar.Legends.Add(legend1);
            this.chartKamar.Location = new System.Drawing.Point(3, 3);
            this.chartKamar.Name = "chartKamar";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Pie;
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.chartKamar.Series.Add(series1);
            this.chartKamar.Size = new System.Drawing.Size(786, 368);
            this.chartKamar.TabIndex = 0;
            this.chartKamar.Text = "chartKamar";
            title1.Name = "Title1";
            title1.Text = "Status Ketersediaan Kamar";
            this.chartKamar.Titles.Add(title1);

            // 
            // tabPendapatan
            // 
            this.tabPendapatan.Controls.Add(this.chartPendapatan);
            this.tabPendapatan.Controls.Add(this.lblTotalPendapatan);
            this.tabPendapatan.Controls.Add(this.dgvPendapatan);
            this.tabPendapatan.Location = new System.Drawing.Point(4, 22);
            this.tabPendapatan.Name = "tabPendapatan";
            this.tabPendapatan.Padding = new System.Windows.Forms.Padding(3);
            this.tabPendapatan.Size = new System.Drawing.Size(792, 374);
            this.tabPendapatan.TabIndex = 1;
            this.tabPendapatan.Text = "Laporan Pendapatan";
            this.tabPendapatan.UseVisualStyleBackColor = true;

            // 
            // chartPendapatan
            // 
            chartArea2.Name = "ChartArea1";
            this.chartPendapatan.ChartAreas.Add(chartArea2);
            this.chartPendapatan.Dock = System.Windows.Forms.DockStyle.Fill;
            legend2.Name = "Legend1";
            this.chartPendapatan.Legends.Add(legend2);
            this.chartPendapatan.Location = new System.Drawing.Point(3, 3);
            this.chartPendapatan.Name = "chartPendapatan";
            series2.ChartArea = "ChartArea1";
            series2.Legend = "Legend1";
            series2.Name = "Pendapatan";
            this.chartPendapatan.Series.Add(series2);
            this.chartPendapatan.Size = new System.Drawing.Size(400, 368);
            this.chartPendapatan.TabIndex = 0;
            this.chartPendapatan.Text = "chartPendapatan";
            title2.Name = "Title1";
            title2.Text = "Pendapatan Per Bulan";
            this.chartPendapatan.Titles.Add(title2);
            this.chartPendapatan.Dock = System.Windows.Forms.DockStyle.Right;
            this.chartPendapatan.Width = 400;

            // 
            // dgvPendapatan
            // 
            this.dgvPendapatan.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPendapatan.Dock = System.Windows.Forms.DockStyle.Left;
            this.dgvPendapatan.Location = new System.Drawing.Point(3, 3);
            this.dgvPendapatan.Name = "dgvPendapatan";
            this.dgvPendapatan.Size = new System.Drawing.Size(380, 368);
            this.dgvPendapatan.TabIndex = 1;
            this.dgvPendapatan.Width = 380;

             // 
            // lblTotalPendapatan
            // 
            this.lblTotalPendapatan.AutoSize = true;
            this.lblTotalPendapatan.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalPendapatan.Location = new System.Drawing.Point(10, 340);
            this.lblTotalPendapatan.Name = "lblTotalPendapatan";
            this.lblTotalPendapatan.Size = new System.Drawing.Size(150, 20);
            this.lblTotalPendapatan.TabIndex = 2;
            this.lblTotalPendapatan.Text = "Total: Rp 0";
            this.lblTotalPendapatan.Dock = System.Windows.Forms.DockStyle.Bottom;

            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnRefresh);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(800, 50);
            this.panel1.TabIndex = 1;

            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(12, 12);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 23);
            this.btnRefresh.TabIndex = 0;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);

            // 
            // FormLaporan
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.panel1);
            this.Name = "FormLaporan";
            this.Text = "Laporan & Statistik";
            this.Load += new System.EventHandler(this.FormLaporan_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabKamar.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chartKamar)).EndInit();
            this.tabPendapatan.ResumeLayout(false);
            this.tabPendapatan.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartPendapatan)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPendapatan)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabKamar;
        private System.Windows.Forms.TabPage tabPendapatan;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartKamar;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartPendapatan;
        private System.Windows.Forms.DataGridView dgvPendapatan;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Label lblTotalPendapatan;
    }
}
