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
    }
}
