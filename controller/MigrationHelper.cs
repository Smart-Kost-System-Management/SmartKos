using System;
using MySql.Data.MySqlClient;
using SmartKos.controller;
using System.Windows.Forms;

namespace SmartKos.controller
{
    internal class MigrationHelper
    {
        public static void CheckAndApplyMigration()
        {
            Koneksi k = new Koneksi();
            MySqlConnection conn = k.GetConn();

            try
            {
                conn.Open();

                // 1. Cek Tabel Tipe Kamar
                if (!CheckIfTableExists("Tbl_TipeKamar", conn))
                {
                    // Create Table
                    string queryCreate = "CREATE TABLE Tbl_TipeKamar (TipeID INT AUTO_INCREMENT PRIMARY KEY, NamaTipe VARCHAR(50) NOT NULL UNIQUE)";
                    ExecuteQuery(queryCreate, conn);

                    // Insert Default
                    string queryInsert = "INSERT INTO Tbl_TipeKamar (NamaTipe) VALUES ('AC'), ('Non-AC'), ('VVIP')";
                    ExecuteQuery(queryInsert, conn);
                }

                // 2. Cek Kolom TipeID di Tbl_Kamar
                if (!CheckIfColumnExists("Tbl_Kamar", "TipeID", conn))
                {
                    // Add Column
                    ExecuteQuery("ALTER TABLE Tbl_Kamar ADD COLUMN TipeID INT", conn);

                    // 3. Jika ada kolom lama (TipeKamar), migrasikan datanya
                    if (CheckIfColumnExists("Tbl_Kamar", "TipeKamar", conn))
                    {
                        // Migrate Data
                        ExecuteQuery("UPDATE Tbl_Kamar k JOIN Tbl_TipeKamar t ON k.TipeKamar = t.NamaTipe SET k.TipeID = t.TipeID", conn);
                        
                        // Add FK
                        ExecuteQuery("ALTER TABLE Tbl_Kamar ADD CONSTRAINT FK_Kamar_Tipe FOREIGN KEY (TipeID) REFERENCES Tbl_TipeKamar(TipeID)", conn);

                        // Drop Old Column
                        ExecuteQuery("ALTER TABLE Tbl_Kamar DROP COLUMN TipeKamar", conn);
                    }
                }

                // 4. Cek Tabel Penghuni
                if (!CheckIfTableExists("Tbl_Penghuni", conn))
                {
                    string queryCreate = @"CREATE TABLE Tbl_Penghuni (
                        PenghuniID INT AUTO_INCREMENT PRIMARY KEY, 
                        Nama VARCHAR(100) NOT NULL,
                        NoKTP VARCHAR(20) NOT NULL,
                        NoHP VARCHAR(20) NOT NULL,
                        KamarID INT,
                        TanggalMasuk DATETIME DEFAULT CURRENT_TIMESTAMP,
                        FOREIGN KEY (KamarID) REFERENCES Tbl_Kamar(KamarID)
                    )";
                    ExecuteQuery(queryCreate, conn);
                }

                // 5. Cek Tabel Pembayaran
                if (!CheckIfTableExists("Tbl_Pembayaran", conn))
                {
                    string queryCreate = @"CREATE TABLE Tbl_Pembayaran (
                        PembayaranID INT AUTO_INCREMENT PRIMARY KEY, 
                        PenghuniID INT NOT NULL,
                        Jumlah DECIMAL(18,2) NOT NULL,
                        TanggalBayar DATETIME DEFAULT CURRENT_TIMESTAMP,
                        Statuses VARCHAR(20) DEFAULT 'Lunas',
                        FOREIGN KEY (PenghuniID) REFERENCES Tbl_Penghuni(PenghuniID)
                    )";
                    ExecuteQuery(queryCreate, conn);
                }

                // 6. Cek Tabel Booking (NEW)
                if (!CheckIfTableExists("Tbl_Booking", conn))
                {
                    string queryCreate = @"CREATE TABLE Tbl_Booking (
                        BookingID INT AUTO_INCREMENT PRIMARY KEY,
                        UserID INT NOT NULL,
                        KamarID INT NOT NULL,
                        TanggalBooking DATETIME DEFAULT CURRENT_TIMESTAMP,
                        Status VARCHAR(20) DEFAULT 'Pending',
                        Nama VARCHAR(100),
                        NoKTP VARCHAR(20),
                        NoHP VARCHAR(20),
                        FOREIGN KEY (UserID) REFERENCES tbl_user(UserID),
                        FOREIGN KEY (KamarID) REFERENCES Tbl_Kamar(KamarID)
                    )";
                    ExecuteQuery(queryCreate, conn);
                }
                else
                {
                    // Update Schema if table exists but columns missing (For existing DB)
                    if (!CheckIfColumnExists("Tbl_Booking", "Nama", conn))
                        ExecuteQuery("ALTER TABLE Tbl_Booking ADD COLUMN Nama VARCHAR(100)", conn);
                    if (!CheckIfColumnExists("Tbl_Booking", "NoKTP", conn))
                        ExecuteQuery("ALTER TABLE Tbl_Booking ADD COLUMN NoKTP VARCHAR(20)", conn);
                    if (!CheckIfColumnExists("Tbl_Booking", "NoHP", conn))
                        ExecuteQuery("ALTER TABLE Tbl_Booking ADD COLUMN NoHP VARCHAR(20)", conn);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal melakukan migrasi database otomatis: " + ex.Message);
            }
            finally
            {
                if (conn.State == System.Data.ConnectionState.Open) conn.Close();
            }
        }

        private static bool CheckIfTableExists(string tableName, MySqlConnection conn)
        {
            MySqlCommand cmd = new MySqlCommand($"SHOW TABLES LIKE '{tableName}'", conn);
            return cmd.ExecuteScalar() != null;
        }

        private static bool CheckIfColumnExists(string tableName, string colName, MySqlConnection conn)
        {
            MySqlCommand cmd = new MySqlCommand($"SHOW COLUMNS FROM {tableName} LIKE '{colName}'", conn);
            return cmd.ExecuteScalar() != null;
        }

        private static void ExecuteQuery(string query, MySqlConnection conn)
        {
            MySqlCommand cmd = new MySqlCommand(query, conn);
            cmd.ExecuteNonQuery();
        }
    }
}
