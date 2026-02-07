using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SmartKos.view
{
    public static class UIHelper
    {
        // Color Palette - Professional & Colorful (Inferred "Smart" Theme: Blue/Teal/White)
        public static Color PrimaryColor = Color.FromArgb(52, 152, 219); // Bright Blue
        public static Color DarkPrimaryColor = Color.FromArgb(41, 128, 185);
        public static Color AccentColor = Color.FromArgb(230, 126, 34); // Orange for actions
        public static Color BackgroundColor = Color.FromArgb(236, 240, 241); // Soft Cloud
        public static Color SurfaceColor = Color.White;
        public static Color TextColor = Color.FromArgb(44, 62, 80); // Dark Slate
        public static Color SuccessColor = Color.FromArgb(46, 204, 113); // Emerald
        public static Color DangerColor = Color.FromArgb(231, 76, 60); // Alizarin
        public static Color WarningColor = Color.FromArgb(241, 196, 15); // Sunflower
        public static Color SecondaryColor = Color.FromArgb(149, 165, 166); // Concrete Gray for Neutral actions

        // Fonts
        public static Font MainFont = new Font("Segoe UI", 10F, FontStyle.Regular);
        public static Font HeaderFont = new Font("Segoe UI", 14F, FontStyle.Bold);
        public static Font TitleFont = new Font("Segoe UI", 18F, FontStyle.Bold);

        public static void SetFormStyle(Form form)
        {
            // form.BackColor = BackgroundColor; // Disable flat color in favor of gradient potentially
            form.Font = MainFont;
            form.ForeColor = TextColor;
            
            // Clean borders or no borders if we want custom title bar (sticking to standard but styled)
            // form.FormBorderStyle = FormBorderStyle.Sizable; // Allow resizing for responsiveness
            
            // Apply Gradient to Form
            form.Paint += (s, e) => PaintGradientBackground(form, e.Graphics, BackgroundColor, Color.White);

            StyleContainer(form);
        }

        private static void StyleContainer(Control container)
        {
            foreach (Control c in container.Controls)
            {
                if (c is Label) StyleLabel((Label)c);
                if (c is Button) StyleButton((Button)c, false); // Default to secondary
                if (c is TextBox) StyleTextBox((TextBox)c);
                if (c is ComboBox) StyleComboBox((ComboBox)c);
                if (c is GroupBox) {
                    c.ForeColor = TextColor;
                    c.Font = new Font(MainFont, FontStyle.Bold);
                    StyleContainer(c); 
                }
                if (c is Panel) StyleContainer(c); 
                if (c is MenuStrip) StyleMenuStrip((MenuStrip)c);
            }
        }

        // --- NEW FEATURES Phase 2 ---
        
        public static void StyleMenuStrip(MenuStrip menu)
        {
            menu.BackColor = PrimaryColor;
            menu.ForeColor = Color.White;
            menu.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            menu.Renderer = new ToolStripProfessionalRenderer(new CustomColorTable());
        }
        
        // Helper for Menu Colors
        private class CustomColorTable : ProfessionalColorTable
        {
            public override Color MenuItemSelected => DarkPrimaryColor;
            public override Color MenuItemSelectedGradientBegin => DarkPrimaryColor;
            public override Color MenuItemSelectedGradientEnd => DarkPrimaryColor;
            public override Color MenuItemPressedGradientBegin => DarkPrimaryColor;
            public override Color MenuItemPressedGradientEnd => DarkPrimaryColor;
        }

        public static void SetRoundedRegion(Control c, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int d = radius * 2;
            path.AddArc(0, 0, d, d, 180, 90);
            path.AddArc(c.Width - d, 0, d, d, 270, 90);
            path.AddArc(c.Width - d, c.Height - d, d, d, 0, 90);
            path.AddArc(0, c.Height - d, d, d, 90, 90);
            path.CloseFigure();
            c.Region = new Region(path);
        }

        public static void PaintGradientBackground(Control c, Graphics g, Color start, Color end)
        {
            if (c.ClientRectangle.Width == 0 || c.ClientRectangle.Height == 0) return;
            // Full screen gradient
            using (LinearGradientBrush brush = new LinearGradientBrush(c.ClientRectangle, start, end, 90F))
            {
                g.FillRectangle(brush, c.ClientRectangle);
            }
        }

        public static void StyleButton(Button btn, bool primary = true)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btn.Cursor = Cursors.Hand;
            btn.Height = 40; // Consistent height

            // Hook into Resize to keep rounded shape correct
            // Remove previous handlers to avoid duplicate calls if styled multiple times
            // btn.Resize -= ... (Too complex to remove lambda, just assume called once)
            btn.Resize += (s, e) => SetRoundedRegion(btn, 10);
            SetRoundedRegion(btn, 10);

            if (primary)
            {
                btn.BackColor = PrimaryColor;
                btn.ForeColor = Color.White;
            }
            else
            {
                string text = btn.Text.ToLower();
                if (text.Contains("hapus") || text.Contains("cancel") || text.Contains("batal"))
                {
                    btn.BackColor = DangerColor;
                    btn.ForeColor = Color.White;
                }
                else if (text.Contains("clear") || text.Contains("refresh"))
                {
                    btn.BackColor = SecondaryColor;
                    btn.ForeColor = Color.White;
                }
                else if (text.Contains("ubah") || text.Contains("edit"))
                {
                    btn.BackColor = WarningColor;
                    btn.ForeColor = Color.White;
                }
                else
                {
                    btn.BackColor = PrimaryColor; // Default nice blue
                    btn.ForeColor = Color.White;
                }
            }
        }

        public static void StyleTextBox(TextBox txt)
        {
            txt.Font = new Font("Segoe UI", 11F); // Slightly larger
            txt.BorderStyle = BorderStyle.FixedSingle;
            txt.BackColor = Color.White;
        }

        public static void StyleComboBox(ComboBox cmb)
        {
            cmb.Font = new Font("Segoe UI", 11F);
            cmb.FlatStyle = FlatStyle.Flat;
            cmb.BackColor = Color.White;
        }

        public static void StyleLabel(Label lbl, bool isHeader = false)
        {
            lbl.BackColor = Color.Transparent; // Important for gradient
            if (isHeader)
            {
                lbl.Font = HeaderFont;
                lbl.ForeColor = PrimaryColor;
            }
            else
            {
                lbl.Font = MainFont;
                lbl.ForeColor = TextColor;
            }
        }

        public static void StyleDataGrid(DataGridView dgv)
        {
            dgv.BackgroundColor = Color.WhiteSmoke;
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            
            // RESPONSIVE FIX
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = PrimaryColor;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgv.ColumnHeadersHeight = 45;

            dgv.DefaultCellStyle.BackColor = SurfaceColor;
            dgv.DefaultCellStyle.ForeColor = TextColor;
            dgv.DefaultCellStyle.Font = MainFont;
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(200, 230, 255); // Lighter selection
            dgv.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgv.DefaultCellStyle.Padding = new Padding(6);
            
            dgv.RowTemplate.Height = 40; // Taller rows
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 248, 250);
        }

        // Custom Gradient Panel helper if needed
        public static void SetGradientBackground(Control control)
        {
            // This would require handling the Paint event, might be too complex for a static helper 
            // without subclassing. We'll skip for now or use solid colors for simplicity.
            control.BackColor = PrimaryColor;
        }
    }
}
