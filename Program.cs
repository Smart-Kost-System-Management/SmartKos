using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmartKos
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // PENTING: Jangan dihapus! Ini fungsi untuk memperbaiki database otomatis
            // agar error "Table doesn't exist" hilang.
            controller.MigrationHelper.CheckAndApplyMigration();

            // Run Unit Tests (Manual Quality Check)
            try { 
                SmartKos.Test.UnitTest.RunTests(); 
            } catch(Exception ex) { 
                MessageBox.Show("Unit Test Failed: " + ex.Message); 
            }

            Application.Run(new view.FormSplash());
        }
    }
}
