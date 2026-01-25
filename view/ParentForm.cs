using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SmartKos.view;

namespace SmartKos.view
{
    public partial class ParentForm : Form
    {
        public ParentForm()
        {
            InitializeComponent();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void dashboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormDashboard FrmDashboard = new FormDashboard();
            FrmDashboard.MdiParent = this;
            FrmDashboard.Show();
        }

        private void signInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormLogin FrmLogin = new FormLogin();
            FrmLogin.MdiParent = this;
            FrmLogin.Show();
        }

        private void signUpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormRegistrasi formRegistrasi = new FormRegistrasi();
            formRegistrasi.MdiParent = this;
            formRegistrasi.Show();
        }
    }
}
