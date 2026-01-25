-- Migration script to add Tbl_TipeKamar and normalize Tbl_Kamar

USE smartkos;

-- 1. Create Tbl_TipeKamar
CREATE TABLE IF NOT EXISTS Tbl_TipeKamar (
    TipeID INT AUTO_INCREMENT PRIMARY KEY,
    NamaTipe VARCHAR(50) NOT NULL UNIQUE
);

-- 2. Insert default data
INSERT INTO Tbl_TipeKamar (NamaTipe) VALUES 
('AC'), 
('Non-AC'), 
('VVIP');

-- 3. Add TipeID column to Tbl_Kamar
ALTER TABLE Tbl_Kamar ADD COLUMN TipeID INT;

-- 4. Migrate existing data (Match string to ID)
UPDATE Tbl_Kamar k
JOIN Tbl_TipeKamar t ON k.TipeKamar = t.NamaTipe
SET k.TipeID = t.TipeID;

-- 5. Add Foreign Key
ALTER TABLE Tbl_Kamar
ADD CONSTRAINT FK_Kamar_Tipe
FOREIGN KEY (TipeID) REFERENCES Tbl_TipeKamar(TipeID);

-- 6. Drop old string column (TipeKamar)
-- WARNING: Ensure application code is updated to use JOINs before running this!
ALTER TABLE Tbl_Kamar DROP COLUMN TipeKamar;
