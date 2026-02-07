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
        private PenghuniController penghuniCtrl;
        private PembayaranController pembayaranCtrl;
        private BookingController bookingCtrl; // Added missing field
        private ExcelService excelSvc;
        private M_User _currentUser;
        private string _currentView = "Kamar"; // Kamar, Penghuni, Pembayaran
        private string _selectedId; // Track selected ID for Update/Delete

        // Dynamic Controls
        // Dynamic Controls (Menu Removed for MDI)
        private GroupBox _grpPenghuni;
        private GroupBox _grpPembayaran;
        
        // Penghuni Inputs
        private TextBox txtNamaP, txtKtpP, txtHpP;
        private ComboBox cmbKamarP;

        // Pembayaran Inputs
        private ComboBox cmbPenghuniBayar;
        private TextBox txtJumlahBayar;
        private ComboBox cmbStatusBayar;

        public FormDashboard(M_User user)
        {
            InitializeComponent();
            _currentUser = user;
            kamarCtrl = new Kamar();
            penghuniCtrl = new PenghuniController();
            pembayaranCtrl = new PembayaranController();
            bookingCtrl = new BookingController();
            excelSvc = new ExcelService();

            // Setup UI
            // Setup UI
            // SetupMenu(); // Removed for MDI
            SetupDynamicViews();
            
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

            // Bind Event for Pricing
            cmbTipe.SelectedIndexChanged += new EventHandler(cmbTipe_SelectedIndexChanged);
            
            // RBAC Check
            ApplyRBAC();
            
            this.Text = $"Dashboard SmartKos - {_currentUser.Nama} ({_currentUser.Role})";
        }

        // Default constructor for Designer support (or legacy calls), defaults to Admin for safety/testing
        public FormDashboard() : this(new M_User { Role = "Admin", Nama = "Admin Debug" }) { }
        
        // Add Constructor for main entry if missing or modify existing
        // It seems the main one is: public FormDashboard(M_User user)
        // Let's modify the Load or SetupDynamicViews to enforce MinimumSize
        
        private SplitContainer _splitContent; // Promote to field to access in Load

        private void FormDashboard_Load(object sender, EventArgs e)
        {
            // Enforce Minimum Size to prevent Layout Crash
            this.MinimumSize = new Size(800, 600);
            
            // Set initial view
            SwitchView("Kamar"); 
            
            // Safe Splitter Adjustment
            if (_splitContent != null)
            {
                try
                {
                    // Ensure container has valid width
                    if (_splitContent.Width > 200) 
                    {
                         int sidebarWidth = 380;
                         int targetDist = _splitContent.Width - sidebarWidth;
                         
                         // Determine valid range
                         int min1 = _splitContent.Panel1MinSize;
                         int min2 = _splitContent.Panel2MinSize; // Width - Panel2MinSize (max for splitter)
                         int maxDist = _splitContent.Width - min2;

                         // Clamp
                         if (targetDist < min1) targetDist = min1;
                         if (targetDist > maxDist) targetDist = maxDist;

                         _splitContent.SplitterDistance = targetDist;
                    }
                }
                catch 
                { 
                    // Silent Fail is better than Crash
                }
            }
        }

        // Menu Setup Removed - Moved to ParentForm

        private void SetupDynamicViews()
        {
            // 1. Main Layout (TableLayoutPanel)
            TableLayoutPanel mainLayout = new TableLayoutPanel();
            mainLayout.Dock = DockStyle.Fill;
            mainLayout.ColumnCount = 1;
            mainLayout.RowCount = 3;
            // Row 0: Visual Map (40%)
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 40F));
            // Row 1: Actions Toolbar (Fixed 55px)
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 55F));
            // Row 2: Data & Inputs (Remaining space)
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 60F));
            mainLayout.BackColor = Color.WhiteSmoke; 
            this.Controls.Add(mainLayout);

            // --- ROW 0: Visual Map (pnlKamar) ---
            pnlKamar.Parent = mainLayout;
            pnlKamar.Dock = DockStyle.Fill;
            pnlKamar.BackColor = Color.Transparent; 
            pnlKamar.BorderStyle = BorderStyle.None;
            mainLayout.Controls.Add(pnlKamar, 0, 0);
            
            if(label1 != null) label1.Visible = false; 

            // --- ROW 1: Actions Toolbar ---
            FlowLayoutPanel pnlToolbar = new FlowLayoutPanel();
            pnlToolbar.Dock = DockStyle.Fill;
            pnlToolbar.FlowDirection = FlowDirection.LeftToRight;
            pnlToolbar.Padding = new Padding(5);
            pnlToolbar.AutoSize = false;
            pnlToolbar.BackColor = Color.White;
            mainLayout.Controls.Add(pnlToolbar, 0, 1);

            // Toolbar Controls
            btnRefresh.Parent = pnlToolbar;
            btnRefresh.Margin = new Padding(10, 8, 10, 0);
            btnRefresh.Height = 35;
            btnRefresh.AutoSize = true; 
            
            Label lblCari = new Label { Text = "Cari:", AutoSize = true, TextAlign = ContentAlignment.MiddleRight };
            lblCari.Margin = new Padding(20, 15, 5, 0);
            lblCari.Font = UIHelper.MainFont;
            
            if(txtCari != null) {
                txtCari.Parent = pnlToolbar;
                txtCari.Margin = new Padding(0, 12, 10, 0);
                txtCari.Width = 250;
            }

            if(btnExport != null) {
                btnExport.Parent = pnlToolbar;
                btnExport.Margin = new Padding(20, 8, 0, 0);
                btnExport.Height = 35;
                btnExport.AutoSize = true;
                UIHelper.StyleButton(btnExport, true);
                btnExport.BackColor = UIHelper.SuccessColor; 
                btnExport.Text = "Export Excel";
            }
            
            pnlToolbar.Controls.Add(btnRefresh);
            pnlToolbar.Controls.Add(lblCari);
            pnlToolbar.Controls.Add(txtCari);
            pnlToolbar.Controls.Add(btnExport);
            
            if(groupBox2 != null) groupBox2.Visible = false;

            // --- ROW 2: Split Content (Grid vs Inputs) ---
            _splitContent = new SplitContainer(); // Use field
            _splitContent.Dock = DockStyle.Fill;
            _splitContent.Orientation = Orientation.Vertical;
            _splitContent.SplitterWidth = 8;
            
            // Settings
            _splitContent.FixedPanel = FixedPanel.Panel2; 
            // PREVENT CRASH: Reduce MinSize to allow layout calc on small screens
            _splitContent.Panel2MinSize = 100; 
            _splitContent.Panel1MinSize = 100; 
            
            mainLayout.Controls.Add(_splitContent, 0, 2);

            // Left: DataGrid
            dgvData.Parent = _splitContent.Panel1;
            dgvData.Dock = DockStyle.Fill;
            dgvData.BringToFront();

            // Right: Inputs
            Panel pnlInputs = new Panel();
            pnlInputs.Dock = DockStyle.Fill;
            pnlInputs.AutoScroll = true;
            pnlInputs.Padding = new Padding(5);
            pnlInputs.BackColor = Color.White;
            _splitContent.Panel2.Controls.Add(pnlInputs);

            // Action Buttons (Bottom)
            if(groupBox3 != null) {
                groupBox3.Parent = pnlInputs;
                groupBox3.Dock = DockStyle.Bottom;
                groupBox3.Height = 140;
                groupBox3.Text = "Aksi";
                
                groupBox3.Controls.Clear();
                TableLayoutPanel tblAction = new TableLayoutPanel();
                tblAction.Dock = DockStyle.Fill;
                tblAction.Padding = new Padding(5);
                tblAction.ColumnCount = 2;
                tblAction.RowCount = 2;
                tblAction.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
                tblAction.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
                tblAction.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
                tblAction.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
                
                groupBox3.Controls.Add(tblAction);
                
                // Style Buttons
                UIHelper.StyleButton(btnSimpan, true);
                UIHelper.StyleButton(btnUbah, false);
                UIHelper.StyleButton(btnHapus, false);
                UIHelper.StyleButton(btnClear, false);
                
                btnUbah.BackColor = UIHelper.WarningColor;
                btnHapus.BackColor = UIHelper.DangerColor;
                btnClear.BackColor = UIHelper.SecondaryColor;

                btnSimpan.Dock = DockStyle.Fill; btnSimpan.Margin = new Padding(3);
                btnUbah.Dock = DockStyle.Fill; btnUbah.Margin = new Padding(3);
                btnHapus.Dock = DockStyle.Fill; btnHapus.Margin = new Padding(3);
                btnClear.Dock = DockStyle.Fill; btnClear.Margin = new Padding(3);

                tblAction.Controls.Add(btnClear, 0, 0); tblAction.Controls.Add(btnSimpan, 1, 0);
                tblAction.Controls.Add(btnHapus, 0, 1); tblAction.Controls.Add(btnUbah, 1, 1);
            }

            // Input Groups (Top)
            // 1. Kamar Input
            if(groupBox1 != null) {
                groupBox1.Parent = pnlInputs;
                groupBox1.Dock = DockStyle.Top;
                groupBox1.Height = 250; 
                groupBox1.Visible = true; 
            }

            // 2. Penghuni Input
            _grpPenghuni = new GroupBox();
            _grpPenghuni.Text = "Input Data Penghuni";
            _grpPenghuni.Dock = DockStyle.Top;
            _grpPenghuni.Height = 250;
            _grpPenghuni.Visible = false;
            pnlInputs.Controls.Add(_grpPenghuni);

            TableLayoutPanel tblPenghuni = new TableLayoutPanel();
            tblPenghuni.Dock = DockStyle.Fill;
            tblPenghuni.Padding = new Padding(5);
            tblPenghuni.ColumnCount = 2;
            tblPenghuni.RowCount = 4;
            tblPenghuni.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F)); 
            tblPenghuni.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65F));
            for(int i=0; i<4; i++) tblPenghuni.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            
            _grpPenghuni.Controls.Add(tblPenghuni);

            Label lblNama = new Label { Text = "Nama", AutoSize = true, Anchor = AnchorStyles.Right, TextAlign = ContentAlignment.MiddleRight };
            txtNamaP = new TextBox { Anchor = AnchorStyles.Left | AnchorStyles.Right };
            Label lblKtp = new Label { Text = "No KTP", AutoSize = true, Anchor = AnchorStyles.Right, TextAlign = ContentAlignment.MiddleRight };
            txtKtpP = new TextBox { Anchor = AnchorStyles.Left | AnchorStyles.Right };
            Label lblHp = new Label { Text = "No HP", AutoSize = true, Anchor = AnchorStyles.Right, TextAlign = ContentAlignment.MiddleRight };
            txtHpP = new TextBox { Anchor = AnchorStyles.Left | AnchorStyles.Right };
            Label lblKamar = new Label { Text = "Kamar", AutoSize = true, Anchor = AnchorStyles.Right, TextAlign = ContentAlignment.MiddleRight };
            cmbKamarP = new ComboBox { Anchor = AnchorStyles.Left | AnchorStyles.Right, DropDownStyle = ComboBoxStyle.DropDownList };

            tblPenghuni.Controls.Add(lblNama, 0, 0); tblPenghuni.Controls.Add(txtNamaP, 1, 0);
            tblPenghuni.Controls.Add(lblKtp, 0, 1); tblPenghuni.Controls.Add(txtKtpP, 1, 1);
            tblPenghuni.Controls.Add(lblHp, 0, 2); tblPenghuni.Controls.Add(txtHpP, 1, 2);
            tblPenghuni.Controls.Add(lblKamar, 0, 3); tblPenghuni.Controls.Add(cmbKamarP, 1, 3);

            // 3. Pembayaran Input
            _grpPembayaran = new GroupBox();
            _grpPembayaran.Text = "Input Data Pembayaran";
            _grpPembayaran.Dock = DockStyle.Top;
            _grpPembayaran.Height = 220;
            _grpPembayaran.Visible = false;
            pnlInputs.Controls.Add(_grpPembayaran);

            TableLayoutPanel tblBayar = new TableLayoutPanel();
            tblBayar.Dock = DockStyle.Fill;
            tblBayar.Padding = new Padding(5);
            tblBayar.ColumnCount = 2;
            tblBayar.RowCount = 3;
            tblBayar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
            tblBayar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65F));
            for(int i=0; i<3; i++) tblBayar.RowStyles.Add(new RowStyle(SizeType.Percent, 33F));

            _grpPembayaran.Controls.Add(tblBayar);

            Label lblPenghuni = new Label { Text = "Penghuni", AutoSize = true, Anchor = AnchorStyles.Right, TextAlign = ContentAlignment.MiddleRight };
            cmbPenghuniBayar = new ComboBox { Anchor = AnchorStyles.Left | AnchorStyles.Right, DropDownStyle = ComboBoxStyle.DropDownList };
            Label lblJml = new Label { Text = "Jumlah", AutoSize = true, Anchor = AnchorStyles.Right, TextAlign = ContentAlignment.MiddleRight };
            txtJumlahBayar = new TextBox { Anchor = AnchorStyles.Left | AnchorStyles.Right };
            Label lblSts = new Label { Text = "Status", AutoSize = true, Anchor = AnchorStyles.Right, TextAlign = ContentAlignment.MiddleRight };
            cmbStatusBayar = new ComboBox { Anchor = AnchorStyles.Left | AnchorStyles.Right, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbStatusBayar.Items.AddRange(new string[] { "Lunas", "Belum Lunas" });

            tblBayar.Controls.Add(lblPenghuni, 0, 0); tblBayar.Controls.Add(cmbPenghuniBayar, 1, 0);
            tblBayar.Controls.Add(lblJml, 0, 1); tblBayar.Controls.Add(txtJumlahBayar, 1, 1);
            tblBayar.Controls.Add(lblSts, 0, 2); tblBayar.Controls.Add(cmbStatusBayar, 1, 2);
        }

        private void ApplyRBAC()
        {
            if (_currentUser.Role == "User")
            {
                // User only sees Monitoring
                groupBox1.Visible = false; // Cant Add Kamar
                groupBox3.Visible = false; // Cant Edit/Delete
                // Menu already filtered in SetupMenu
            }
        }

        public void SwitchView(string view)
        {
            _currentView = view;
            
            // Toggle Logic
            pnlKamar.Visible = (view == "Kamar");
            groupBox1.Visible = (view == "Kamar" && _currentUser.Role != "User");
            
            _grpPenghuni.Visible = (view == "Penghuni");
            _grpPembayaran.Visible = (view == "Pembayaran");
            // Booking has no specific input group (uses Alert/Popup on click)
            if (view == "Booking") 
            {
                groupBox3.Visible = true; // Enable Action Buttons (for Approve)
                btnSimpan.Enabled = false; // Cannot Add Manually
                btnUbah.Text = "Approve"; // Repurpose Ubah
                btnUbah.BackColor = UIHelper.SuccessColor;
                btnHapus.Text = "Reject";
            }
            else
            {
                // Reset Buttons
                btnSimpan.Enabled = true;
                btnUbah.Text = "Ubah";
                btnUbah.BackColor = UIHelper.WarningColor;
                btnHapus.Text = "Hapus";
            }

            // Refresh Grid & Inputs
            RefreshSemua();
        }

        private void RefreshSemua()
        {
            LoadDataGrid();
            if (_currentView == "Kamar") LoadDataVisual();
            BersihkanInput();
        }

        private void LoadDataGrid()
        {
            if (_currentView == "Kamar")
            {
                dgvData.DataSource = kamarCtrl.GetAll();
                // Column formatting for Kamar...
            }
            else if (_currentView == "Penghuni")
            {
                dgvData.DataSource = penghuniCtrl.GetAll();
            }
            else if (_currentView == "Pembayaran")
            {
                dgvData.DataSource = pembayaranCtrl.GetAll();
            }
            else if (_currentView == "Booking")
            {
                dgvData.DataSource = bookingCtrl.GetAllBookings();
            }
        }

        private void LoadDataVisual()
        {
            pnlKamar.Controls.Clear();
            
            // Header for Visual Map
            Label lblHeader = new Label();
            lblHeader.Text = "DENAH KAMAR REAL-TIME";
            lblHeader.Font = UIHelper.HeaderFont;
            lblHeader.Dock = DockStyle.Top;
            lblHeader.Height = 40;
            lblHeader.TextAlign = ContentAlignment.MiddleLeft;
            lblHeader.Padding = new Padding(10,0,0,0);
            pnlKamar.Controls.Add(lblHeader);

            // Use FlowLayoutPanel
            FlowLayoutPanel flowMap = new FlowLayoutPanel();
            flowMap.Dock = DockStyle.Fill;
            flowMap.AutoScroll = true;
            flowMap.Padding = new Padding(15);
            flowMap.BackColor = Color.White;
            pnlKamar.Controls.Add(flowMap);
            flowMap.BringToFront();

            List<M_kamar> list = kamarCtrl.GetStatusKamar();

            foreach (var item in list)
            {
                Button btn = new Button();
                // Simpler text for cleanness
                btn.Text = $"{item.NomorKamar}\n{item.TipeKamar}\n{item.Status}";
                btn.Size = new Size(110, 90); // Slightly smaller
                btn.Margin = new Padding(8);
                btn.TextAlign = ContentAlignment.MiddleCenter;
                
                UIHelper.StyleButton(btn, false);
                btn.Height = 90; 
                btn.Font = new Font("Segoe UI", 9F, FontStyle.Bold);

                if (item.Status == "Terisi")
                {
                    btn.BackColor = UIHelper.DangerColor; 
                    btn.ForeColor = Color.White;
                }
                else if (item.Status == "Kosong")
                {
                    btn.BackColor = UIHelper.SuccessColor; 
                    btn.ForeColor = Color.White;
                }
                else 
                {
                    btn.BackColor = UIHelper.WarningColor; 
                    btn.ForeColor = Color.White;
                }

                UIHelper.SetRoundedRegion(btn, 12);

                btn.Click += (s, ev) => {
                    if (_currentUser.Role == "User")
                    {
                         if (item.Status == "Kosong")
                         {
                             // Open Request Form
                             FormBookingRequest frm = new FormBookingRequest(item.KamarID, item.NomorKamar, _currentUser.Id);
                             if (frm.ShowDialog() == DialogResult.OK)
                             {
                                 if (bookingCtrl.AddBooking(frm.BookingData))
                                 {
                                     MessageBox.Show("Booking Terkirim! Menunggu persetujuan Admin.\nData anda akan masuk ke sistem Penghuni setelah disetujui.", "Sukses");
                                 }
                             }
                         }
                         else MessageBox.Show($"Kamar {item.NomorKamar} - {item.Status}.", "Info");
                    }
                    else
                    {
                        txtNoKamar.Text = item.NomorKamar;
                        cmbStatus.Text = item.Status;
                        cmbTipe.Text = item.TipeKamar;
                        txtHarga.Text = item.Harga.ToString();
                        _selectedId = item.NomorKamar;
                    }
                };

                flowMap.Controls.Add(btn);
            }
        }

        private void BersihkanInput()
        {
            // Clear Kamar
            txtNoKamar.Clear();
            txtHarga.Clear();
            cmbTipe.SelectedIndex = -1;
            cmbStatus.SelectedIndex = -1;
            
            // Clear Penghuni
            if (txtNamaP != null) {
                txtNamaP.Clear(); txtKtpP.Clear(); txtHpP.Clear(); 
                
                // Reload Kamar ComboBox with Binding
                var kamars = kamarCtrl.GetStatusKamar();
                // Optional: Filter for Kosong only during Add, but for now show all to allow editing existing
                cmbKamarP.DataSource = kamars;
                cmbKamarP.DisplayMember = "NomorKamar";
                cmbKamarP.ValueMember = "KamarId";
                cmbKamarP.SelectedIndex = -1;
            }

            // Clear Pembayaran
            if (txtJumlahBayar != null) {
               txtJumlahBayar.Clear(); cmbStatusBayar.SelectedIndex = -1; cmbPenghuniBayar.SelectedIndex = -1;
               // Reload Penghuni Combo
               cmbPenghuniBayar.DataSource = penghuniCtrl.GetAll();
               cmbPenghuniBayar.DisplayMember = "Nama";
               cmbPenghuniBayar.ValueMember = "PenghuniID";
            }
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
            if (string.IsNullOrEmpty(_selectedId)) { MessageBox.Show("Pilih data dulu!"); return; }

            if (MessageBox.Show("Hapus data ini?", "Konfirmasi", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                bool success = false;
                if (_currentView == "Kamar") success = kamarCtrl.Delete(_selectedId);
                else if (_currentView == "Penghuni") success = penghuniCtrl.Delete(_selectedId);
                else if (_currentView == "Pembayaran") success = pembayaranCtrl.Delete(_selectedId);

                if (success) { MessageBox.Show("Data Deleted"); RefreshSemua(); }
            }
        }

        private void btnSimpan_Click(object sender, EventArgs e)
        {
            if (_currentView == "Kamar")
            {
                if (string.IsNullOrEmpty(txtNoKamar.Text)) return;
                M_kamar newKamar = new M_kamar();
                newKamar.NomorKamar = txtNoKamar.Text;
                newKamar.TipeKamar = cmbTipe.Text;
                int.TryParse(txtHarga.Text, out int h); newKamar.Harga = h;
                newKamar.Status = cmbStatus.Text;

                if (kamarCtrl.Add(newKamar)) { MessageBox.Show("Kamar Saved"); RefreshSemua(); }
            }
            else if (_currentView == "Penghuni")
            {
                if (cmbKamarP.SelectedIndex == -1) // Validation: Check if Kamar is selected
                {
                    MessageBox.Show("Pilih Kamar terlebih dahulu!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                M_Penghuni p = new M_Penghuni();
                p.Nama = txtNamaP.Text;
                p.NoKTP = txtKtpP.Text;
                p.NoHP = txtHpP.Text;
                
                if(cmbKamarP.SelectedValue != null) {
                     p.KamarID = Convert.ToInt32(cmbKamarP.SelectedValue);
                }
                
                if (penghuniCtrl.Add(p)) { MessageBox.Show("Penghuni Saved"); RefreshSemua(); }
            }
            else if (_currentView == "Pembayaran")
            {
                 if (cmbPenghuniBayar.SelectedIndex == -1) // Validation: Check if Penghuni is selected
                 {
                    MessageBox.Show("Pilih Penghuni terlebih dahulu!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                 }

                 M_Pembayaran py = new M_Pembayaran();
                 if (cmbPenghuniBayar.SelectedValue != null)
                    py.PenghuniID = Convert.ToInt32(cmbPenghuniBayar.SelectedValue);
                 decimal.TryParse(txtJumlahBayar.Text, out decimal j); py.Jumlah = j;
                 py.Status = cmbStatusBayar.Text;
                 
                 if (pembayaranCtrl.Add(py)) { MessageBox.Show("Pembayaran Saved"); RefreshSemua(); }
            }
        }

        private void btnUbah_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedId)) { MessageBox.Show("Pilih data dulu!"); return; }

            if (_currentView == "Kamar")
            {
                M_kamar updateKamar = new M_kamar();
                updateKamar.NomorKamar = txtNoKamar.Text;
                updateKamar.TipeKamar = cmbTipe.Text;
                int.TryParse(txtHarga.Text, out int h); updateKamar.Harga = h;
                updateKamar.Status = cmbStatus.Text;

                if (kamarCtrl.Update(updateKamar)) { MessageBox.Show("Kamar Updated"); RefreshSemua(); }
            }
            else if (_currentView == "Penghuni")
            {
                M_Penghuni p = new M_Penghuni();
                p.PenghuniID = int.Parse(_selectedId);
                p.Nama = txtNamaP.Text;
                p.NoKTP = txtKtpP.Text;
                p.NoHP = txtHpP.Text;
                
                if (penghuniCtrl.Update(p)) { MessageBox.Show("Penghuni Updated"); RefreshSemua(); }
            }
            else if (_currentView == "Pembayaran")
            {
                M_Pembayaran py = new M_Pembayaran();
                py.PembayaranID = int.Parse(_selectedId);
                if (cmbPenghuniBayar.SelectedValue != null)
                   py.PenghuniID = Convert.ToInt32(cmbPenghuniBayar.SelectedValue);
                decimal.TryParse(txtJumlahBayar.Text, out decimal j); py.Jumlah = j;
                py.Status = cmbStatusBayar.Text;

                if (pembayaranCtrl.Update(py)) { MessageBox.Show("Pembayaran Updated"); RefreshSemua(); }
            }
            else if (_currentView == "Booking")
            {
                // Admin Approves Booking
                if (MessageBox.Show("Approve booking ini?", "Konfirmasi", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    if (dgvData.CurrentRow != null)
                    {
                        // Get KamarID from current row safely
                        var cellVal = dgvData.CurrentRow.Cells["KamarID"].Value;
                        if(cellVal != null)
                        {
                            int kid = Convert.ToInt32(cellVal);
                            if(bookingCtrl.UpdateStatus(int.Parse(_selectedId), "Approved", kid))
                            {
                                MessageBox.Show("Booking Approved! Data Penghuni telah dibuat."); 
                                RefreshSemua();
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Pilih data booking terlebih dahulu.");
                    }
                }
            }
        }

        private void dgvData_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dgvData.Rows.Count)
            {
                DataGridViewRow row = dgvData.Rows[e.RowIndex];
                
                try
                {
                    if (_currentView == "Kamar")
                    {
                        if(row.Cells["NomorKamar"].Value != null) 
                        {
                             txtNoKamar.Text = row.Cells["NomorKamar"].Value.ToString();
                             cmbTipe.Text = row.Cells["TipeKamar"].Value.ToString();
                             txtHarga.Text = row.Cells["Harga"].Value.ToString();
                             cmbStatus.Text = row.Cells["Status"].Value.ToString();
                             _selectedId = txtNoKamar.Text;
                        }
                    }
                    else if (_currentView == "Penghuni")
                    {
                         if(row.Cells["PenghuniID"].Value != null)
                         {
                             _selectedId = row.Cells["PenghuniID"].Value.ToString();
                             txtNamaP.Text = row.Cells["Nama"].Value.ToString();
                             txtKtpP.Text = row.Cells["NoKTP"].Value.ToString();
                             txtHpP.Text = row.Cells["NoHP"].Value.ToString();
                             if(row.Cells["KamarID"].Value != DBNull.Value)
                                 cmbKamarP.SelectedValue = Convert.ToInt32(row.Cells["KamarID"].Value);
                         }
                    }
                    else if (_currentView == "Pembayaran")
                    {
                         if(row.Cells["PembayaranID"].Value != null)
                         {
                             _selectedId = row.Cells["PembayaranID"].Value?.ToString();
                             txtJumlahBayar.Text = row.Cells["Jumlah"].Value?.ToString();
                             cmbStatusBayar.Text = row.Cells["Status"].Value?.ToString();
                             cmbPenghuniBayar.SelectedValue = row.Cells["PenghuniID"].Value;
                         }
                    }
                    // Handle "Booking" View safely
                    else if (_currentView == "Booking")
                    {
                        // Check if column exists to prevent crash
                        if (dgvData.Columns.Contains("BookingID") && row.Cells["BookingID"].Value != null)
                        {
                            _selectedId = row.Cells["BookingID"].Value.ToString();
                            
                            // Optional: Show detail in inputs if needed, or just keep ID for Approve
                            // Since we don't have specific Booking inputs, we just select the ID.
                        }
                    }
                }
                catch(Exception ex) 
                { 
                    // Prevent crash on missing columns
                    Console.WriteLine("Grid Click Error: " + ex.Message); 
                }
            }
        }

        private void txtCari_TextChanged(object sender, EventArgs e)
        {
             if (_currentView == "Kamar") dgvData.DataSource = kamarCtrl.Search(txtCari.Text);
             else if (_currentView == "Penghuni") dgvData.DataSource = penghuniCtrl.Search(txtCari.Text);
             else if (_currentView == "Pembayaran") dgvData.DataSource = pembayaranCtrl.Search(txtCari.Text);
        }

        private void cmbTipe_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbTipe.SelectedIndex == -1) return;

            string tipe = cmbTipe.SelectedItem.ToString();
            switch (tipe)
            {
                case "Non-AC":
                    txtHarga.Text = "850000";
                    break;
                case "AC":
                    txtHarga.Text = "1500000";
                    break;
                case "VVIP":
                    txtHarga.Text = "2500000";
                    break;
            }
        }
    }
}
