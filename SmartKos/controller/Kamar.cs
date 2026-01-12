using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using SmartKos.model;

namespace SmartKos.controller
{
    internal class Kamar
    {
        private Koneksi koneksi;

        public Kamar()
        {
            koneksi = new Koneksi();
        }

        public DataTable TampilSemuaKamar()
        {
            DataTable dt = new DataTable();
            try
            {
                koneksi.GetConn().Open();
                // Gunakan MySqlCommand dan MySqlDataAdapter
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM Tbl_Kamar", koneksi.GetConn());
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                da.Fill(dt);
            }
            finally
            {
                koneksi.GetConn().Close();
            }
            return dt;
        }

        public List<M_kamar> GetListKamar()
        {
            List<M_kamar> list = new List<M_kamar>();
            DataTable dt = TampilSemuaKamar();
            foreach (DataRow row in dt.Rows)
            {
                M_kamar k = new M_kamar();
                k.NomorKamar = row["NomorKamar"].ToString();
                k.Status = row["Status"].ToString();
                list.Add(k);
            }
            return list;
        }

        // ... (Kode sebelumnya tetap ada, tambahkan di bawahnya)

        public void TambahKamar(M_kamar kamar)
        {
            string query = "INSERT INTO Tbl_Kamar (NomorKamar, TipeKamar, Harga, Status) VALUES (@nomor, @tipe, @harga, @status)";
            try
            {
                koneksi.GetConn().Open();
                MySqlCommand cmd = new MySqlCommand(query, koneksi.GetConn());
                cmd.Parameters.AddWithValue("@nomor", kamar.NomorKamar);
                cmd.Parameters.AddWithValue("@tipe", kamar.TipeKamar);
                cmd.Parameters.AddWithValue("@harga", kamar.Harga);
                cmd.Parameters.AddWithValue("@status", kamar.Status);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex; // Lempar error ke View agar muncul MessageBox
            }
            finally
            {
                koneksi.GetConn().Close();
            }
        }

        public void UpdateKamar(M_kamar kamar, string id)
        {
            string query = "UPDATE Tbl_Kamar SET NomorKamar=@nomor, TipeKamar=@tipe, Harga=@harga, Status=@status WHERE KamarID=@id";
            try
            {
                koneksi.GetConn().Open();
                MySqlCommand cmd = new MySqlCommand(query, koneksi.GetConn());
                cmd.Parameters.AddWithValue("@nomor", kamar.NomorKamar);
                cmd.Parameters.AddWithValue("@tipe", kamar.TipeKamar);
                cmd.Parameters.AddWithValue("@harga", kamar.Harga);
                cmd.Parameters.AddWithValue("@status", kamar.Status);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                koneksi.GetConn().Close();
            }
        }

        public void HapusKamar(string id)
        {
            string query = "DELETE FROM Tbl_Kamar WHERE KamarID=@id";
            try
            {
                koneksi.GetConn().Open();
                MySqlCommand cmd = new MySqlCommand(query, koneksi.GetConn());
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                koneksi.GetConn().Close();
            }
        }

        public DataTable CariKamar(string keyword)
        {
            DataTable dt = new DataTable();
            try
            {
                koneksi.GetConn().Open();

                // REVISI QUERY: Tambahkan "OR KamarID..." dan "OR Harga..."
                // MySQL otomatis mengkonversi angka ke string saat pakai LIKE
                string query = "SELECT * FROM Tbl_Kamar WHERE " +
                               "KamarID LIKE @key OR " +       // Cari berdasarkan ID
                               "NomorKamar LIKE @key OR " +    // Cari berdasarkan No Kamar
                               "TipeKamar LIKE @key OR " +     // Cari berdasarkan Tipe
                               "Harga LIKE @key OR " +         // Cari berdasarkan Harga
                               "Status LIKE @key";             // Cari berdasarkan Status

                MySqlCommand cmd = new MySqlCommand(query, koneksi.GetConn());
                cmd.Parameters.AddWithValue("@key", "%" + keyword + "%");

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                da.Fill(dt);
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
            finally
            {
                koneksi.GetConn().Close();
            }
            return dt;
        }
    }
}
