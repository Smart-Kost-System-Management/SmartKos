using System;

namespace SmartKos.model
{
    /// <summary>
    /// Model User dengan proper Encapsulation
    /// Mendemonstrasikan konsep ENCAPSULATION dalam OOP
    /// menggunakan private fields + public properties
    /// </summary>
    public class M_User
    {
        // Private fields - tidak bisa diakses langsung dari luar class (Encapsulation)
        private int _id;
        private string _nama;
        private string _username;
        private string _password;
        private string _role;
        private DateTime _createdAt;

        // Public Properties - akses terkontrol ke private fields

        /// <summary>
        /// ID User (Auto Increment dari database)
        /// </summary>
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        /// Nama lengkap user
        /// </summary>
        public string Nama
        {
            get { return _nama; }
            set
            {
                // Validasi: nama tidak boleh kosong
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _nama = value.Trim();
                }
            }
        }

        /// <summary>
        /// Username untuk login
        /// </summary>
        public string Username
        {
            get { return _username; }
            set
            {
                // Validasi: username tidak boleh kosong dan tanpa spasi
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _username = value.Trim().Replace(" ", "");
                }
            }
        }

        /// <summary>
        /// Password user (sebaiknya di-hash sebelum disimpan)
        /// </summary>
        public string Password
        {
            get { return _password; }
            set
            {
                // Validasi: password minimal 4 karakter
                if (!string.IsNullOrEmpty(value) && value.Length >= 4)
                {
                    _password = value;
                }
            }
        }

        /// <summary>
        /// Role user (Admin/User)
        /// </summary>
        public string Role
        {
            get { return _role; }
            set
            {
                // Validasi: hanya role yang valid
                if (value == "Admin" || value == "User" || value == "Pemilik")
                {
                    _role = value;
                }
                else
                {
                    _role = "User"; // Default role
                }
            }
        }

        /// <summary>
        /// Tanggal pembuatan akun
        /// </summary>
        public DateTime CreatedAt
        {
            get { return _createdAt; }
            set { _createdAt = value; }
        }

        // Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public M_User()
        {
            _createdAt = DateTime.Now;
            _role = "User";
        }

        /// <summary>
        /// Constructor dengan parameter lengkap
        /// </summary>
        public M_User(string nama, string username, string password, string role)
        {
            Nama = nama;
            Username = username;
            Password = password;
            Role = role;
            _createdAt = DateTime.Now;
        }

        /// <summary>
        /// Method untuk menampilkan info user (untuk debugging)
        /// </summary>
        public override string ToString()
        {
            return $"User: {_nama} ({_username}) - Role: {_role}";
        }
    }
}
