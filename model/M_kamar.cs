using System;

namespace SmartKos.model
{
    /// <summary>
    /// Model Kamar dengan proper Encapsulation
    /// Mendemonstrasikan konsep ENCAPSULATION dalam OOP
    /// menggunakan private fields + public properties dengan validasi
    /// </summary>
    public class M_kamar
    {
        // Private fields - tidak bisa diakses langsung dari luar class (Encapsulation)
        private int _kamarId;
        private string _nomorKamar;
        private string _status;
        private string _tipeKamar;
        private int _tipeId;
        private decimal _harga;

        // Public Properties dengan validasi

        /// <summary>
        /// ID Kamar (Primary Key dari database)
        /// </summary>
        public int KamarID
        {
            get { return _kamarId; }
            set { _kamarId = value; }
        }

        /// <summary>
        /// Nomor Kamar (contoh: "101", "A1", dll)
        /// </summary>
        public string NomorKamar
        {
            get { return _nomorKamar; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _nomorKamar = value.Trim().ToUpper();
                }
            }
        }

        /// <summary>
        /// Status Kamar (Terisi/Kosong/Maintenance)
        /// </summary>
        public string Status
        {
            get { return _status; }
            set
            {
                // Validasi: hanya status yang valid
                if (value == "Terisi" || value == "Kosong" || value == "Maintenance")
                {
                    _status = value;
                }
                else
                {
                    _status = "Kosong"; // Default status
                }
            }
        }

        /// <summary>
        /// Tipe Kamar (AC/Non-AC/VVIP) - untuk display
        /// </summary>
        public string TipeKamar
        {
            get { return _tipeKamar; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _tipeKamar = value.Trim();
                }
            }
        }

        /// <summary>
        /// TipeID - Foreign Key ke Tbl_TipeKamar
        /// </summary>
        public int TipeId
        {
            get { return _tipeId; }
            set
            {
                if (value > 0)
                {
                    _tipeId = value;
                }
            }
        }

        /// <summary>
        /// Harga sewa per bulan
        /// </summary>
        public decimal Harga
        {
            get { return _harga; }
            set
            {
                // Validasi: harga tidak boleh negatif
                if (value >= 0)
                {
                    _harga = value;
                }
            }
        }

        // Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public M_kamar()
        {
            _status = "Kosong";
            _harga = 0;
        }

        /// <summary>
        /// Constructor dengan parameter
        /// </summary>
        public M_kamar(string nomorKamar, string tipeKamar, decimal harga, string status)
        {
            NomorKamar = nomorKamar;
            TipeKamar = tipeKamar;
            Harga = harga;
            Status = status;
        }

        /// <summary>
        /// Method untuk format harga dengan currency
        /// </summary>
        public string GetFormattedHarga()
        {
            return string.Format("Rp {0:N0}", _harga);
        }

        /// <summary>
        /// Override ToString untuk debugging
        /// </summary>
        public override string ToString()
        {
            return $"Kamar {_nomorKamar} - {_tipeKamar} - {GetFormattedHarga()} - {_status}";
        }

        // Extended Property for Display
        public string NamaPenghuni { get; set; }
    }
}
