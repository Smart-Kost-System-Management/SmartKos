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
    public partial class FormRegistrasi : Form
    {
        public FormRegistrasi()
        {
            InitializeComponent();
            UIHelper.SetFormStyle(this);
            // Gradient Background
            this.Paint += (s, e) => UIHelper.PaintGradientBackground(this, e.Graphics, UIHelper.BackgroundColor, Color.White);

            UIHelper.StyleButton(btnSimpan, true); // Primary
            UIHelper.StyleButton(btnLogin, false); // Link to Login
            UIHelper.StyleButton(btnBatal, false); // Cancel
            
            // Custom styling for specific controls if needed
            groupBox1.ForeColor = UIHelper.PrimaryColor; // Make GroupBox title blue
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnSimpan_Click(object sender, EventArgs e)
        {
            // Validasi input kosong
            if (string.IsNullOrWhiteSpace(txtNama.Text) || string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Semua data harus diisi!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validasi role harus dipilih
            if (cmbRole.SelectedIndex == -1)
            {
                MessageBox.Show("Pilih role terlebih dahulu!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                Koneksi db = new Koneksi();
                using (MySqlConnection conn = db.GetConn())
                {
                    conn.Open();

                    // Gunakan NamaLengkap sesuai dengan header kolom di phpMyAdmin
                    string query = "INSERT INTO tbl_user (NamaLengkap, Username, Password, Role) VALUES (@nama, @user, @pwd, @role)";

                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    // Menghubungkan input form ke kolom database
                    cmd.Parameters.AddWithValue("@nama", txtNama.Text);
                    cmd.Parameters.AddWithValue("@user", txtUsername.Text);
                    cmd.Parameters.AddWithValue("@pwd", txtPassword.Text);
                    cmd.Parameters.AddWithValue("@role", cmbRole.Text);

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Registrasi Berhasil!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    FormLogin loginForm = new FormLogin();
                    loginForm.Show();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            FormLogin loginForm = new FormLogin();
            loginForm.Show();
            this.Close();
        }

        private void btnBatal_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
