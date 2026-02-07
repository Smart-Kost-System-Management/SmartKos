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
    public class PembayaranController : BaseController, IRepository<M_Pembayaran>
    {
        // Polymorphism
        public override string GetTableName()
        {
            return "Tbl_Pembayaran";
        }

        public override void LogActivity(string action, string details)
        {
            Console.WriteLine($"[PEMBAYARAN LOG] {DateTime.Now}: {action} - {details}");
        }

        public List<M_Pembayaran> GetAll()
        {
            List<M_Pembayaran> list = new List<M_Pembayaran>();
            try
            {
                OpenConnection();
                // JOIN Tbl_Penghuni & Tbl_Kamar for full details
                string query = "SELECT p.*, h.Nama as NamaPenghuni, k.NomorKamar " +
                               "FROM Tbl_Pembayaran p " +
                               "JOIN Tbl_Penghuni h ON p.PenghuniID = h.PenghuniID " +
                               "LEFT JOIN Tbl_Kamar k ON h.KamarID = k.KamarID";

                MySqlCommand cmd = new MySqlCommand(query, koneksi.GetConn());
                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        M_Pembayaran p = new M_Pembayaran();
                        p.PembayaranID = Convert.ToInt32(dr["PembayaranID"]);
                        p.PenghuniID = Convert.ToInt32(dr["PenghuniID"]);
                        p.Jumlah = Convert.ToDecimal(dr["Jumlah"]);
                        p.TanggalBayar = Convert.ToDateTime(dr["TanggalBayar"]);
                        p.Status = dr["Status"].ToString();
                        
                        // Map Extended Properties
                        p.NamaPenghuni = dr["NamaPenghuni"].ToString();
                        p.NomorKamar = dr["NomorKamar"] != DBNull.Value ? dr["NomorKamar"].ToString() : "-";

                        list.Add(p);
                    }
                }
                LogActivity("READ", "Tampil semua pembayaran (with Details)");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error GetAll Pembayaran: " + ex.Message);
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
                // Join logic could be added here for better search (e.g. searching by Penghuni Name)
                string query = "SELECT p.*, h.Nama FROM Tbl_Pembayaran p " +
                               "JOIN Tbl_Penghuni h ON p.PenghuniID = h.PenghuniID " +
                               "WHERE h.Nama LIKE @key OR p.Status LIKE @key";
                MySqlCommand cmd = new MySqlCommand(query, koneksi.GetConn());
                cmd.Parameters.AddWithValue("@key", "%" + keyword + "%");
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Search Pembayaran: " + ex.Message);
            }
            finally
            {
                CloseConnection();
            }
            return dt;
        }

        public bool Add(M_Pembayaran entity)
        {
            bool status = false;
            try
            {
                OpenConnection();
                string query = "INSERT INTO Tbl_Pembayaran (PenghuniID, Jumlah, TanggalBayar, Status) " +
                               "VALUES (@pid, @jml, @tgl, @stt)";
                MySqlCommand cmd = new MySqlCommand(query, koneksi.GetConn());
                cmd.Parameters.AddWithValue("@pid", entity.PenghuniID);
                cmd.Parameters.AddWithValue("@jml", entity.Jumlah);
                cmd.Parameters.AddWithValue("@tgl", entity.TanggalBayar);
                cmd.Parameters.AddWithValue("@stt", entity.Status);
                
                int rows = cmd.ExecuteNonQuery();
                if (rows > 0)
                {
                    LogActivity("ADD", "Added Payment for PenghuniID: " + entity.PenghuniID);
                    status = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Add Pembayaran: " + ex.Message);
            }
            finally
            {
                CloseConnection();
            }
            return status;
        }

        public bool Update(M_Pembayaran entity)
        {
            bool status = false;
            try
            {
                OpenConnection();
                string query = "UPDATE Tbl_Pembayaran SET Jumlah=@jml, Status=@stt WHERE PembayaranID=@id";
                MySqlCommand cmd = new MySqlCommand(query, koneksi.GetConn());
                cmd.Parameters.AddWithValue("@jml", entity.Jumlah);
                cmd.Parameters.AddWithValue("@stt", entity.Status);
                cmd.Parameters.AddWithValue("@id", entity.PembayaranID);

                int rows = cmd.ExecuteNonQuery();
                if (rows > 0)
                {
                    LogActivity("UPDATE", "Updated Payment ID: " + entity.PembayaranID);
                    status = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Update Pembayaran: " + ex.Message);
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
                string query = "DELETE FROM Tbl_Pembayaran WHERE PembayaranID = @id";
                MySqlCommand cmd = new MySqlCommand(query, koneksi.GetConn());
                cmd.Parameters.AddWithValue("@id", id);
                
                int rows = cmd.ExecuteNonQuery();
                if (rows > 0)
                {
                    LogActivity("DELETE", "Deleted Payment ID: " + id);
                    status = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Delete Pembayaran: " + ex.Message);
            }
            finally
            {
                CloseConnection();
            }
            return status;
        }
    }
}
