using MySql.Data.MySqlClient;
using SmartKos.model;
using System;
using System.Collections.Generic;
using System.Data;

namespace SmartKos.controller
{
    internal class Kamar
    {
        private Koneksi koneksi = new Koneksi();

        // Mengambil data User untuk tabel bawah
        public DataTable TampilSemuaKamar()
        {
            DataTable dt = new DataTable();
            try
            {
                koneksi.GetConn().Open();
                // Query mengambil Nomor, Tipe, Harga, dan Status dari tbl_kamar
                string query = "SELECT NomorKamar, TipeKamar, Harga, Status FROM tbl_kamar";
                MySqlCommand cmd = new MySqlCommand(query, koneksi.GetConn());
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Gagal Load DataGrid: " + ex.Message);
            }
            finally
            {
                koneksi.GetConn().Close();
            }
            return dt;
        }

        // FUNGSI UTAMA: Mengambil data untuk kotak kamar
        public List<M_kamar> GetStatusKamar()
        {
            List<M_kamar> list = new List<M_kamar>();
            try
            {
                koneksi.GetConn().Open();
                string query = "SELECT NomorKamar, Status FROM tbl_kamar";
                MySqlCommand cmd = new MySqlCommand(query, koneksi.GetConn());
                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add(new M_kamar
                        {
                            NomorKamar = dr["NomorKamar"].ToString(),
                            Status = dr["Status"].ToString()
                        });
                    }
                }
            }
            catch (Exception ex) { System.Windows.Forms.MessageBox.Show("Gagal Load: " + ex.Message); }
            finally { koneksi.GetConn().Close(); }
            return list;
        }

        // PERBAIKAN: Fungsi Update dengan 3 Parameter agar tidak error CS1501
        public bool UpdateStatusKamar(string kamarID, string status, string userID)
        {
            try
            {
                koneksi.GetConn().Open();
                // Samakan parameter SQL @nomor dengan variabel kamarID
                string query = "UPDATE tbl_kamar SET Status = @status, UserID = @uid WHERE NomorKamar = @nomor";
                MySqlCommand cmd = new MySqlCommand(query, koneksi.GetConn());

                cmd.Parameters.AddWithValue("@status", status);
                cmd.Parameters.AddWithValue("@uid", string.IsNullOrEmpty(userID) ? (object)DBNull.Value : userID);
                cmd.Parameters.AddWithValue("@nomor", kamarID); // SEBELUMNYA ERROR DI SINI

                return cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                koneksi.GetConn().Close();
            }
        }


        public bool TambahKamar(string nomor, string tipe, string harga)
        {
            try
            {
                koneksi.GetConn().Open();
                string query = "INSERT INTO tbl_kamar (NomorKamar, TipeKamar, Harga, Status) VALUES (@no, @tipe, @harga, 'Kosong')";
                MySqlCommand cmd = new MySqlCommand(query, koneksi.GetConn());
                cmd.Parameters.AddWithValue("@no", nomor);
                cmd.Parameters.AddWithValue("@tipe", tipe);
                cmd.Parameters.AddWithValue("@harga", harga);

                return cmd.ExecuteNonQuery() > 0;
            }
            catch { return false; }
            finally { koneksi.GetConn().Close(); }
        }

        public bool CekKamarEksis(string nomorKamar)
        {
            try
            {
                koneksi.GetConn().Open();
                string query = "SELECT COUNT(*) FROM tbl_kamar WHERE NomorKamar = @nomor";
                MySqlCommand cmd = new MySqlCommand(query, koneksi.GetConn());
                cmd.Parameters.AddWithValue("@nomor", nomorKamar);
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                return count > 0;
            }
            catch { return false; }
            finally { koneksi.GetConn().Close(); }
        }

        public bool TambahKamarBaru(string nomorKamar, string status)
        {
            try
            {
                koneksi.GetConn().Open();
                // Menambah kamar baru dengan tipe default 'Standard' dan harga '0'
                string query = "INSERT INTO tbl_kamar (NomorKamar, TipeKamar, Harga, Status) VALUES (@nomor, 'Standard', 0, @status)";
                MySqlCommand cmd = new MySqlCommand(query, koneksi.GetConn());
                cmd.Parameters.AddWithValue("@nomor", nomorKamar);
                cmd.Parameters.AddWithValue("@status", status);
                return cmd.ExecuteNonQuery() > 0;
            }
            catch { return false; }
            finally { koneksi.GetConn().Close(); }
        }
    }
}