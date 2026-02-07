using System;
using System.Data;

namespace SmartKos.Interface
{
    /// <summary>
    /// Interface generic untuk operasi CRUD (Abstraction)
    /// Mendemonstrasikan konsep ABSTRACTION dalam OOP
    /// </summary>
    /// <typeparam name="T">Tipe model yang akan digunakan</typeparam>
    public interface IRepository<T>
    {
        /// <summary>
        /// Mengambil semua data dari database
        /// </summary>
        /// <summary>
        /// Mengambil semua data dari database
        /// </summary>
        System.Collections.Generic.List<T> GetAll();

        /// <summary>
        /// Menambah data baru ke database
        /// </summary>
        bool Add(T entity);

        /// <summary>
        /// Mengupdate data yang sudah ada
        /// </summary>
        bool Update(T entity);

        /// <summary>
        /// Menghapus data berdasarkan ID
        /// </summary>
        bool Delete(string id);

        /// <summary>
        /// Mencari data berdasarkan keyword
        /// </summary>
        DataTable Search(string keyword);
    }
}
