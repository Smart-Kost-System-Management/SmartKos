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
using SmartKos.controller;

namespace SmartKos.view
{
    public partial class FormLogin : Form
    {
        public FormLogin()
        {
            InitializeComponent();
            UIHelper.SetFormStyle(this);
            // Gradient Background
            this.Paint += (s, e) => UIHelper.PaintGradientBackground(this, e.Graphics, UIHelper.BackgroundColor, Color.White);

            UIHelper.StyleLabel(label1, true); // Title
            UIHelper.StyleButton(btnLogin, true); // Primary
            UIHelper.StyleButton(button1, false); // Register (Secondary)

            // 1. Form Settings
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Normal; // Disable Full Screen
            this.Size = new Size(1080, 720); // Standard Window Size
            this.FormBorderStyle = FormBorderStyle.FixedSingle; // Prevent resizing to keep layout consistent
            this.MaximizeBox = false; // Disable maximize button
            this.Text = "Login Smart Kos";

            // 2. Main Layout (Centered)
            TableLayoutPanel mainLayout = new TableLayoutPanel();
            mainLayout.Dock = DockStyle.Fill;
            mainLayout.ColumnCount = 3;
            mainLayout.RowCount = 3;
            
            // Columns: 50% - Fixed(420) - 50%
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 420F)); 
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            
            // Rows: 50% - Fixed(500) - 50%
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 500F)); 
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            
            mainLayout.BackColor = Color.Transparent;
            this.Controls.Add(mainLayout);

            // 3. Login Container (Card)
            Panel loginBox = new Panel();
            loginBox.Dock = DockStyle.Fill;
            loginBox.BackColor = Color.White;
            loginBox.Padding = new Padding(30);
            
            // Fix: Set Rounded Region ON RESIZE to avoid clipping
            loginBox.Resize += (s, e) => UIHelper.SetRoundedRegion(loginBox, 20);
            // Also call once initially *after* layout if possible, or just let Resize handle it.
            // But to be safe, we don't set it immediately to avoid 0-size clip.

            // 4. Controls inside Card
            // HEADER "LOGIN"
            Label lblHeader = new Label();
            lblHeader.Text = "LOGIN";
            lblHeader.Font = new Font("Segoe UI", 24F, FontStyle.Bold); 
            lblHeader.ForeColor = UIHelper.PrimaryColor;
            lblHeader.AutoSize = false;
            lblHeader.Size = new Size(360, 60);
            lblHeader.Location = new Point(30, 30);
            lblHeader.TextAlign = ContentAlignment.MiddleCenter;
            loginBox.Controls.Add(lblHeader);

            // Subtitle / App Name
            label1.Parent = loginBox;
            label1.Text = "Smart Kos Management";
            label1.Font = new Font("Segoe UI", 12F, FontStyle.Regular);
            label1.ForeColor = Color.Gray;
            label1.AutoSize = false;
            label1.Size = new Size(360, 30);
            label1.Location = new Point(30, 80);
            label1.TextAlign = ContentAlignment.MiddleCenter;

            // Username
            label2.Parent = loginBox;
            label2.Location = new Point(40, 140);
            
            txtUsername.Parent = loginBox;
            txtUsername.Location = new Point(40, 165);
            txtUsername.Width = 340;
            txtUsername.Height = 35;

            // Password
            label3.Parent = loginBox;
            label3.Location = new Point(40, 220);
            
            txtPassword.Parent = loginBox;
            txtPassword.Location = new Point(40, 245);
            txtPassword.Width = 340;
            txtPassword.Height = 35;
            txtPassword.PasswordChar = '•'; 

            // Buttons
            btnLogin.Parent = loginBox;
            btnLogin.Location = new Point(40, 310);
            btnLogin.Width = 340;
            btnLogin.Height = 45; 
            
            button1.Parent = loginBox; 
            button1.Location = new Point(40, 370);
            button1.Width = 340;
            button1.Height = 40;
            button1.Text = "Belum punya akun? Daftar disini";
            button1.FlatStyle = FlatStyle.Flat; button1.FlatAppearance.BorderSize = 0;
            button1.BackColor = Color.Transparent; button1.ForeColor = UIHelper.PrimaryColor;
            button1.Font = new Font("Segoe UI", 9F, FontStyle.Underline);
            
            // Add loginBox to Center Cell (1,1)
            mainLayout.Controls.Add(loginBox, 1, 1);

            // Lifecycle Fix: Exit app when Login form is closed
            this.FormClosed += (s, e) =>
            {
                if (this.Visible) Application.Exit();
            };
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string user = txtUsername.Text;
            string pass = txtPassword.Text;

            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
            {
                MessageBox.Show("Username dan Password harus diisi!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                UserController controller = new UserController();
                SmartKos.model.M_User loggedUser = controller.Login(user, pass);

                if (loggedUser != null)
                {
                    MessageBox.Show($"Selamat Datang, {loggedUser.Nama} ({loggedUser.Role})!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ParentForm mainForm = new ParentForm(loggedUser); 
                    mainForm.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Username atau Password salah!", "Gagal", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtPassword.Clear();
                    txtPassword.Focus();
                }
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText("login_error.txt", ex.ToString());
                MessageBox.Show("Terjadi Kesalahan: " + ex.Message + "\nDetail tersimpan di login_error.txt", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            FormRegistrasi regForm = new FormRegistrasi();
            regForm.Show();
            this.Hide();
        }
    }
}
