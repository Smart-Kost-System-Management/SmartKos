using System;

namespace SmartKos.model
{
    public class M_Booking
    {
        private int _bookingId;
        private int _userId;
        private int _kamarId;
        private DateTime _tanggalBooking;
        private string _status;
        
        // Candidate Data
        private string _nama;
        private string _noKtp;
        private string _noHp;
        
        // Extra properties for JOIN display
        private string _userName;
        private string _nomorKamar;

        public int BookingID { get { return _bookingId; } set { _bookingId = value; } }
        public int UserID { get { return _userId; } set { _userId = value; } }
        public int KamarID { get { return _kamarId; } set { _kamarId = value; } }
        public DateTime TanggalBooking { get { return _tanggalBooking; } set { _tanggalBooking = value; } }
        public string Status { get { return _status; } set { _status = value; } }

        public string Nama { get { return _nama; } set { _nama = value; } }
        public string NoKTP { get { return _noKtp; } set { _noKtp = value; } }
        public string NoHP { get { return _noHp; } set { _noHp = value; } }

        // Display Properties
        public string UserName { get { return _userName; } set { _userName = value; } }
        public string NomorKamar { get { return _nomorKamar; } set { _nomorKamar = value; } }

        public M_Booking()
        {
            _tanggalBooking = DateTime.Now;
            _status = "Pending";
        }
    }
}
