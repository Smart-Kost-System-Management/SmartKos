using System;

namespace SmartKos.model
{
    public class M_Penghuni
    {
        private int _penghuniId;
        private string _nama;
        private string _noKTP;
        private string _noHP;
        private int _kamarId;
        private DateTime _tanggalMasuk;

        // Encapsulation
        public int PenghuniID
        {
            get { return _penghuniId; }
            set { _penghuniId = value; }
        }

        public string Nama
        {
            get { return _nama; }
            set 
            {
                if (!string.IsNullOrWhiteSpace(value))
                    _nama = value.Trim();
            }
        }

        public string NoKTP
        {
            get { return _noKTP; }
            set 
            {
                if (!string.IsNullOrWhiteSpace(value) && value.Length >= 10)
                    _noKTP = value.Trim();
                else
                    throw new ArgumentException("No KTP tidak valid (min 10 digit)");
            }
        }

        public string NoHP
        {
            get { return _noHP; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && value.Length >= 10)
                    _noHP = value.Trim();
                else
                    throw new ArgumentException("No HP tidak valid (min 10 digit)");
            }
        }

        public int KamarID
        {
            get { return _kamarId; }
            set { _kamarId = value; }
        }

        public DateTime TanggalMasuk
        {
            get { return _tanggalMasuk; }
            set { _tanggalMasuk = value; }
        }

        // Extended Property from JOIN
        public string NomorKamar { get; set; }

        public M_Penghuni()
        {
            _tanggalMasuk = DateTime.Now;
        }
    }
}
