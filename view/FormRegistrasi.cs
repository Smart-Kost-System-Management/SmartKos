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

        }

        private void btnSimpan_Click(object sender, EventArgs e)
        {
            // 1. Validasi Input (Abaikan txtNama karena tidak disimpan ke database)
            if (string.IsNullOrEmpty(txtUsername.Text) || string.IsNullOrEmpty(txtPassword.Text) || cmbRole.SelectedIndex == -1)
            {
                MessageBox.Show("Username, Password, dan Role wajib diisi!", "Peringatan");
                return;
            }

            // 2. Panggil Controller
            UserController userController = new UserController();

            // Kirim data sesuai urutan parameter di UserController (user, pass, role)
            bool isSuccess = userController.TambahUser(
                txtUsername.Text,
                txtPassword.Text,
                cmbRole.Text
            );

            if (isSuccess)
            {
                MessageBox.Show("User Baru Berhasil Didaftarkan!", "Sukses");
                this.Close(); // Kembali ke Login
            }
        }

        private void btnBatal_Click(object sender, EventArgs e)
        {
            this.Close(); // Menutup Child Form
        }

        // Fungsi pembantu untuk mengosongkan form
        private void ClearForm()
        {
            txtNamaLengkap.Clear();
            txtUsername.Clear();
            txtPassword.Clear();
            cmbRole.SelectedIndex = -1;
            txtNamaLengkap.Focus();
        }
        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            var user = UserController.Login(txtUsername.Text, txtPassword.Text);
            // Buka Form Login
            FormLogin formLogin = new FormLogin();
            formLogin.Show();

            // Sembunyikan Form Registrasi
            this.Hide();

            if (user != null)
            {
                FormDashboard dashboard = new FormDashboard();
                dashboard.Show();
                this.Hide();
            }
            else
            {
            }
        }

        private void btnSimpan_Click_1(object sender, EventArgs e)
        {
            // Validasi input
            if (string.IsNullOrEmpty(txtNamaLengkap.Text) || string.IsNullOrEmpty(txtUsername.Text))
            {
                MessageBox.Show("Nama dan Username tidak boleh kosong!");
                return;
            }

            // Inisialisasi controller
            UserController userControl = new UserController();

            // Ambil data dari form (Pastikan Name di Properties sudah benar)
            if (userControl.TambahUser(txtNamaLengkap.Text, txtUsername.Text, txtPassword.Text, cmbRole.Text))
            {
                MessageBox.Show("Data User Berhasil Disimpan", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close(); // Kembali ke login
            }
        }

        private void btnBatal_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
