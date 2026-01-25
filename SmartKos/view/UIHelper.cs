using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SmartKos.view
{
    public static class UIHelper
    {
        // Color Palette
        public static Color PrimaryColor = Color.FromArgb(78, 115, 223); // #4e73df (Royal Blue)
        public static Color DarkPrimaryColor = Color.FromArgb(34, 74, 190);
        public static Color SecondaryColor = Color.FromArgb(133, 135, 150); // #858796 (Gray)
        public static Color BackgroundColor = Color.FromArgb(248, 249, 252); // #f8f9fc (Light Gray/White)
        public static Color SurfaceColor = Color.White;
        public static Color TextColor = Color.FromArgb(90, 92, 105); // #5a5c69 (Dark Gray)
        public static Color SuccessColor = Color.FromArgb(28, 200, 138);
        public static Color DangerColor = Color.FromArgb(231, 74, 59);
        public static Color WarningColor = Color.FromArgb(246, 194, 62);

        // Fonts
        public static Font MainFont = new Font("Segoe UI", 10F, FontStyle.Regular);
        public static Font HeaderFont = new Font("Segoe UI", 14F, FontStyle.Bold);
        public static Font TitleFont = new Font("Segoe UI", 18F, FontStyle.Bold);

        public static void SetFormStyle(Form form)
        {
            // form.BackColor = BackgroundColor; // Disable flat color in favor of gradient potentially
            form.Font = MainFont;
            form.ForeColor = TextColor;
            form.FormBorderStyle = FormBorderStyle.FixedSingle; // Clean borders
            
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
                if (c is GroupBox) {
                    c.ForeColor = TextColor; 
                    StyleContainer(c); 
                }
                if (c is Panel) StyleContainer(c); 
            }
        }

        // --- NEW FEATURES Phase 2 ---

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
            using (LinearGradientBrush brush = new LinearGradientBrush(c.ClientRectangle, start, end, 45F))
            {
                g.FillRectangle(brush, c.ClientRectangle);
            }
        }

        public static void StyleCard(Panel p)
        {
            p.BackColor = SurfaceColor;
            // SetRoundedRegion(p, 20); // Make panel rounded
            // Note: Region clipping might cut off borders, so for cards we often just rely on the bg color 
            // or we use a custom paint to draw a rounded border.
            // For simplicity, let's just round it.
            p.Paint += (s, e) => {
                // Optional: Draw shadow or border here if we want to be fancy
            };
        }

        public static void StyleButton(Button btn, bool primary = true)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btn.Cursor = Cursors.Hand;
            btn.Size = new Size(btn.Width, 45); // Taller for modern look

            // Hook into Resize to keep rounded shape correct
            btn.Resize += (s, e) => SetRoundedRegion(btn, 15);
            SetRoundedRegion(btn, 15); // Initial set

            if (primary)
            {
                btn.BackColor = PrimaryColor;
                btn.ForeColor = Color.White;
            }
            else
            {
                // Determine if it's a specific action button based on text or name if needed, 
                // but for now let's assume non-primary are secondary/outline or gray.
                // Or user can manually set colors after calling this.
                // Let's make "Batal" or "Clear" buttons gray.
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
                     // Default secondary
                    btn.BackColor = SecondaryColor;
                    btn.ForeColor = Color.White;
                }
            }
        }

        public static void StyleTextBox(TextBox txt)
        {
            txt.Font = new Font("Segoe UI", 10F);
            txt.BorderStyle = BorderStyle.FixedSingle; 
            // Cannot easily change padding in standard textbox without custom control, 
            // but we can increase height by font size.
        }

        public static void StyleLabel(Label lbl, bool isHeader = false)
        {
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
            dgv.BackgroundColor = SurfaceColor;
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = PrimaryColor;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgv.ColumnHeadersHeight = 40;

            dgv.DefaultCellStyle.BackColor = SurfaceColor;
            dgv.DefaultCellStyle.ForeColor = TextColor;
            dgv.DefaultCellStyle.Font = MainFont;
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(230, 240, 255); // Light blue selection
            dgv.DefaultCellStyle.SelectionForeColor = TextColor;
            dgv.DefaultCellStyle.Padding = new Padding(5);
            
            dgv.RowTemplate.Height = 35;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = BackgroundColor;
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
