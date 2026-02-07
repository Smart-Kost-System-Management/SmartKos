using MySql.Data.MySqlClient;
using SmartKos.model;
using SmartKos.Interface;
using System;
using System.Data;

namespace SmartKos.controller
{
    /// <summary>
    /// Controller User - Mengelola data user/penghuni
    /// Mendemonstrasikan:
    /// - INHERITANCE: extends BaseController
    /// - ABSTRACTION: implements IRepository<M_User>
    /// - POLYMORPHISM: override GetTableName() dan LogActivity()
    /// </summary>
    public class UserController : BaseController, IRepository<M_User>
    {
        // Constructor memanggil base constructor (Inheritance)
        public UserController() : base()
        {
        }

        #region Abstract/Virtual Method Override (POLYMORPHISM)

        /// <summary>
        /// Override abstract method dari BaseController
        /// WAJIB diimplementasi (Polymorphism)
        /// </summary>
        public override string GetTableName()
        {
            return "tbl_user";
        }

        /// <summary>
        /// Override virtual method dari BaseController
        /// Customize logging untuk User - menyembunyikan data sensitif (Polymorphism)
        /// </summary>
        public override void LogActivity(string action, string detail = "")
        {
            // Untuk keamanan, sembunyikan detail password
            if (action == "CREATE" || action == "UPDATE")
            {
                detail = "[Data user - password hidden]";
            }

            base.LogActivity(action, detail);
        }

        /// <summary>
        /// Override Validate method untuk validasi user
        /// </summary>
        public override bool Validate(object entity)
        {
            if (entity is M_User user)
            {
                return !string.IsNullOrEmpty(user.Username) && 
                       !string.IsNullOrEmpty(user.Password) &&
                       user.Password.Length >= 4;
            }
            return false;
        }

        #endregion

        #region IRepository Implementation (ABSTRACTION)

        /// <summary>
        /// Implementasi IRepository.GetAll() - Mengambil semua data user
        /// </summary>
        public System.Collections.Generic.List<M_User> GetAll()
        {
            System.Collections.Generic.List<M_User> list = new System.Collections.Generic.List<M_User>();
            try
            {
                OpenConnection();
                string query = "SELECT UserID, Username, Role FROM tbl_user";
                MySqlCommand cmd = new MySqlCommand(query, koneksi.GetConn());
                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add(new M_User
                        {
                            Id = Convert.ToInt32(dr["UserID"]),
                            Username = dr["Username"].ToString(),
                            Role = dr["Role"].ToString()
                        });
                    }
                }

                LogActivity("READ", "Tampil semua user");
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Gagal Load User: " + ex.Message);
            }
            finally
            {
                CloseConnection();
            }
            return list;
        }

        /// <summary>
        /// Implementasi IRepository.Add() - Menambah user baru
        /// </summary>
        public bool Add(M_User entity)
        {
            if (!Validate(entity))
            {
                System.Windows.Forms.MessageBox.Show("Data user tidak valid!");
                return false;
            }
            return TambahUser(entity.Nama, entity.Username, entity.Password, entity.Role);
        }

        /// <summary>
        /// Implementasi IRepository.Update() - Update data user
        /// </summary>
        public bool Update(M_User entity)
        {
            try
            {
                OpenConnection();
                string query = "UPDATE tbl_user SET Role = @role WHERE Username = @user";
                MySqlCommand cmd = new MySqlCommand(query, koneksi.GetConn());

                // Nama column removed
                cmd.Parameters.AddWithValue("@role", entity.Role);
                cmd.Parameters.AddWithValue("@user", entity.Username);

                bool result = cmd.ExecuteNonQuery() > 0;
                if (result)
                {
                    LogActivity("UPDATE", entity.Username);
                }
                return result;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Gagal Update User: " + ex.Message);
                return false;
            }
            finally
            {
                CloseConnection();
            }
        }

        /// <summary>
        /// Implementasi IRepository.Delete() - Hapus user
        /// </summary>
        public bool Delete(string id)
        {
            try
            {
                OpenConnection();
                string query = "DELETE FROM tbl_user WHERE Username = @user";
                MySqlCommand cmd = new MySqlCommand(query, koneksi.GetConn());
                cmd.Parameters.AddWithValue("@user", id);

                bool result = cmd.ExecuteNonQuery() > 0;
                if (result)
                {
                    LogActivity("DELETE", id);
                }
                return result;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Gagal Hapus User: " + ex.Message);
                return false;
            }
            finally
            {
                CloseConnection();
            }
        }

        /// <summary>
        /// Implementasi IRepository.Search() - Cari user
        /// </summary>
        public DataTable Search(string keyword)
        {
            DataTable dt = new DataTable();
            try
            {
                OpenConnection();
                string query = "SELECT UserID, Username, Role FROM tbl_user " +
                               "WHERE Username LIKE @key OR Role LIKE @key";
                MySqlCommand cmd = new MySqlCommand(query, koneksi.GetConn());
                cmd.Parameters.AddWithValue("@key", "%" + keyword + "%");
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                da.Fill(dt);

                LogActivity("SEARCH", $"Keyword: {keyword}");
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Gagal Cari User: " + ex.Message);
            }
            finally
            {
                CloseConnection();
            }
            return dt;
        }

        #endregion

        #region Original Methods (dengan perbaikan)

        public bool TambahUser(string username, string password, string role)
        {
            try
            {
                OpenConnection();
                string query = "INSERT INTO tbl_user (Username, Password, Role) VALUES (@user, @pass, @role)";

                MySqlCommand cmd = new MySqlCommand(query, koneksi.GetConn());
                cmd.Parameters.AddWithValue("@user", username);
                cmd.Parameters.AddWithValue("@pass", password);
                cmd.Parameters.AddWithValue("@role", role);

                bool result = cmd.ExecuteNonQuery() > 0;
                if (result)
                {
                    LogActivity("CREATE", username);
                }
                return result;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Gagal Simpan: " + ex.Message);
                return false;
            }
            finally
            {
                CloseConnection();
            }
        }

        public bool TambahUser(string nama, string username, string password, string role)
        {
            try
            {
                OpenConnection();
                // Schema mismatch: Nama not in DB. Ignore Nama for now or add column later.
                // Fallback to TambahUser(username, password, role) logic
                string query = "INSERT INTO tbl_user (Username, Password, Role) VALUES (@user, @pass, @role)";

                MySqlCommand cmd = new MySqlCommand(query, koneksi.GetConn());
                cmd.Parameters.AddWithValue("@user", username);
                cmd.Parameters.AddWithValue("@pass", password);
                cmd.Parameters.AddWithValue("@role", role);

                bool result = cmd.ExecuteNonQuery() > 0;
                if (result)
                {
                    LogActivity("CREATE", username);
                }
                return result;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Gagal Simpan: " + ex.Message);
                return false;
            }
            finally
            {
                CloseConnection();
            }
        }

        /// <summary>
        /// Login user - cek kredensial ke database
        /// </summary>
        public M_User Login(string username, string password)
        {
            try
            {
                OpenConnection();
                string query = "SELECT * FROM tbl_user WHERE Username = @user AND Password = @pass";
                MySqlCommand cmd = new MySqlCommand(query, koneksi.GetConn());
                cmd.Parameters.AddWithValue("@user", username);
                cmd.Parameters.AddWithValue("@pass", password);

                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        LogActivity("LOGIN", username);
                        return new M_User
                        {
                            Id = Convert.ToInt32(dr["UserID"]), // Fixed: Id -> UserID
                            Nama = "", // Fixed: Column Nama doesn't exist in DB
                            Username = dr["Username"].ToString(),
                            Role = dr["Role"].ToString()
                        };
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error Login: " + ex.Message);
                return null;
            }
            finally
            {
                CloseConnection();
            }
        }

        /// <summary>
        /// Cek apakah username sudah ada
        /// </summary>
        public bool CekUsernameTersedia(string username)
        {
            try
            {
                OpenConnection();
                string query = "SELECT COUNT(*) FROM tbl_user WHERE Username = @user";
                MySqlCommand cmd = new MySqlCommand(query, koneksi.GetConn());
                cmd.Parameters.AddWithValue("@user", username);
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                return count == 0; // Return true jika username belum ada
            }
            catch
            {
                return false;
            }
            finally
            {
                CloseConnection();
            }
        }

        #endregion
    }
}
