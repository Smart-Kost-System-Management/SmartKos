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
            
            // Round TextBoxes (workaround as we can't easily set region on textbox directly without cutting text sometimes, 
            // but let's try or just rely on the flat border style from Helper)
            // Actually, let's just leave textboxes flat with single border but ensure they have padding if possible.
            // For now, we focus on the buttons and background.
            
            // Adjust spacing if needed
            label1.Top = 30; // More space at top
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            // 1. Ambil data dari TextBox
            string user = txtUsername.Text;
            string pass = txtPassword.Text;

            // 2. Validasi Sederhana: Tidak boleh kosong
            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
            {
                MessageBox.Show("Username dan Password harus diisi!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 3. Cek ke Database (Logika Login)
            try
            {
                Koneksi db = new Koneksi(); // Memanggil class Koneksi dari folder Controller
                using (MySqlConnection conn = db.GetConn())
                {
                    conn.Open();

                    // Query cek user & password
                    string query = "SELECT * FROM Tbl_User WHERE Username=@uid AND Password=@pwd";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    // Pakai parameter agar aman dari SQL Injection
                    cmd.Parameters.AddWithValue("@uid", user);
                    cmd.Parameters.AddWithValue("@pwd", pass);

                    MySqlDataReader dr = cmd.ExecuteReader();

                    if (dr.Read())
                    {
                        // --- LOGIN SUKSES ---
                        MessageBox.Show("Login Berhasil!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Panggil Form Utama (FrmMain)
                        // Pastikan Anda sudah membuat Form bernama FrmMain di folder View
                        FormDashboard mainForm = new FormDashboard();
                        mainForm.Show();

                        // Sembunyikan Form Login ini
                        this.Hide();
                    }
                    else
                    {
                        // --- LOGIN GAGAL ---
                        MessageBox.Show("Username atau Password salah!", "Gagal", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtPassword.Clear();
                        txtPassword.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Terjadi Kesalahan Database: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
}

