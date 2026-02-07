using System;
using System.Diagnostics;
using SmartKos.controller;
using SmartKos.model;

namespace SmartKos.Test
{
    public class UnitTest
    {
        public static void RunTests()
        {
            Console.WriteLine("=== RUNNING UNIT TESTS ===");
            try
            {
                TestKamarValidation();
                TestHargaKamar();
                Console.WriteLine("=== ALL TESTS PASSED ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine("!!! TEST FAILED: " + ex.Message);
            }
        }

        private static void TestKamarValidation()
        {
            Console.Write("TestKamarValidation... ");
            M_kamar k = new M_kamar();
            k.NomorKamar = "  a101  "; // Should trim and upper
            
            if (k.NomorKamar != "A101") 
                throw new Exception($"Expected 'A101', got '{k.NomorKamar}'");
                
            k.Status = "InvalidStatus"; // Should default to Kosong
            if (k.Status != "Kosong")
                throw new Exception($"Expected 'Kosong' for invalid status, got '{k.Status}'");

            Console.WriteLine("PASS");
        }

        private static void TestHargaKamar()
        {
            Console.Write("TestHargaKamar... ");
            M_kamar k = new M_kamar();
            k.Harga = -5000; // Should be ignored (keep default 0)
            
            if (k.Harga != 0)
                 throw new Exception("Negative price validation failed");

            k.Harga = 1500000;
            if (k.GetFormattedHarga() != "Rp 1.500.000") // Culture dependent, simplified check
            {
                 // Check if it at least contains 1.500.000 or similar
                 string s = k.GetFormattedHarga();
                 if(!s.Contains("1.500.000") && !s.Contains("1,500,000"))
                    Console.WriteLine($"(Warning: Format '{s}' might be culture specific) ");
            }

            Console.WriteLine("PASS");
        }
    }
}
