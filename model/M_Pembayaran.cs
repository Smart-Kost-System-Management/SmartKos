using System;

namespace SmartKos.model
{
    public class M_Pembayaran
    {
        private int _pembayaranId;
        private int _penghuniId;
        private decimal _jumlah;
        private DateTime _tanggalBayar;
        private string _status;

        public int PembayaranID
        {
            get { return _pembayaranId; }
            set { _pembayaranId = value; }
        }

        public int PenghuniID
        {
            get { return _penghuniId; }
            set { _penghuniId = value; }
        }

        public decimal Jumlah
        {
            get { return _jumlah; }
            set 
            {
                if (value > 0)
                    _jumlah = value;
                else
                    throw new ArgumentException("Jumlah pembayaran harus lebih dari 0");
            }
        }

        public DateTime TanggalBayar
        {
            get { return _tanggalBayar; }
            set { _tanggalBayar = value; }
        }

        public string Status
        {
            get { return _status; }
            set { _status = value; }
        }

        // Extended Properties from JOINS
        public string NamaPenghuni { get; set; }
        public string NomorKamar { get; set; }

        public M_Pembayaran()
        {
            _tanggalBayar = DateTime.Now;
            _status = "Lunas";
        }
    }
}
