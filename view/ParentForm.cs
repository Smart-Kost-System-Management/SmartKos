using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SmartKos.view;

namespace SmartKos.view
{
    public partial class ParentForm : Form
    {
        private model.M_User _currentUser;
        private FormDashboard _dashboard;
        private bool isLoggingOut = false; // Flag to handle splash-free logout

        public ParentForm(model.M_User user)
        {
            InitializeComponent();
            _currentUser = user;
            SetupMenu();
            OpenDashboard();
            this.Text = $"SmartKos Management System - {_currentUser.Nama}";
            this.WindowState = FormWindowState.Maximized;
        }

        // Default constructor for designer
        public ParentForm()
        {
            InitializeComponent();
        }

        private void SetupMenu()
        {
            // Clear existing menu from designer if any, or just use this code to build it dynamic
            menuStrip1.Items.Clear();
            UIHelper.StyleMenuStrip(menuStrip1);

            // 1. Menu Navigasi (Renamed to "Menu")
            ToolStripMenuItem navMenu = new ToolStripMenuItem("Menu");
            
            ToolStripMenuItem menuKamar = new ToolStripMenuItem("Monitoring Kamar");
            menuKamar.Click += (s, e) => {
                OpenDashboard();
                _dashboard?.SwitchView("Kamar");
            };
            
            navMenu.DropDownItems.Add(menuKamar);

            if (_currentUser.Role == "Admin" || _currentUser.Role == "Pemilik")
            {
                ToolStripMenuItem menuPenghuni = new ToolStripMenuItem("Data Penghuni");
                menuPenghuni.Click += (s, e) => {
                    OpenDashboard();
                    _dashboard?.SwitchView("Penghuni");
                };

                ToolStripMenuItem menuBayar = new ToolStripMenuItem("Data Pembayaran");
                menuBayar.Click += (s, e) => {
                    OpenDashboard();
                    _dashboard?.SwitchView("Pembayaran");
                };

                navMenu.DropDownItems.Add(menuPenghuni);
                navMenu.DropDownItems.Add(menuBayar);

                ToolStripMenuItem menuLaporan = new ToolStripMenuItem("Laporan & Statistik");
                menuLaporan.Click += (s, e) => OpenLaporan();
                navMenu.DropDownItems.Add(menuLaporan);

                // NEW: Permintaan Booking
                ToolStripMenuItem menuBooking = new ToolStripMenuItem("Permintaan Booking");
                menuBooking.Click += (s, e) => {
                    OpenDashboard();
                    _dashboard?.SwitchView("Booking");
                };
                navMenu.DropDownItems.Add(menuBooking);
            }

            // 2. Akun
            ToolStripMenuItem akunMenu = new ToolStripMenuItem("Akun");
            ToolStripMenuItem logoutItem = new ToolStripMenuItem("Logout");
            logoutItem.Click += (s, e) => {
                // Logout Logic: Show Login Form directly without restarting app (Skip Splash)
                isLoggingOut = true;
                
                // Close all other open forms
                if (_laporan != null && !_laporan.IsDisposed) _laporan.Close();
                if (_dashboard != null && !_dashboard.IsDisposed) _dashboard.Close();

                FormLogin login = new FormLogin();
                login.Show();
                
                this.Close(); // Close ParentForm
            };
            akunMenu.DropDownItems.Add(logoutItem);

            // 3. Exit (Far Right)
            ToolStripMenuItem exitItem = new ToolStripMenuItem("Exit");
            exitItem.Alignment = ToolStripItemAlignment.Right; // Move to far right
            exitItem.Click += (s, e) => Application.Exit();
            // Optional: Make it red
            exitItem.ForeColor = Color.Pink; 
            
            menuStrip1.Items.AddRange(new ToolStripItem[] { navMenu, akunMenu, exitItem });
            
            // Lifecycle Fix: Exit entire app when Main Form is closed, UNLESS logging out
            this.FormClosed += (s, e) => 
            {
                if (!isLoggingOut) Application.Exit();
            };
        }

        private void OpenDashboard()
        {
            if (_laporan != null && !_laporan.IsDisposed) _laporan.Hide();

            if (_dashboard == null || _dashboard.IsDisposed)
            {
                _dashboard = new FormDashboard(_currentUser);
                _dashboard.MdiParent = this;
                _dashboard.FormBorderStyle = FormBorderStyle.None;
                _dashboard.Dock = DockStyle.Fill;
                _dashboard.Show();
            }
            else
            {
                _dashboard.Show();
                _dashboard.Activate();
            }
        }

        private FormLaporan _laporan;
        private void OpenLaporan()
        {
            if (_dashboard != null && !_dashboard.IsDisposed) _dashboard.Hide();

            if (_laporan == null || _laporan.IsDisposed)
            {
                _laporan = new FormLaporan();
                _laporan.MdiParent = this;
                _laporan.FormBorderStyle = FormBorderStyle.None;
                _laporan.Dock = DockStyle.Fill;
                _laporan.Show();
            }
            else
            {
                _laporan.Show();
                _laporan.Activate();
            }
        }
    }
}
