using MySql.Data.MySqlClient;
using SmartKos.model;
using SmartKos.Interface;
using System;
using System.Collections.Generic;
using System.Data;

namespace SmartKos.controller
{
    /// <summary>
    /// Controller Kamar - Mengelola data kamar kos
    /// Mendemonstrasikan:
    /// - INHERITANCE: extends BaseController
    /// - ABSTRACTION: implements IRepository<M_kamar>
    /// - POLYMORPHISM: override GetTableName() dan LogActivity()
    /// </summary>
    internal class Kamar : BaseController, IRepository<M_kamar>
    {
        // Constructor memanggil base constructor (Inheritance)
        public Kamar() : base()
        {
        }

        #region Abstract/Virtual Method Override (POLYMORPHISM)

        /// <summary>
        /// Override abstract method dari BaseController
        /// WAJIB diimplementasi (Polymorphism)
        /// </summary>
        public override string GetTableName()
        {
            return "tbl_kamar";
        }

        /// <summary>
        /// Override virtual method dari BaseController
        /// Customize logging untuk Kamar (Polymorphism)
        /// </summary>
        public override void LogActivity(string action, string detail = "")
        {
            // Panggil base method terlebih dahulu
            base.LogActivity(action, detail);

            // Tambahan logging khusus untuk Kamar
            if (action == "DELETE")
            {
                System.Diagnostics.Debug.WriteLine($"[WARNING] Kamar {detail} telah dihapus!");
            }
        }

        #endregion

        #region IRepository Implementation (ABSTRACTION)

        /// <summary>
        /// Implementasi IRepository.GetAll() - Mengambil semua data kamar
        /// </summary>
        /// <summary>
        /// Implementasi IRepository.GetAll() - Mengambil semua data kamar
        /// </summary>
        public List<M_kamar> GetAll()
        {
            return TampilSemuaKamar();
        }

        /// <summary>
        /// Implementasi IRepository.Add() - Menambah kamar baru
        /// </summary>
        public bool Add(M_kamar entity)
        {
            return TambahKamar(entity.NomorKamar, entity.TipeKamar, entity.Harga.ToString(), entity.Status);
        }

        /// <summary>
        /// Implementasi IRepository.Update() - Update data kamar
        /// </summary>
        public bool Update(M_kamar entity)
        {
            return UpdateStatusKamar(entity.NomorKamar, entity.Status, entity.TipeKamar, entity.Harga.ToString());
        }

        /// <summary>
        /// Implementasi IRepository.Delete() - Hapus kamar
        /// </summary>
        public bool Delete(string id)
        {
            return HapusKamar(id);
        }

        /// <summary>
        /// Implementasi IRepository.Search() - Cari kamar
        /// </summary>
        public DataTable Search(string keyword)
        {
            return CariKamar(keyword);
        }

        #endregion

        #region Original Methods (dengan perbaikan menggunakan base class)

        public List<M_kamar> TampilSemuaKamar()
        {
            List<M_kamar> list = new List<M_kamar>();
            try
            {
                OpenConnection(); // Menggunakan method dari BaseController
                string query = "SELECT k.KamarID, k.NomorKamar, k.TipeID, t.NamaTipe as TipeKamar, k.Harga, k.Status, p.Nama as NamaPenghuni " +
                               "FROM tbl_kamar k " +
                               "JOIN Tbl_TipeKamar t ON k.TipeID = t.TipeID " +
                               "LEFT JOIN Tbl_Penghuni p ON k.KamarID = p.KamarID";
                MySqlCommand cmd = new MySqlCommand(query, koneksi.GetConn());
                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        M_kamar k = new M_kamar();
                        k.KamarID = Convert.ToInt32(dr["KamarID"]);
                        k.NomorKamar = dr["NomorKamar"].ToString();
                        k.TipeId = Convert.ToInt32(dr["TipeID"]);
                        k.TipeKamar = dr["TipeKamar"].ToString();
                        k.Harga = Convert.ToDecimal(dr["Harga"]);
                        k.Status = dr["Status"].ToString();
                        
                        // Map NamaPenghuni only if Status is Terisi
                        if (k.Status == "Terisi" && dr["NamaPenghuni"] != DBNull.Value)
                        {
                            k.NamaPenghuni = dr["NamaPenghuni"].ToString();
                        }
                        else
                        {
                            k.NamaPenghuni = null; // or "-" if preferred, but user said "null isinya"
                        }
                        
                        list.Add(k);
                    }
                }

                LogActivity("READ", "Tampil semua kamar"); // Logging (Polymorphism)
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Gagal Load Data: " + ex.Message);
            }
            finally
            {
                CloseConnection(); // Menggunakan method dari BaseController
            }
            return list;
        }

        public List<M_kamar> GetStatusKamar()
        {
            List<M_kamar> list = new List<M_kamar>();
            try
            {
                OpenConnection();
                string query = "SELECT KamarID, NomorKamar, Status FROM tbl_kamar";
                MySqlCommand cmd = new MySqlCommand(query, koneksi.GetConn());
                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add(new M_kamar
                        {
                            KamarID = Convert.ToInt32(dr["KamarID"]),
                            NomorKamar = dr["NomorKamar"].ToString(),
                            Status = dr["Status"].ToString()
                        });
                    }
                }
            }
            catch (Exception ex) { System.Windows.Forms.MessageBox.Show("Gagal Load Visual: " + ex.Message); }
            finally { CloseConnection(); }
            return list;
        }

        public bool TambahKamar(string nomor, string tipe, string harga, string status)
        {
            try
            {
                OpenConnection();
                string query = "INSERT INTO tbl_kamar (NomorKamar, TipeID, Harga, Status) " +
                               "VALUES (@no, (SELECT TipeID FROM Tbl_TipeKamar WHERE NamaTipe = @tipe), @harga, @status)";
                MySqlCommand cmd = new MySqlCommand(query, koneksi.GetConn());
                cmd.Parameters.AddWithValue("@no", nomor);
                cmd.Parameters.AddWithValue("@tipe", tipe);
                cmd.Parameters.AddWithValue("@harga", harga);
                cmd.Parameters.AddWithValue("@status", status);

                bool result = cmd.ExecuteNonQuery() > 0;
                if (result)
                {
                    LogActivity("CREATE", $"Kamar {nomor} ditambahkan");
                }
                return result;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Gagal Tambah Kamar: " + ex.Message);
                return false;
            }
            finally { CloseConnection(); }
        }

        public bool UpdateStatusKamar(string nomorKamar, string status, string tipe, string harga)
        {
            try
            {
                OpenConnection();
                string query = "UPDATE tbl_kamar SET Status = @status, " +
                               "TipeID = (SELECT TipeID FROM Tbl_TipeKamar WHERE NamaTipe = @tipe), " +
                               "Harga = @harga WHERE NomorKamar = @nomor";
                MySqlCommand cmd = new MySqlCommand(query, koneksi.GetConn());

                cmd.Parameters.AddWithValue("@status", status);
                cmd.Parameters.AddWithValue("@tipe", tipe);
                cmd.Parameters.AddWithValue("@harga", harga);
                cmd.Parameters.AddWithValue("@nomor", nomorKamar);

                bool result = cmd.ExecuteNonQuery() > 0;
                if (result)
                {
                    LogActivity("UPDATE", $"Kamar {nomorKamar} diupdate");
                }
                return result;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Gagal Update: " + ex.Message);
                return false;
            }
            finally
            {
                CloseConnection();
            }
        }

        public bool HapusKamar(string nomorKamar)
        {
            try
            {
                OpenConnection();
                string query = "DELETE FROM tbl_kamar WHERE NomorKamar = @nomor";
                MySqlCommand cmd = new MySqlCommand(query, koneksi.GetConn());
                cmd.Parameters.AddWithValue("@nomor", nomorKamar);

                bool result = cmd.ExecuteNonQuery() > 0;
                if (result)
                {
                    LogActivity("DELETE", nomorKamar); // Akan trigger custom warning
                }
                return result;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Gagal Hapus: " + ex.Message);
                return false;
            }
            finally { CloseConnection(); }
        }

        public DataTable CariKamar(string keyword)
        {
            DataTable dt = new DataTable();
            try
            {
                OpenConnection();
                string query = "SELECT k.KamarID, k.NomorKamar, k.TipeID, t.NamaTipe as TipeKamar, k.Harga, k.Status, p.Nama as NamaPenghuni " +
                               "FROM tbl_kamar k " +
                               "JOIN Tbl_TipeKamar t ON k.TipeID = t.TipeID " +
                               "LEFT JOIN Tbl_Penghuni p ON k.KamarID = p.KamarID " +
                               "WHERE k.NomorKamar LIKE @key OR t.NamaTipe LIKE @key OR k.Status LIKE @key OR p.Nama LIKE @key";
                MySqlCommand cmd = new MySqlCommand(query, koneksi.GetConn());
                cmd.Parameters.AddWithValue("@key", "%" + keyword + "%");
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                
                // Use DataReader or manually fill specific columns to object? 
                // Ah, CariKamar returns DataTable, so we need to be careful.
                // If it returns DataTable, the grid will just show columns from the query.
                // We need to apply the "null if not Terisi" logic here too if possible, OR just let the grid show whatever the query returns.
                // The query returns `NamaPenghuni` which might be present even if status is wrong (data inconsistency hope not). 
                // But specifically, if status is 'Kosong' usually there is no matching record in Tbl_Penghuni (assuming correctly managed), so it will be null anyway.
                // However, user said "if status maintenance or kosong then null".
                // Since this returns a DataTable directly to DataSource, we can't easily apply C# logic per row unless we iterate.
                // But purely SQL: CASE WHEN k.Status = 'Terisi' THEN p.Nama ELSE NULL END as NamaPenghuni
                
                // Let's rewrite query with CASE WHEN.
                
                query = "SELECT k.KamarID, k.NomorKamar, k.TipeID, t.NamaTipe as TipeKamar, k.Harga, k.Status, " +
                        "CASE WHEN k.Status = 'Terisi' THEN p.Nama ELSE NULL END as NamaPenghuni " +
                        "FROM tbl_kamar k " +
                        "JOIN Tbl_TipeKamar t ON k.TipeID = t.TipeID " +
                        "LEFT JOIN Tbl_Penghuni p ON k.KamarID = p.KamarID " +
                        "WHERE k.NomorKamar LIKE @key OR t.NamaTipe LIKE @key OR k.Status LIKE @key OR p.Nama LIKE @key";
                
                cmd.CommandText = query; // Update command text
                da.SelectCommand = cmd; // Re-assign
                da.Fill(dt);

                LogActivity("SEARCH", $"Keyword: {keyword}");
            }
            catch (Exception ex) { System.Windows.Forms.MessageBox.Show("Gagal Cari: " + ex.Message); }
            finally { CloseConnection(); }
            return dt;
        }

        public bool CekKamarEksis(string nomorKamar)
        {
            try
            {
                OpenConnection();
                string query = "SELECT COUNT(*) FROM tbl_kamar WHERE NomorKamar = @nomor";
                MySqlCommand cmd = new MySqlCommand(query, koneksi.GetConn());
                cmd.Parameters.AddWithValue("@nomor", nomorKamar);
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                return count > 0;
            }
            catch { return false; }
            finally { CloseConnection(); }
        }

        #endregion
    }
}