using SmartKos.controller;
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
using SmartKos.view;

namespace SmartKos.view
{
    public partial class FormRegistrasi : Form
    {
        private UserController userController;
        // Use class level fields for controls we need to access
        private Label label4; 

        public FormRegistrasi()
        {
            InitializeComponent();
            userController = new UserController();
            UIHelper.SetFormStyle(this);
            
            // Gradient Background
            this.Paint += (s, e) => UIHelper.PaintGradientBackground(this, e.Graphics, UIHelper.BackgroundColor, Color.White);

            UIHelper.StyleButton(btnSimpan, true); 
            UIHelper.StyleButton(btnLogin, false); 
            UIHelper.StyleButton(btnBatal, false); 

            // 1. Form Settings
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Normal; 
            this.Size = new Size(1080, 720); 
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // RESPONSIVE LAYOUT (Centered Card)
            TableLayoutPanel mainLayout = new TableLayoutPanel();
            mainLayout.Dock = DockStyle.Fill;
            mainLayout.ColumnCount = 3;
            mainLayout.RowCount = 3;
            
            // Columns: 50% - Fixed(500) - 50%
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 500F)); 
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            
            // Rows: 50% - Fixed(600) - 50%
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 600F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            
            mainLayout.BackColor = Color.Transparent; 
            this.Controls.Add(mainLayout);

            // Container for Register Box - REPLACED GroupBox with Panel
            Panel registerBox = new Panel();
            registerBox.Parent = mainLayout;
            registerBox.Dock = DockStyle.Fill; 
            registerBox.BackColor = Color.White;
            registerBox.Padding = new Padding(20);
            
            // Fix: Set Rounded Region ON RESIZE
            registerBox.Resize += (s, e) => UIHelper.SetRoundedRegion(registerBox, 20); 
            
            // Hide original groupBox1 if it exists in Designer but we are replacing it manually
            if (groupBox1 != null) groupBox1.Visible = false;

            // Manual Layout inside registerBox
            Label lblTitle = new Label();
            lblTitle.Text = "REGISTRASI USER";
            lblTitle.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            lblTitle.ForeColor = UIHelper.PrimaryColor;
            lblTitle.AutoSize = false;
            lblTitle.Size = new Size(460, 50);
            lblTitle.Location = new Point(20, 30);
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            registerBox.Controls.Add(lblTitle);

            // Adjust Controls inside registerBox
            int labelX = 50;
            int inputX = 50;
            int width = 400;

            // We need to re-parent the existing designer controls to our new Panel
            // Labels
            label1.Parent = registerBox; label1.Location = new Point(labelX, 100); label1.Text = "Nama Lengkap";
            label2.Parent = registerBox; label2.Location = new Point(labelX, 180); label2.Text = "Username";
            label3.Parent = registerBox; label3.Location = new Point(labelX, 260); label3.Text = "Password";
            
            // Dynamic Label 4 (Role)
            label4 = new Label(); 
            label4.Text = "Role";
            label4.Location = new Point(labelX, 340);
            label4.AutoSize = true;
            label4.Font = UIHelper.MainFont;
            registerBox.Controls.Add(label4);

            // Inputs
            txtNamaLengkap.Parent = registerBox; txtNamaLengkap.Location = new Point(inputX, 125); txtNamaLengkap.Width = width; txtNamaLengkap.Height = 35;
            txtUsername.Parent = registerBox; txtUsername.Location = new Point(inputX, 205); txtUsername.Width = width; txtUsername.Height = 35;
            txtPassword.Parent = registerBox; txtPassword.Location = new Point(inputX, 285); txtPassword.Width = width; txtPassword.Height = 35;
            cmbRole.Parent = registerBox; cmbRole.Location = new Point(inputX, 365); cmbRole.Width = width; cmbRole.Height = 35;

            // Buttons
            btnSimpan.Parent = registerBox; 
            btnSimpan.Location = new Point(inputX, 430); 
            btnSimpan.Width = width; 
            btnSimpan.Height = 45;
            
            // "Sudah Punya Akun?"
            btnLogin.Parent = registerBox;
            btnLogin.Location = new Point(inputX, 490);
            btnLogin.Width = width;
            btnLogin.Height = 40;
            btnLogin.FlatStyle = FlatStyle.Flat; btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.BackColor = Color.Transparent; btnLogin.ForeColor = UIHelper.PrimaryColor;
            btnLogin.Font = new Font("Segoe UI", 9F, FontStyle.Underline);
            
            btnBatal.Visible = false; 
            
            // Add to center cell
            mainLayout.Controls.Add(registerBox, 1, 1);
            
            UIHelper.StyleTextBox(txtNamaLengkap);
            UIHelper.StyleTextBox(txtUsername);
            UIHelper.StyleTextBox(txtPassword);
            UIHelper.StyleComboBox(cmbRole);
        }

        private void btnSimpan_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtNamaLengkap.Text) || string.IsNullOrEmpty(txtUsername.Text) || string.IsNullOrEmpty(txtPassword.Text) || cmbRole.SelectedIndex == -1)
            {
                MessageBox.Show("Semua data harus diisi!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SmartKos.model.M_User newUser = new SmartKos.model.M_User
            {
                Nama = txtNamaLengkap.Text,
                Username = txtUsername.Text,
                Password = txtPassword.Text,
                Role = cmbRole.Text
            };

            if (userController.Add(newUser))
            {
                MessageBox.Show("Registrasi Berhasil!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                FormLogin loginForm = new FormLogin();
                loginForm.Show();
                this.Close();
            }
        }

        private void btnBatal_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            FormLogin loginForm = new FormLogin();
            loginForm.Show();
            this.Close();
        }
    }
}
