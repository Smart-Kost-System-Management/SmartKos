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

        public DataTable TampilSemuaKamar()
        {
            DataTable dt = new DataTable();
            try
            {
                koneksi.GetConn().Open();
                // REVISI: Gunakan JOIN ke Tbl_TipeKamar karena kolom TipeKamar di tbl_kamar sudah dihapus (diganti TipeID)
                string query = "SELECT k.KamarID, k.NomorKamar, t.NamaTipe as TipeKamar, k.Harga, k.Status " +
                               "FROM tbl_kamar k " +
                               "JOIN Tbl_TipeKamar t ON k.TipeID = t.TipeID";
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
            catch (Exception ex) { System.Windows.Forms.MessageBox.Show("Gagal Load Visual: " + ex.Message); }
            finally { koneksi.GetConn().Close(); }
            return list;
        }

        public bool TambahKamar(string nomor, string tipe, string harga, string status)
        {
            try
            {
                koneksi.GetConn().Open();
                // Gunakan TipeID dari Tbl_TipeKamar
                string query = "INSERT INTO tbl_kamar (NomorKamar, TipeID, Harga, Status) " +
                               "VALUES (@no, (SELECT TipeID FROM Tbl_TipeKamar WHERE NamaTipe = @tipe), @harga, @status)";
                MySqlCommand cmd = new MySqlCommand(query, koneksi.GetConn());
                cmd.Parameters.AddWithValue("@no", nomor);
                cmd.Parameters.AddWithValue("@tipe", tipe);
                cmd.Parameters.AddWithValue("@harga", harga);
                cmd.Parameters.AddWithValue("@status", status);

                return cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Gagal Tambah Kamar: " + ex.Message);
                return false;
            }
            finally { koneksi.GetConn().Close(); }
        }

        public bool UpdateStatusKamar(string nomorKamar, string status, string tipe, string harga)
        {
            try
            {
                koneksi.GetConn().Open();
                string query = "UPDATE tbl_kamar SET Status = @status, " +
                               "TipeID = (SELECT TipeID FROM Tbl_TipeKamar WHERE NamaTipe = @tipe), " +
                               "Harga = @harga WHERE NomorKamar = @nomor";
                MySqlCommand cmd = new MySqlCommand(query, koneksi.GetConn());

                cmd.Parameters.AddWithValue("@status", status);
                cmd.Parameters.AddWithValue("@tipe", tipe);
                cmd.Parameters.AddWithValue("@harga", harga);
                cmd.Parameters.AddWithValue("@nomor", nomorKamar);

                return cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Gagal Update: " + ex.Message);
                return false;
            }
            finally
            {
                koneksi.GetConn().Close();
            }
        }

        public bool HapusKamar(string nomorKamar)
        {
            try
            {
                koneksi.GetConn().Open();
                string query = "DELETE FROM tbl_kamar WHERE NomorKamar = @nomor";
                MySqlCommand cmd = new MySqlCommand(query, koneksi.GetConn());
                cmd.Parameters.AddWithValue("@nomor", nomorKamar);
                return cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Gagal Hapus: " + ex.Message);
                return false;
            }
            finally { koneksi.GetConn().Close(); }
        }

        public DataTable CariKamar(string keyword)
        {
            DataTable dt = new DataTable();
            try
            {
                koneksi.GetConn().Open();
                string query = "SELECT k.KamarID, k.NomorKamar, t.NamaTipe as TipeKamar, k.Harga, k.Status " +
                               "FROM tbl_kamar k " +
                               "JOIN Tbl_TipeKamar t ON k.TipeID = t.TipeID " +
                               "WHERE k.NomorKamar LIKE @key OR t.NamaTipe LIKE @key OR k.Status LIKE @key";
                MySqlCommand cmd = new MySqlCommand(query, koneksi.GetConn());
                cmd.Parameters.AddWithValue("@key", "%" + keyword + "%");
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                da.Fill(dt);
            }
            catch (Exception ex) { System.Windows.Forms.MessageBox.Show("Gagal Cari: " + ex.Message); }
            finally { koneksi.GetConn().Close(); }
            return dt;
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
    }
}