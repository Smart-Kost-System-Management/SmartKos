using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmartKos.view
{
    public partial class FormSplash : Form
    {
        public FormSplash()
        {
            InitializeComponent();
            UIHelper.SetFormStyle(this);
            // Override with special gradient for Splash
            this.Paint += (s, e) => UIHelper.PaintGradientBackground(this, e.Graphics, UIHelper.PrimaryColor, UIHelper.DarkPrimaryColor);
            
            // Center controls
            if (pictureBox1 != null) {
                pictureBox1.Left = (this.ClientSize.Width - pictureBox1.Width) / 2;
                pictureBox1.Top = (this.ClientSize.Height - pictureBox1.Height) / 2 - 40;
            }
            
            if (progressBar1 != null) {
                progressBar1.Left = (this.ClientSize.Width - progressBar1.Width) / 2;
                progressBar1.Top = (pictureBox1 != null) ? pictureBox1.Bottom + 20 : 300;
            }

            if (label1 != null) {
                label1.Left = (this.ClientSize.Width - label1.Width) / 2;
                label1.Top = (progressBar1 != null) ? progressBar1.Bottom + 10 : 330;
            }

            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // Pastikan nilai tidak lebih dari 100
            if (progressBar1.Value < 100)
            {
                progressBar1.Value += 2;
            }
            else
            {
                timer1.Stop();

                // Sesuai syarat Tugas Besar: Buka Login dulu
                FormLogin login = new FormLogin();
                login.Show();

                this.Hide();
            }
        }
        private void FormSplash_Load(object sender, EventArgs e)
        {
            // Placeholder for any implementation needed on load
        }
    }
}
