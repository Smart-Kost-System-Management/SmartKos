using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace SmartKos.controller
{
    internal class Koneksi
    {
        public static string connString = "server=localhost;port=3306;database=smartkos;uid=root;pwd=;";
        // Perhatikan tipe datanya harus MySqlConnection
        public MySqlConnection conn;

        public Koneksi()
        {
            conn = new MySqlConnection(connString);
        }

        // Fungsi ini harus mengembalikan MySqlConnection
        public MySqlConnection GetConn()
        {
            return conn;
        }
    }
}
