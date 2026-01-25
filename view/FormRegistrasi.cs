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
using SmartKos.controller;
using SmartKos.view;

namespace SmartKos.view
{
    public partial class FormRegistrasi : Form
    {
        private UserController userController;

        public FormRegistrasi()
        {
            InitializeComponent();
            userController = new UserController();
            UIHelper.SetFormStyle(this);
            // Gradient Background
            this.Paint += (s, e) => UIHelper.PaintGradientBackground(this, e.Graphics, UIHelper.BackgroundColor, Color.White);

            UIHelper.StyleButton(btnSimpan, true); // Primary
            UIHelper.StyleButton(btnLogin, false); // Link to Login
            UIHelper.StyleButton(btnBatal, false); // Cancel
            
            // Custom styling for specific controls if needed
            groupBox1.ForeColor = UIHelper.PrimaryColor; // Make GroupBox title blue
        }

        }

        private void btnSimpan_Click(object sender, EventArgs e)
        {
            // Validasi input
            if (string.IsNullOrEmpty(txtNamaLengkap.Text) || string.IsNullOrEmpty(txtUsername.Text) || string.IsNullOrEmpty(txtPassword.Text) || cmbRole.SelectedIndex == -1)
            {
                MessageBox.Show("Semua data harus diisi!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Panggil Controller
            if (userController.TambahUser(txtNamaLengkap.Text, txtUsername.Text, txtPassword.Text, cmbRole.Text))
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
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            FormLogin loginForm = new FormLogin();
            loginForm.Show();
            this.Close();
        }
        {
            this.Close();
        }
    }
}
