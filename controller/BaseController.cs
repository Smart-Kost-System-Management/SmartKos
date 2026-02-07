using System;
using MySql.Data.MySqlClient;

namespace SmartKos.controller
{
    /// <summary>
    /// Abstract Base Controller - Parent class untuk semua controller
    /// Mendemonstrasikan konsep INHERITANCE dan POLYMORPHISM dalam OOP
    /// </summary>
    public abstract class BaseController
    {
        // Protected agar bisa diakses oleh child class (Inheritance)
        protected Koneksi koneksi;

        /// <summary>
        /// Constructor - inisialisasi koneksi database
        /// </summary>
        public BaseController()
        {
            koneksi = new Koneksi();
        }

        /// <summary>
        /// Abstract method - WAJIB di-override oleh child class (Polymorphism)
        /// Mengembalikan nama tabel yang digunakan controller
        /// </summary>
        public abstract string GetTableName();

        /// <summary>
        /// Virtual method - BOLEH di-override oleh child class (Polymorphism)
        /// Untuk logging aktivitas CRUD
        /// </summary>
        /// <param name="action">Jenis aksi (Create/Read/Update/Delete)</param>
        /// <param name="detail">Detail tambahan</param>
        public virtual void LogActivity(string action, string detail = "")
        {
            string tableName = GetTableName();
            string logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {tableName} - {action}";
            
            if (!string.IsNullOrEmpty(detail))
            {
                logMessage += $" - {detail}";
            }

            // Log ke console (bisa dikembangkan ke file atau database)
            System.Diagnostics.Debug.WriteLine(logMessage);
        }

        /// <summary>
        /// Helper method untuk membuka koneksi dengan aman
        /// </summary>
        protected void OpenConnection()
        {
            if (koneksi.GetConn().State == System.Data.ConnectionState.Closed)
            {
                koneksi.GetConn().Open();
            }
        }

        /// <summary>
        /// Helper method untuk menutup koneksi dengan aman
        /// </summary>
        protected void CloseConnection()
        {
            if (koneksi.GetConn().State == System.Data.ConnectionState.Open)
            {
                koneksi.GetConn().Close();
            }
        }

        /// <summary>
        /// Virtual method untuk validasi sebelum insert/update
        /// Bisa di-override untuk validasi spesifik di child class
        /// </summary>
        public virtual bool Validate(object entity)
        {
            return entity != null;
        }
    }
}
