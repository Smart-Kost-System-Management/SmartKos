using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartKos.controller;

namespace SmartKos.controller
{
    public class UserController
    {
        // Pastikan objek koneksi dibuat agar tidak kena error CS0120
        private Koneksi koneksi = new Koneksi();

        public bool TambahUser(string username, string password, string role)
        {
            try
            {
                // Pastikan menggunakan instance 'koneksi' yang sudah dibuat di tingkat class
                if (koneksi.GetConn().State == System.Data.ConnectionState.Closed)
                    koneksi.GetConn().Open();

                // QUERY DIPERBAIKI: Hanya memasukkan ke kolom yang ADA di database Anda
                string query = "INSERT INTO tbl_user (Username, Password, Role) VALUES (@user, @pass, @role)";

                MySqlCommand cmd = new MySqlCommand(query, koneksi.GetConn());

                // Parameter harus sesuai dengan variabel di atas
                cmd.Parameters.AddWithValue("@user", username);
                cmd.Parameters.AddWithValue("@pass", password);
                cmd.Parameters.AddWithValue("@role", role);

                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Gagal Simpan: " + ex.Message);
                return false;
            }
            finally
            {
                koneksi.GetConn().Close();
            }
        }
        public bool TambahUser(string nama, string username, string password, string role)
        {
            try
            {
                if (koneksi.GetConn().State == System.Data.ConnectionState.Closed)
                    koneksi.GetConn().Open();

                // SESUAIKAN: Pastikan nama kolom (NamaLengkap, Username, Password, Role) 
                // sama persis dengan yang ada di phpMyAdmin
                string query = "INSERT INTO tbl_user (NamaLengkap, Username, Password, Role) VALUES (@nama, @user, @pass, @role)";

                MySqlCommand cmd = new MySqlCommand(query, koneksi.GetConn());

                cmd.Parameters.AddWithValue("@nama", nama);
                cmd.Parameters.AddWithValue("@user", username);
                cmd.Parameters.AddWithValue("@pass", password);
                cmd.Parameters.AddWithValue("@role", role);

                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                // Menampilkan pesan error asli dari MySQL untuk memudahkan pelacakan
                System.Windows.Forms.MessageBox.Show("Gagal Simpan: " + ex.Message);
                return false;
            }
            finally
            {
                koneksi.GetConn().Close();
            }
        }

        public static object Login(string user, string pass)
        {
            if (user == "admin" && pass == "admin") return new object();
            return null;
        }
    }
}

