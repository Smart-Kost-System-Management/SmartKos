using MySql.Data.MySqlClient;
using SmartKos.model;
using System;
using System.Collections.Generic;
using System.Data;

namespace SmartKos.controller
{
    /// <summary>
    /// Controller for Booking logic
    /// </summary>
    public class BookingController : BaseController
    {
        public override string GetTableName() { return "Tbl_Booking"; }

        public bool AddBooking(M_Booking booking)
        {
            try
            {
                OpenConnection();
                // Check if user already has pending booking
                string checkQuery = "SELECT COUNT(*) FROM Tbl_Booking WHERE UserID = @uid AND Status = 'Pending'";
                MySqlCommand checkCmd = new MySqlCommand(checkQuery, koneksi.GetConn());
                checkCmd.Parameters.AddWithValue("@uid", booking.UserID);
                int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                
                if (count > 0)
                {
                    System.Windows.Forms.MessageBox.Show("Anda masih memiliki booking Pending!");
                    return false;
                }

                string query = "INSERT INTO Tbl_Booking (UserID, KamarID, Status, Nama, NoKTP, NoHP) VALUES (@uid, @kid, 'Pending', @nm, @ktp, @hp)";
                MySqlCommand cmd = new MySqlCommand(query, koneksi.GetConn());
                cmd.Parameters.AddWithValue("@uid", booking.UserID);
                cmd.Parameters.AddWithValue("@kid", booking.KamarID);
                cmd.Parameters.AddWithValue("@nm", booking.Nama ?? "");
                cmd.Parameters.AddWithValue("@ktp", booking.NoKTP ?? "");
                cmd.Parameters.AddWithValue("@hp", booking.NoHP ?? "");
                
                bool result = cmd.ExecuteNonQuery() > 0;
                if(result) LogActivity("CREATE", $"Booking Room ID: {booking.KamarID}");
                
                return result;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Gagal Booking: " + ex.Message);
                return false;
            }
            finally { CloseConnection(); }
        }

        public List<M_Booking> GetAllBookings()
        {
            List<M_Booking> list = new List<M_Booking>();
            try
            {
                OpenConnection();
                // JOIN to get readable names
                string query = @"SELECT * FROM Tbl_Booking b
                                 JOIN tbl_user u ON b.UserID = u.UserID
                                 JOIN Tbl_Kamar k ON b.KamarID = k.KamarID
                                 ORDER BY b.TanggalBooking DESC";
                                 
                MySqlCommand cmd = new MySqlCommand(query, koneksi.GetConn());
                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add(new M_Booking
                        {
                            BookingID = Convert.ToInt32(dr["BookingID"]),
                            UserID = Convert.ToInt32(dr["UserID"]),
                            KamarID = Convert.ToInt32(dr["KamarID"]),
                            TanggalBooking = Convert.ToDateTime(dr["TanggalBooking"]),
                            Status = dr["Status"].ToString(),
                            Nama = dr["Nama"].ToString(),
                            NoKTP = dr["NoKTP"].ToString(),
                            NoHP = dr["NoHP"].ToString(),
                            UserName = dr["Username"].ToString(),
                            NomorKamar = dr["NomorKamar"].ToString()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Gagal Load Booking: " + ex.Message);
            }
            finally { CloseConnection(); }
            return list;
        }

        public bool UpdateStatus(int bookingId, string status, int kamarId)
        {
            MySqlTransaction transaction = null;
            try
            {
                OpenConnection();
                transaction = koneksi.GetConn().BeginTransaction();

                // 1. Update Booking Status
                string query = "UPDATE Tbl_Booking SET Status = @stat WHERE BookingID = @id";
                MySqlCommand cmd = new MySqlCommand(query, koneksi.GetConn(), transaction);
                cmd.Parameters.AddWithValue("@stat", status);
                cmd.Parameters.AddWithValue("@id", bookingId);
                
                bool result = cmd.ExecuteNonQuery() > 0;
                
                if (result && status == "Approved")
                {
                    // 2. Get Candidate Data (Use scalar/params to reuse connection in transaction context?)
                    // Note: Cannot use ExecuteReader easily on same connection if not managed well, 
                    // but inside transaction it works if we use the same command object or close readers.
                    string qGet = "SELECT Nama, NoKTP, NoHP FROM Tbl_Booking WHERE BookingID = @bid";
                    MySqlCommand cGet = new MySqlCommand(qGet, koneksi.GetConn(), transaction);
                    cGet.Parameters.AddWithValue("@bid", bookingId);
                    
                    string nama = "", ktp = "", hp = "";
                    using(MySqlDataReader dr = cGet.ExecuteReader())
                    {
                        if(dr.Read())
                        {
                            nama = dr["Nama"].ToString();
                            ktp = dr["NoKTP"].ToString();
                            hp = dr["NoHP"].ToString();
                        }
                    }

                    // 3. Insert Into Penghuni
                    string qIns = "INSERT INTO Tbl_Penghuni (Nama, NoKTP, NoHP, KamarID) VALUES (@nm, @ktp, @hp, @kid)";
                    MySqlCommand cIns = new MySqlCommand(qIns, koneksi.GetConn(), transaction);
                    cIns.Parameters.AddWithValue("@nm", nama);
                    cIns.Parameters.AddWithValue("@ktp", ktp);
                    cIns.Parameters.AddWithValue("@hp", hp);
                    cIns.Parameters.AddWithValue("@kid", kamarId);
                    cIns.ExecuteNonQuery();

                    // 4. Update Kamar Status
                    string qKam = "UPDATE Tbl_Kamar SET Status = 'Terisi' WHERE KamarID = @kid";
                    MySqlCommand cKam = new MySqlCommand(qKam, koneksi.GetConn(), transaction);
                    cKam.Parameters.AddWithValue("@kid", kamarId);
                    cKam.ExecuteNonQuery();
                    
                    LogActivity("APPROVE", $"Booking {bookingId} Approved -> Penghuni Created");
                }
                else if (result && status == "Rejected")
                {
                    LogActivity("REJECT", $"Booking {bookingId} Rejected");
                }
                
                transaction.Commit();
                return result;
            }
            catch (Exception ex)
            {
                if(transaction != null) transaction.Rollback();
                System.Windows.Forms.MessageBox.Show("Gagal Update Status: " + ex.Message);
                return false;
            }
            finally { CloseConnection(); }
        }
    }
}
