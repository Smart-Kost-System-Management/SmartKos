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

            Application.Run(new view.FormSplash());
        }
    }
}
