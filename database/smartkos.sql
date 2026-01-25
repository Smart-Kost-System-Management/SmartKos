-- Script untuk membuat database SmartKos
-- Jalankan script ini di phpMyAdmin atau MySQL Workbench

-- Buat database (jika belum ada)
CREATE DATABASE IF NOT EXISTS smartkos;

-- Gunakan database
USE smartkos;

-- Buat tabel Tbl_User untuk registrasi dan login
CREATE TABLE IF NOT EXISTS Tbl_User (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Nama VARCHAR(100) NOT NULL,
    Username VARCHAR(50) NOT NULL UNIQUE,
    Password VARCHAR(255) NOT NULL,
    Role VARCHAR(20) NOT NULL,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
);

-- Contoh data awal (opsional)
-- INSERT INTO Tbl_User (Nama, Username, Password, Role) VALUES ('Admin', 'admin', 'admin123', 'Admin');
