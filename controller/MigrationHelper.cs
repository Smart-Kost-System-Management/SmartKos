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
