using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Data;
using System.Windows.Forms;
using SmartKos.model;
using SmartKos.Interface;

namespace SmartKos.controller
{
    // Inheritance: Inherits from BaseController
    // Abstraction: Implements IRepository interface
    public class PenghuniController : BaseController, IRepository<M_Penghuni>
    {
        // Polymorphism: Override abstract method
        public override string GetTableName()
        {
            return "Tbl_Penghuni";
        }

        // Polymorphism: Override virtual method
        public override void LogActivity(string action, string details)
        {
            Console.WriteLine($"[PENGHUNI LOG] {DateTime.Now}: {action} - {details}");
        }

        public List<M_Penghuni> GetAll()
        {
            List<M_Penghuni> list = new List<M_Penghuni>();
            try
            {
                OpenConnection();
                // JOIN Tbl_Kamar to get NomorKamar
                string query = "SELECT p.*, k.NomorKamar " +
                               "FROM Tbl_Penghuni p " +
                               "LEFT JOIN Tbl_Kamar k ON p.KamarID = k.KamarID";
                MySqlCommand cmd = new MySqlCommand(query, koneksi.GetConn());
                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        M_Penghuni p = new M_Penghuni();
                        p.PenghuniID = Convert.ToInt32(dr["PenghuniID"]);
                        p.Nama = dr["Nama"].ToString();
                        p.NoKTP = dr["NoKTP"].ToString();
                        p.NoHP = dr["NoHP"].ToString();
                        if (dr["KamarID"] != DBNull.Value)
                            p.KamarID = Convert.ToInt32(dr["KamarID"]);
                        
                        // Map NomorKamar from JOIN result
                        if (dr["NomorKamar"] != DBNull.Value)
                            p.NomorKamar = dr["NomorKamar"].ToString();
                        else
                            p.NomorKamar = "-";

                        p.TanggalMasuk = Convert.ToDateTime(dr["TanggalMasuk"]);
                        list.Add(p);
                    }
                }
                LogActivity("READ", "Tampil semua penghuni (with Room Info)");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error GetAll Penghuni: " + ex.Message);
            }
            finally
            {
                CloseConnection();
            }
            return list;
        }

        public DataTable Search(string keyword)
        {
            DataTable dt = new DataTable();
            try
            {
                OpenConnection();
                string query = "SELECT * FROM Tbl_Penghuni WHERE Nama LIKE @key OR NoKTP LIKE @key";
                MySqlCommand cmd = new MySqlCommand(query, koneksi.GetConn());
                cmd.Parameters.AddWithValue("@key", "%" + keyword + "%");
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Search Penghuni: " + ex.Message);
            }
            finally
            {
                CloseConnection();
            }
            return dt;
        }

        public bool Add(M_Penghuni entity)
        {
            bool status = false;
            try
            {
                OpenConnection();
                string query = "INSERT INTO Tbl_Penghuni (Nama, NoKTP, NoHP, KamarID, TanggalMasuk) " +
                               "VALUES (@nama, @ktp, @hp, @kamar, @tgl)";
                MySqlCommand cmd = new MySqlCommand(query, koneksi.GetConn());
                cmd.Parameters.AddWithValue("@nama", entity.Nama);
                cmd.Parameters.AddWithValue("@ktp", entity.NoKTP);
                cmd.Parameters.AddWithValue("@hp", entity.NoHP);
                cmd.Parameters.AddWithValue("@kamar", entity.KamarID);
                cmd.Parameters.AddWithValue("@tgl", entity.TanggalMasuk);
                
                int rows = cmd.ExecuteNonQuery();
                if (rows > 0)
                {
                    LogActivity("ADD", "Added Penghuni: " + entity.Nama);
                    status = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Add Penghuni: " + ex.Message);
            }
            finally
            {
                CloseConnection();
            }
            return status;
        }

        public bool Update(M_Penghuni entity)
        {
            bool status = false;
            try
            {
                OpenConnection();
                string query = "UPDATE Tbl_Penghuni SET Nama=@nama, NoKTP=@ktp, NoHP=@hp, KamarID=@kamar WHERE PenghuniID=@id";
                MySqlCommand cmd = new MySqlCommand(query, koneksi.GetConn());
                cmd.Parameters.AddWithValue("@nama", entity.Nama);
                cmd.Parameters.AddWithValue("@ktp", entity.NoKTP);
                cmd.Parameters.AddWithValue("@hp", entity.NoHP);
                cmd.Parameters.AddWithValue("@kamar", entity.KamarID);
                cmd.Parameters.AddWithValue("@id", entity.PenghuniID);

                int rows = cmd.ExecuteNonQuery();
                if (rows > 0)
                {
                    LogActivity("UPDATE", "Updated Penghuni ID: " + entity.PenghuniID);
                    status = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Update Penghuni: " + ex.Message);
            }
            finally
            {
                CloseConnection();
            }
            return status;
        }

        public bool Delete(string id)
        {
            bool status = false;
            try
            {
                OpenConnection();
                string query = "DELETE FROM Tbl_Penghuni WHERE PenghuniID = @id";
                MySqlCommand cmd = new MySqlCommand(query, koneksi.GetConn());
                cmd.Parameters.AddWithValue("@id", id);
                
                int rows = cmd.ExecuteNonQuery();
                if (rows > 0)
                {
                    LogActivity("DELETE", "Deleted Penghuni ID: " + id);
                    status = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Delete Penghuni: " + ex.Message);
            }
            finally
            {
                CloseConnection();
            }
            return status;
        }
    }
}
